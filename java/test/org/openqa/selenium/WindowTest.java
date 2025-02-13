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

package org.openqa.selenium;

import static org.assertj.core.api.Assertions.assertThat;
import static org.junit.jupiter.api.Assumptions.assumeFalse;
import static org.openqa.selenium.Platform.ANDROID;
import static org.openqa.selenium.testing.drivers.Browser.CHROME;
import static org.openqa.selenium.testing.drivers.Browser.FIREFOX;
import static org.openqa.selenium.testing.drivers.Browser.SAFARI;

import java.util.function.Consumer;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.support.ui.ExpectedCondition;
import org.openqa.selenium.testing.Ignore;
import org.openqa.selenium.testing.JupiterTestBase;
import org.openqa.selenium.testing.SwitchToTopAfterTest;
import org.openqa.selenium.testing.TestUtilities;

class WindowTest extends JupiterTestBase {

  @Test
  void testGetsTheSizeOfTheCurrentWindow() {
    Dimension size = driver.manage().window().getSize();

    assertThat(size.width).isPositive();
    assertThat(size.height).isPositive();
  }

  @Test
  void testSetsTheSizeOfTheCurrentWindow() {
    // Browser window cannot be resized or moved on ANDROID (and most mobile platforms
    // though others aren't defined in org.openqa.selenium.Platform).
    assumeFalse(TestUtilities.getEffectivePlatform(driver).is(ANDROID));
    // resize relative to the initial size, since we don't know what it is
    changeSizeBy(-20, -20);
  }

  @SwitchToTopAfterTest
  @Test
  void testSetsTheSizeOfTheCurrentWindowFromFrame() {
    // Browser window cannot be resized or moved on ANDROID (and most mobile platforms
    // though others aren't defined in org.openqa.selenium.Platform).
    assumeFalse(TestUtilities.getEffectivePlatform(driver).is(ANDROID));
    driver.get(pages.framesetPage);
    driver.switchTo().frame("fourth");
    // resize relative to the initial size, since we don't know what it is
    changeSizeBy(-20, -20);
  }

  @SwitchToTopAfterTest
  @Test
  void testSetsTheSizeOfTheCurrentWindowFromIframe() {
    // Browser window cannot be resized or moved on ANDROID (and most mobile platforms
    // though others aren't defined in org.openqa.selenium.Platform).
    assumeFalse(TestUtilities.getEffectivePlatform(driver).is(ANDROID));
    driver.get(pages.iframePage);
    driver.switchTo().frame("iframe1-name");
    // resize relative to the initial size, since we don't know what it is
    changeSizeBy(-20, -20);
  }

  @Test
  void testGetsThePositionOfTheCurrentWindow() {
    // Window position is undefined on ANDROID (and most mobile platforms
    // though others aren't defined in org.openqa.selenium.Platform).
    assumeFalse(TestUtilities.getEffectivePlatform(driver).is(ANDROID));
    Point position = driver.manage().window().getPosition();

    // If the Chrome under test is launched by default as maximized, the window
    // coordinates may have small negative values (note that elements in the
    // viewport are, of course, still clickable).
    assertThat(position.x).isGreaterThanOrEqualTo(-10);
    assertThat(position.y).isGreaterThanOrEqualTo(-10);
  }

  @Test
  void testSetsThePositionOfTheCurrentWindow() {
    // Browser window cannot be resized or moved on ANDROID (and most mobile platforms
    // though others aren't defined in org.openqa.selenium.Platform).
    assumeFalse(TestUtilities.getEffectivePlatform(driver).is(ANDROID));
    WebDriver.Window window = driver.manage().window();
    Point position = window.getPosition();
    Dimension originalSize = window.getSize();

    try {
      // Some Linux window managers start taking liberties wrt window positions when moving the
      // window
      // off-screen. Therefore, try to stay on-screen. Hopefully you have more than 210 px,
      // or this may fail.
      window.setSize(new Dimension(200, 200));
      Point targetPosition = new Point(position.x + 10, position.y + 10);
      window.setPosition(targetPosition);

      wait.until($ -> window.getPosition().x == targetPosition.x);
      wait.until($ -> window.getPosition().y == targetPosition.y);
    } finally {
      window.setSize(originalSize);
    }
  }

  @Test
  @Ignore(value = CHROME, travis = true)
  @Ignore(value = FIREFOX, gitHubActions = true)
  public void testCanMaximizeTheWindow() {
    // Browser window cannot be resized or moved on ANDROID (and most mobile platforms
    // though others aren't defined in org.openqa.selenium.Platform).
    assumeFalse(TestUtilities.getEffectivePlatform(driver).is(ANDROID));

    changeSizeTo(new Dimension(640, 323));
    enlargeBy(WebDriver.Window::maximize);
  }

  @SwitchToTopAfterTest
  @Test
  @Ignore(value = CHROME, gitHubActions = true)
  @Ignore(value = FIREFOX, gitHubActions = true)
  public void testCanMaximizeTheWindowFromFrame() {
    // Browser window cannot be resized or moved on ANDROID (and most mobile platforms
    // though others aren't defined in org.openqa.selenium.Platform).
    assumeFalse(TestUtilities.getEffectivePlatform(driver).is(ANDROID));

    driver.get(pages.framesetPage);
    changeSizeTo(new Dimension(640, 324));

    driver.switchTo().frame("fourth");
    enlargeBy(WebDriver.Window::maximize);
  }

  @SwitchToTopAfterTest
  @Test
  @Ignore(value = CHROME, gitHubActions = true)
  @Ignore(value = FIREFOX, gitHubActions = true)
  public void testCanMaximizeTheWindowFromIframe() {
    // Browser window cannot be resized or moved on ANDROID (and most mobile platforms
    // though others aren't defined in org.openqa.selenium.Platform).
    assumeFalse(TestUtilities.getEffectivePlatform(driver).is(ANDROID));

    driver.get(pages.iframePage);
    changeSizeTo(new Dimension(640, 400));

    driver.switchTo().frame("iframe1-name");
    enlargeBy(WebDriver.Window::maximize);
  }

  @Test
  @Ignore(travis = true, gitHubActions = true)
  public void canMinimizeTheWindow() {
    // Browser window cannot be resized or moved on ANDROID (and most mobile platforms
    // though others aren't defined in org.openqa.selenium.Platform).
    assumeFalse(TestUtilities.getEffectivePlatform(driver).is(ANDROID));

    changeSizeTo(new Dimension(640, 400));
    driver.manage().window().minimize();

    assertThat(((JavascriptExecutor) driver).executeScript("return document.hidden;"))
        .isEqualTo(true);
  }

  @Test
  @Ignore(value = CHROME, travis = true)
  @Ignore(value = FIREFOX, gitHubActions = true)
  @Ignore(SAFARI)
  public void canFullscreenTheWindow() {
    // Browser window cannot be resized or moved on ANDROID (and most mobile platforms
    // though others aren't defined in org.openqa.selenium.Platform).
    assumeFalse(TestUtilities.getEffectivePlatform(driver).is(ANDROID));

    try {
      changeSizeTo(new Dimension(640, 400));
      enlargeBy(WebDriver.Window::fullscreen);
    } finally {
      driver.manage().window().setSize(new Dimension(640, 323));
    }
  }

  @SwitchToTopAfterTest
  @Test
  @Ignore(value = CHROME, travis = true)
  @Ignore(value = FIREFOX, gitHubActions = true)
  @Ignore(SAFARI)
  public void canFullscreenTheWindowFromFrame() {
    // Browser window cannot be resized or moved on ANDROID (and most mobile platforms
    // though others aren't defined in org.openqa.selenium.Platform).
    assumeFalse(TestUtilities.getEffectivePlatform(driver).is(ANDROID));

    driver.get(pages.framesetPage);
    changeSizeTo(new Dimension(640, 400));

    driver.switchTo().frame("fourth");
    enlargeBy(WebDriver.Window::fullscreen);
  }

  @SwitchToTopAfterTest
  @Test
  @Ignore(value = CHROME, travis = true)
  @Ignore(value = FIREFOX, gitHubActions = true)
  @Ignore(SAFARI)
  public void canFullscreenTheWindowFromIframe() {
    // Browser window cannot be resized or moved on ANDROID (and most mobile platforms
    // though others aren't defined in org.openqa.selenium.Platform).
    assumeFalse(TestUtilities.getEffectivePlatform(driver).is(ANDROID));

    driver.get(pages.iframePage);
    changeSizeTo(new Dimension(640, 400));

    driver.switchTo().frame("iframe1-name");
    enlargeBy(WebDriver.Window::fullscreen);
  }

  private void changeSizeBy(int deltaX, int deltaY) {
    WebDriver.Window window = driver.manage().window();
    Dimension size = window.getSize();
    changeSizeTo(new Dimension(size.width + deltaX, size.height + deltaY));
  }

  private void changeSizeTo(Dimension targetSize) {
    WebDriver.Window window = driver.manage().window();

    window.setSize(targetSize);

    wait.until(windowSizeEqual(targetSize));
  }

  private void enlargeBy(Consumer<WebDriver.Window> operation) {
    WebDriver.Window window = driver.manage().window();
    Dimension size = window.getSize();
    operation.accept(window);
    wait.until($ -> window.getSize().width > size.width);
    wait.until($ -> window.getSize().height > size.height);
  }

  private ExpectedCondition<Boolean> windowSizeEqual(final Dimension size) {
    return driver -> {
      Dimension newSize = driver.manage().window().getSize();
      return newSize.height == size.height && newSize.width == size.width;
    };
  }
}
