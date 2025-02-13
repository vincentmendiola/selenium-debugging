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

package org.openqa.selenium.grid.web;

import static org.openqa.selenium.remote.http.Contents.asJson;

import com.google.common.collect.ImmutableMap;
import java.io.UncheckedIOException;
import java.net.URI;
import java.net.URL;
import org.openqa.selenium.UnsupportedCommandException;
import org.openqa.selenium.internal.Require;
import org.openqa.selenium.remote.http.ClientConfig;
import org.openqa.selenium.remote.http.HttpClient;
import org.openqa.selenium.remote.http.HttpRequest;
import org.openqa.selenium.remote.http.HttpResponse;
import org.openqa.selenium.remote.http.WebSocket;

public class RoutableHttpClientFactory implements HttpClient.Factory {

  private final URL self;
  private final CombinedHandler handler;
  private final HttpClient.Factory delegate;

  public RoutableHttpClientFactory(URL self, CombinedHandler handler, HttpClient.Factory delegate) {
    this.self = Require.nonNull("URL", self);
    this.handler = Require.nonNull("Handler", handler);
    this.delegate = Require.nonNull("Delegate", delegate);
  }

  @Override
  public HttpClient createClient(ClientConfig config) {
    Require.nonNull("Client config", config);

    URI url = config.baseUri();

    if (self.getProtocol().equals(url.getScheme())
        && self.getHost().equals(url.getHost())
        && self.getPort() == url.getPort()) {

      return new HttpClient() {
        @Override
        public HttpResponse execute(HttpRequest request) throws UncheckedIOException {
          if (!handler.test(request)) {
            HttpResponse response = new HttpResponse();
            response.setStatus(404);
            response.setContent(
                asJson(
                    ImmutableMap.of(
                        "value", request.getUri(),
                        "message", "Unable to route " + request,
                        "error", UnsupportedCommandException.class.getName())));
            return response;
          }

          return handler.execute(request);
        }

        @Override
        public WebSocket openSocket(HttpRequest request, WebSocket.Listener listener) {
          throw new UnsupportedOperationException("openSocket");
        }
      };
    }

    return delegate.createClient(config);
  }

  @Override
  public void cleanupIdleClients() {
    delegate.cleanupIdleClients();
  }
}
