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

package org.openqa.selenium.bidi.browsingcontext;

import static org.assertj.core.api.AssertionsForClassTypes.assertThat;

import java.util.concurrent.CompletableFuture;
import java.util.concurrent.ExecutionException;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.TimeoutException;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.By;
import org.openqa.selenium.WindowType;
import org.openqa.selenium.bidi.module.BrowsingContextInspector;
import org.openqa.selenium.testing.JupiterTestBase;
import org.openqa.selenium.testing.NeedsFreshDriver;

class BrowsingContextInspectorTest extends JupiterTestBase {

  @Test
  @NeedsFreshDriver
  void canListenToWindowBrowsingContextCreatedEvent()
      throws ExecutionException, InterruptedException, TimeoutException {
    try (BrowsingContextInspector inspector = new BrowsingContextInspector(driver)) {
      CompletableFuture<BrowsingContextInfo> future = new CompletableFuture<>();

      inspector.onBrowsingContextCreated(future::complete);

      String windowHandle = driver.switchTo().newWindow(WindowType.WINDOW).getWindowHandle();

      BrowsingContextInfo browsingContextInfo = future.get(5, TimeUnit.SECONDS);

      assertThat(browsingContextInfo.getId()).isEqualTo(windowHandle);
      assertThat("about:blank").isEqualTo(browsingContextInfo.getUrl());
      assertThat(browsingContextInfo.getId()).isEqualTo(windowHandle);
      assertThat(browsingContextInfo.getChildren()).isEqualTo(null);
      assertThat(browsingContextInfo.getParentBrowsingContext()).isEqualTo(null);
    }
  }

  @Test
  @NeedsFreshDriver
  void canListenToBrowsingContextDestroyedEvent()
      throws ExecutionException, InterruptedException, TimeoutException {
    try (BrowsingContextInspector inspector = new BrowsingContextInspector(driver)) {
      CompletableFuture<BrowsingContextInfo> future = new CompletableFuture<>();

      inspector.onBrowsingContextDestroyed(future::complete);

      String windowHandle = driver.switchTo().newWindow(WindowType.WINDOW).getWindowHandle();

      driver.close();

      BrowsingContextInfo browsingContextInfo = future.get(5, TimeUnit.SECONDS);

      assertThat(browsingContextInfo.getId()).isEqualTo(windowHandle);
      assertThat("about:blank").isEqualTo(browsingContextInfo.getUrl());
      assertThat(browsingContextInfo.getChildren()).isEqualTo(null);
      assertThat(browsingContextInfo.getParentBrowsingContext()).isEqualTo(null);
    }
  }

  @Test
  @NeedsFreshDriver
  void canListenToTabBrowsingContextCreatedEvent()
      throws ExecutionException, InterruptedException, TimeoutException {
    try (BrowsingContextInspector inspector = new BrowsingContextInspector(driver)) {
      CompletableFuture<BrowsingContextInfo> future = new CompletableFuture<>();
      inspector.onBrowsingContextCreated(future::complete);

      String windowHandle = driver.switchTo().newWindow(WindowType.TAB).getWindowHandle();

      BrowsingContextInfo browsingContextInfo = future.get(5, TimeUnit.SECONDS);

      assertThat(browsingContextInfo.getId()).isEqualTo(windowHandle);
      assertThat("about:blank").isEqualTo(browsingContextInfo.getUrl());
      assertThat(browsingContextInfo.getId()).isEqualTo(windowHandle);
      assertThat(browsingContextInfo.getChildren()).isEqualTo(null);
      assertThat(browsingContextInfo.getParentBrowsingContext()).isEqualTo(null);
    }
  }

  @Test
  @NeedsFreshDriver
  void canListenToDomContentLoadedEvent()
      throws ExecutionException, InterruptedException, TimeoutException {
    try (BrowsingContextInspector inspector = new BrowsingContextInspector(driver)) {
      CompletableFuture<NavigationInfo> future = new CompletableFuture<>();
      inspector.onDomContentLoaded(future::complete);

      BrowsingContext context = new BrowsingContext(driver, driver.getWindowHandle());
      context.navigate(appServer.whereIs("/bidi/logEntryAdded.html"), ReadinessState.COMPLETE);

      NavigationInfo navigationInfo = future.get(5, TimeUnit.SECONDS);
      assertThat(navigationInfo.getBrowsingContextId()).isEqualTo(context.getId());
      assertThat(navigationInfo.getUrl()).contains("/bidi/logEntryAdded.html");
    }
  }

  @Test
  @NeedsFreshDriver
  void canListenToBrowsingContextLoadedEvent()
      throws ExecutionException, InterruptedException, TimeoutException {
    try (BrowsingContextInspector inspector = new BrowsingContextInspector(driver)) {
      CompletableFuture<NavigationInfo> future = new CompletableFuture<>();
      inspector.onBrowsingContextLoaded(future::complete);

      BrowsingContext context = new BrowsingContext(driver, driver.getWindowHandle());
      context.navigate(appServer.whereIs("/bidi/logEntryAdded.html"), ReadinessState.COMPLETE);

      NavigationInfo navigationInfo = future.get(5, TimeUnit.SECONDS);
      assertThat(navigationInfo.getBrowsingContextId()).isEqualTo(context.getId());
      assertThat(navigationInfo.getUrl()).contains("/bidi/logEntryAdded.html");
    }
  }

  @Test
  @NeedsFreshDriver
  void canListenToNavigationStartedEvent()
      throws ExecutionException, InterruptedException, TimeoutException {
    try (BrowsingContextInspector inspector = new BrowsingContextInspector(driver)) {
      CompletableFuture<NavigationInfo> future = new CompletableFuture<>();
      inspector.onNavigationStarted(future::complete);

      BrowsingContext context = new BrowsingContext(driver, driver.getWindowHandle());
      context.navigate(appServer.whereIs("/bidi/logEntryAdded.html"), ReadinessState.COMPLETE);

      NavigationInfo navigationInfo = future.get(5, TimeUnit.SECONDS);
      assertThat(navigationInfo.getBrowsingContextId()).isEqualTo(context.getId());
      assertThat(navigationInfo.getUrl()).contains("/bidi/logEntryAdded.html");
    }
  }

  @Test
  @NeedsFreshDriver
  void canListenToFragmentNavigatedEvent()
      throws ExecutionException, InterruptedException, TimeoutException {
    try (BrowsingContextInspector inspector = new BrowsingContextInspector(driver)) {
      CompletableFuture<NavigationInfo> future = new CompletableFuture<>();

      BrowsingContext context = new BrowsingContext(driver, driver.getWindowHandle());
      context.navigate(appServer.whereIs("/linked_image.html"), ReadinessState.COMPLETE);

      inspector.onFragmentNavigated(future::complete);

      context.navigate(
          appServer.whereIs("/linked_image.html#linkToAnchorOnThisPage"), ReadinessState.COMPLETE);

      NavigationInfo navigationInfo = future.get(5, TimeUnit.SECONDS);
      assertThat(navigationInfo.getBrowsingContextId()).isEqualTo(context.getId());
      assertThat(navigationInfo.getUrl()).contains("linkToAnchorOnThisPage");
    }
  }

  @Test
  @NeedsFreshDriver
  void canListenToUserPromptOpenedEvent()
      throws ExecutionException, InterruptedException, TimeoutException {
    try (BrowsingContextInspector inspector = new BrowsingContextInspector(driver)) {
      CompletableFuture<UserPromptOpened> future = new CompletableFuture<>();

      BrowsingContext context = new BrowsingContext(driver, driver.getWindowHandle());
      inspector.onUserPromptOpened(future::complete);

      driver.get(appServer.whereIs("/alerts.html"));

      driver.findElement(By.id("alert")).click();

      UserPromptOpened userPromptOpened = future.get(5, TimeUnit.SECONDS);
      assertThat(userPromptOpened.getBrowsingContextId()).isEqualTo(context.getId());
      assertThat(userPromptOpened.getType()).isEqualTo(UserPromptType.ALERT);
    }
  }

  @Test
  @NeedsFreshDriver
  // TODO: This test is flaky for comparing the browsing context id for Chrome and Edge. Fix flaky
  // test.
  void canListenToUserPromptClosedEvent()
      throws ExecutionException, InterruptedException, TimeoutException {
    try (BrowsingContextInspector inspector = new BrowsingContextInspector(driver)) {
      CompletableFuture<UserPromptClosed> future = new CompletableFuture<>();

      BrowsingContext context = new BrowsingContext(driver, driver.getWindowHandle());
      inspector.onUserPromptClosed(future::complete);

      driver.get(appServer.whereIs("/alerts.html"));

      driver.findElement(By.id("prompt")).click();

      context.handleUserPrompt(true, "selenium");

      UserPromptClosed userPromptClosed = future.get(5, TimeUnit.SECONDS);
      assertThat(userPromptClosed.getBrowsingContextId()).isEqualTo(context.getId());
      assertThat(userPromptClosed.getUserText().isPresent()).isTrue();
      assertThat(userPromptClosed.getUserText().get()).isEqualTo("selenium");
      assertThat(userPromptClosed.getAccepted()).isTrue();
    }
  }
}
