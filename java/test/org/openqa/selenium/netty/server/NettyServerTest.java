// Licensed to the Software Freedom Conservancy (SFC) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The SFC licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.

package org.openqa.selenium.netty.server;

import static org.assertj.core.api.Assertions.assertThat;
import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.junit.jupiter.api.Assertions.assertNull;
import static org.junit.jupiter.api.Assertions.assertTrue;
import static org.openqa.selenium.remote.http.Contents.utf8String;
import static org.openqa.selenium.remote.http.HttpMethod.DELETE;
import static org.openqa.selenium.remote.http.HttpMethod.GET;

import com.google.common.collect.ImmutableMap;
import java.net.URL;
import java.time.Duration;
import java.util.concurrent.CountDownLatch;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.atomic.AtomicInteger;
import org.junit.jupiter.api.Assertions;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.TimeoutException;
import org.openqa.selenium.grid.config.CompoundConfig;
import org.openqa.selenium.grid.config.Config;
import org.openqa.selenium.grid.config.MapConfig;
import org.openqa.selenium.grid.server.BaseServerOptions;
import org.openqa.selenium.grid.server.Server;
import org.openqa.selenium.net.PortProber;
import org.openqa.selenium.remote.http.ClientConfig;
import org.openqa.selenium.remote.http.HttpClient;
import org.openqa.selenium.remote.http.HttpRequest;
import org.openqa.selenium.remote.http.HttpResponse;

class NettyServerTest {

  /**
   * There is a bug between an OkHttp client and the Netty server where a TCP RST causes the same
   * HTTP request to be generated twice. This is clearly less than desirable behaviour, so this test
   * attempts to ensure the problem does not occur. I suspect the problem is to do with OkHttp's
   * connection pool, but it seems cruel to make our users deal with this. Better to have it be
   * something the server handles.
   */
  @Test
  void ensureMultipleCallsWorkAsExpected() {
    System.out.println("\n\n\n\nNetty!");

    AtomicInteger count = new AtomicInteger(0);

    Server<?> server =
        new NettyServer(
                new BaseServerOptions(
                    new MapConfig(
                        ImmutableMap.of(
                            "server", ImmutableMap.of("port", PortProber.findFreePort())))),
                req -> {
                  count.incrementAndGet();
                  return new HttpResponse().setContent(utf8String("Count is " + count.get()));
                })
            .start();

    // TODO: avoid using netty for this
    HttpClient client = HttpClient.Factory.createDefault().createClient(server.getUrl());

    HttpResponse res = client.execute(new HttpRequest(GET, "/does-not-matter"));
    outputHeaders(res);
    assertThat(count.get()).isEqualTo(1);

    client.execute(new HttpRequest(GET, "/does-not-matter"));
    outputHeaders(res);
    assertThat(count.get()).isEqualTo(2);
  }

  @Test
  void shouldDisableAllowOrigin() {
    Server<?> server =
        new NettyServer(
                new BaseServerOptions(
                    new MapConfig(
                        ImmutableMap.of(
                            "server", ImmutableMap.of("port", PortProber.findFreePort())))),
                req -> new HttpResponse().setContent(utf8String("Count is ")))
            .start();

    URL url = server.getUrl();
    HttpClient client = HttpClient.Factory.createDefault().createClient(url);
    HttpRequest request = new HttpRequest(DELETE, "/session");
    String exampleUrl = "http://www.example.com";
    request.setHeader("Origin", exampleUrl);
    request.setHeader("Accept", "*/*");
    HttpResponse response = client.execute(request);

    assertNull(
        response.getHeader("Access-Control-Allow-Origin"),
        "Access-Control-Allow-Origin should be null");
  }

  @Test
  void shouldAllowCORS() {
    Config cfg =
        new CompoundConfig(
            new MapConfig(ImmutableMap.of("server", ImmutableMap.of("allow-cors", "true"))));
    BaseServerOptions options = new BaseServerOptions(cfg);
    assertTrue(options.getAllowCORS(), "Allow CORS should be enabled");

    Server<?> server = new NettyServer(options, req -> new HttpResponse()).start();

    URL url = server.getUrl();
    HttpClient client = HttpClient.Factory.createDefault().createClient(url);
    HttpRequest request = new HttpRequest(DELETE, "/session");
    request.setHeader("Origin", "http://www.example.com");
    request.setHeader("Accept", "*/*");
    HttpResponse response = client.execute(request);

    assertEquals(
        "*",
        response.getHeader("Access-Control-Allow-Origin"),
        "Access-Control-Allow-Origin should be equal to origin in request header");
  }

  @Test
  void shouldNotBindToHost() {
    Config cfg =
        new CompoundConfig(
            new MapConfig(
                ImmutableMap.of(
                    "server", ImmutableMap.of("bind-host", "false", "host", "anyRandomHost"))));
    BaseServerOptions options = new BaseServerOptions(cfg);
    assertFalse(options.getBindHost(), "Bind to host should be disabled");

    Server<?> server = new NettyServer(options, req -> new HttpResponse()).start();

    assertEquals("anyRandomHost", server.getUrl().getHost());
  }

  @Test
  void doesInterruptPending() throws Exception {
    CountDownLatch interrupted = new CountDownLatch(1);
    Config cfg = new MapConfig(ImmutableMap.of());
    BaseServerOptions options = new BaseServerOptions(cfg);

    Server<?> server =
        new NettyServer(
                options,
                req -> {
                  try {
                    Thread.sleep(800);
                  } catch (InterruptedException ex) {
                    interrupted.countDown();
                  }
                  return new HttpResponse();
                })
            .start();
    ClientConfig config =
        ClientConfig.defaultConfig()
            .readTimeout(Duration.ofMillis(400))
            .baseUri(server.getUrl().toURI());

    // provoke a client timeout
    Assertions.assertThrows(
        TimeoutException.class,
        () -> {
          try (HttpClient client = HttpClient.Factory.createDefault().createClient(config)) {
            HttpRequest request = new HttpRequest(DELETE, "/session");
            request.setHeader("Accept", "*/*");
            client.execute(request);
          }
        });

    assertTrue(interrupted.await(1000, TimeUnit.MILLISECONDS), "The handling was interrupted");
  }

  private void outputHeaders(HttpResponse res) {
    res.forEachHeader((name, value) -> System.out.printf("%s -> %s\n", name, value));
  }
}
