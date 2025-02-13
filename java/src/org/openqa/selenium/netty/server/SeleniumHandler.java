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

import io.netty.channel.ChannelHandlerContext;
import io.netty.channel.SimpleChannelInboundHandler;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;
import java.util.concurrent.Future;
import org.openqa.selenium.internal.Require;
import org.openqa.selenium.remote.ErrorFilter;
import org.openqa.selenium.remote.http.HttpHandler;
import org.openqa.selenium.remote.http.HttpRequest;
import org.openqa.selenium.remote.http.HttpResponse;

class SeleniumHandler extends SimpleChannelInboundHandler<HttpRequest> {

  private static final ExecutorService EXECUTOR = Executors.newCachedThreadPool();
  private final HttpHandler seleniumHandler;
  private Future<?> lastOne;

  public SeleniumHandler(HttpHandler seleniumHandler) {
    super(HttpRequest.class);
    this.seleniumHandler = Require.nonNull("HTTP handler", seleniumHandler).with(new ErrorFilter());
    this.lastOne = CompletableFuture.completedFuture(null);
  }

  @Override
  protected void channelRead0(ChannelHandlerContext ctx, HttpRequest msg) {
    lastOne =
        EXECUTOR.submit(
            () -> {
              HttpResponse res = seleniumHandler.execute(msg);
              ctx.writeAndFlush(res);
            });
  }

  @Override
  public void channelInactive(ChannelHandlerContext ctx) throws Exception {
    lastOne.cancel(true);
    super.channelInactive(ctx);
  }
}
