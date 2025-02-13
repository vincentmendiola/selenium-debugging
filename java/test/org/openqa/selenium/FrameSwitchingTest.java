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
import static org.assertj.core.api.Assertions.assertThatExceptionOfType;
import static org.openqa.selenium.support.ui.ExpectedConditions.frameToBeAvailableAndSwitchToIt;
import static org.openqa.selenium.support.ui.ExpectedConditions.not;
import static org.openqa.selenium.support.ui.ExpectedConditions.presenceOfElementLocated;
import static org.openqa.selenium.support.ui.ExpectedConditions.textToBe;
import static org.openqa.selenium.support.ui.ExpectedConditions.titleIs;
import static org.openqa.selenium.testing.drivers.Browser.CHROME;
import static org.openqa.selenium.testing.drivers.Browser.EDGE;
import static org.openqa.selenium.testing.drivers.Browser.IE;
import static org.openqa.selenium.testing.drivers.Browser.SAFARI;

import java.util.Random;
import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.Timeout;
import org.openqa.selenium.testing.Ignore;
import org.openqa.selenium.testing.JupiterTestBase;
import org.openqa.selenium.testing.NotYetImplemented;

class FrameSwitchingTest extends JupiterTestBase {

  private Random random;

  @AfterEach
  public void tearDown() {
    try {
      driver.switchTo().defaultContent();
    } catch (Exception e) {
      // May happen if the driver went away.
    }
  }

  // ----------------------------------------------------------------------------------------------
  //
  // Tests that WebDriver doesn't do anything fishy when it navigates to a page with frames.
  //
  // ----------------------------------------------------------------------------------------------
  @Test
  void testShouldAlwaysFocusOnTheTopMostFrameAfterANavigationEvent() {
    driver.get(pages.framesetPage);
    driver.findElement(By.tagName("frameset")); // Test passes if this does not throw.
  }

  @Test
  void testShouldNotAutomaticallySwitchFocusToAnIFrameWhenAPageContainingThemIsLoaded() {
    driver.get(pages.iframePage);
    driver.findElement(By.id("iframe_page_heading"));
  }

  @Test
  @Timeout(10)
  public void testShouldOpenPageWithBrokenFrameset() {
    driver.get(appServer.whereIs("framesetPage3.html"));

    WebElement frame1 = driver.findElement(By.id("first"));
    driver.switchTo().frame(frame1);

    driver.switchTo().defaultContent();

    WebElement frame2 = driver.findElement(By.id("second"));

    try {
      driver.switchTo().frame(frame2);
    } catch (WebDriverException e) {
      // IE9 can not switch to this broken frame - it has no window.
    }
  }

  // ----------------------------------------------------------------------------------------------
  //
  // Tests that WebDriver can switch to frames as expected.
  //
  // ----------------------------------------------------------------------------------------------
  @Test
  void testShouldBeAbleToSwitchToAFrameByItsIndex() {
    driver.get(pages.framesetPage);
    driver.switchTo().frame(1);

    assertThat(driver.findElement(By.id("pageNumber")).getText()).isEqualTo("2");
  }

  @Test
  void testShouldBeAbleToSwitchToAnIframeByItsIndex() {
    driver.get(pages.iframePage);
    driver.switchTo().frame(0);

    assertThat(driver.findElement(By.name("id-name1")).getAttribute("value")).isEqualTo("name");
  }

  @Test
  void testShouldBeAbleToSwitchToAFrameByItsName() {
    driver.get(pages.framesetPage);
    driver.switchTo().frame("fourth");

    assertThat(driver.findElement(By.tagName("frame")).getAttribute("name")).isEqualTo("child1");
  }

  @Test
  void testShouldBeAbleToSwitchToAnIframeByItsName() {
    driver.get(pages.iframePage);
    driver.switchTo().frame("iframe1-name");

    assertThat(driver.findElement(By.name("id-name1")).getAttribute("value")).isEqualTo("name");
  }

  @Test
  void testShouldBeAbleToSwitchToAFrameByItsID() {
    driver.get(pages.framesetPage);
    driver.switchTo().frame("fifth");
    assertThat(driver.findElement(By.name("windowOne")).getText()).isEqualTo("Open new window");
  }

  @Test
  void testShouldBeAbleToSwitchToAnIframeByItsID() {
    driver.get(pages.iframePage);
    driver.switchTo().frame("iframe1");

    assertThat(driver.findElement(By.name("id-name1")).getAttribute("value")).isEqualTo("name");
  }

  @Test
  void testShouldBeAbleToSwitchToFrameWithNameContainingDot() {
    driver.get(pages.framesetPage);
    driver.switchTo().frame("sixth.iframe1");
    assertThat(driver.findElement(By.tagName("body")).getText()).contains("Page number 3");
  }

  @Test
  void testShouldBeAbleToSwitchToAFrameUsingAPreviouslyLocatedWebElement() {
    driver.get(pages.framesetPage);
    WebElement frame = driver.findElement(By.tagName("frame"));
    driver.switchTo().frame(frame);

    assertThat(driver.findElement(By.id("pageNumber")).getText()).isEqualTo("1");
  }

  @Test
  void testShouldBeAbleToSwitchToAnIFrameUsingAPreviouslyLocatedWebElement() {
    driver.get(pages.iframePage);
    WebElement frame = driver.findElement(By.tagName("iframe"));
    driver.switchTo().frame(frame);

    WebElement element = driver.findElement(By.name("id-name1"));
    assertThat(element.getAttribute("value")).isEqualTo("name");
  }

  @Test
  void testShouldEnsureElementIsAFrameBeforeSwitching() {
    driver.get(pages.framesetPage);
    WebElement frame = driver.findElement(By.tagName("frameset"));

    assertThatExceptionOfType(NoSuchFrameException.class)
        .isThrownBy(() -> driver.switchTo().frame(frame));
  }

  @Test
  void testFrameSearchesShouldBeRelativeToTheCurrentlySelectedFrame() {
    driver.get(pages.framesetPage);

    driver.switchTo().frame("second");
    assertThat(driver.findElement(By.id("pageNumber")).getText()).isEqualTo("2");

    assertThatExceptionOfType(NoSuchFrameException.class)
        .isThrownBy(() -> driver.switchTo().frame("third"));

    driver.switchTo().defaultContent();
    driver.switchTo().frame("third");

    assertThatExceptionOfType(NoSuchFrameException.class)
        .isThrownBy(() -> driver.switchTo().frame("second"));

    driver.switchTo().defaultContent();
    driver.switchTo().frame("second");
    assertThat(driver.findElement(By.id("pageNumber")).getText()).isEqualTo("2");
  }

  @Test
  void testShouldSelectChildFramesByChainedCalls() {
    driver.get(pages.framesetPage);

    driver.switchTo().frame("fourth").switchTo().frame("child2");
    assertThat(driver.findElement(By.id("pageNumber")).getText()).isEqualTo("11");
  }

  @Test
  void testShouldThrowFrameNotFoundExceptionLookingUpSubFramesWithSuperFrameNames() {
    driver.get(pages.framesetPage);
    driver.switchTo().frame("fourth");

    assertThatExceptionOfType(NoSuchFrameException.class)
        .isThrownBy(() -> driver.switchTo().frame("second"));
  }

  @Test
  void testShouldThrowAnExceptionWhenAFrameCannotBeFound() {
    driver.get(pages.xhtmlTestPage);

    assertThatExceptionOfType(NoSuchFrameException.class)
        .isThrownBy(() -> driver.switchTo().frame("Nothing here"));
  }

  @Test
  void testShouldThrowAnExceptionWhenAFrameCannotBeFoundByIndex() {
    driver.get(pages.xhtmlTestPage);

    assertThatExceptionOfType(NoSuchFrameException.class)
        .isThrownBy(() -> driver.switchTo().frame(27));
  }

  @Test
  void testShouldBeAbleToSwitchToParentFrame() {
    driver.get(pages.framesetPage);

    driver.switchTo().frame("fourth").switchTo().parentFrame().switchTo().frame("first");
    assertThat(driver.findElement(By.id("pageNumber")).getText()).isEqualTo("1");
  }

  @Test
  @NotYetImplemented(SAFARI)
  public void testShouldBeAbleToSwitchToParentFrameFromASecondLevelFrame() {
    driver.get(pages.framesetPage);

    driver
        .switchTo()
        .frame("fourth")
        .switchTo()
        .frame("child1")
        .switchTo()
        .parentFrame()
        .switchTo()
        .frame("child2");
    assertThat(driver.findElement(By.id("pageNumber")).getText()).isEqualTo("11");
  }

  @Test
  void testSwitchingToParentFrameFromDefaultContextIsNoOp() {
    driver.get(pages.xhtmlTestPage);
    driver.switchTo().parentFrame();
    assertThat(driver.getTitle()).isEqualTo("XHTML Test Page");
  }

  @Test
  void testShouldBeAbleToSwitchToParentFromAnIframe() {
    driver.get(pages.iframePage);
    driver.switchTo().frame(0);

    driver.switchTo().parentFrame();
    driver.findElement(By.id("iframe_page_heading"));
  }

  // ----------------------------------------------------------------------------------------------
  //
  // General frame handling behavior tests
  //
  // ----------------------------------------------------------------------------------------------

  @Test
  void testShouldContinueToReferToTheSameFrameOnceItHasBeenSelected() {
    driver.get(pages.framesetPage);

    driver.switchTo().frame(2);
    WebElement checkbox = driver.findElement(By.xpath("//input[@name='checky']"));
    checkbox.click();
    checkbox.submit();

    wait.until(textToBe(By.xpath("//p"), "Success!"));
  }

  @Test
  @NotYetImplemented(SAFARI)
  public void testShouldFocusOnTheReplacementWhenAFrameFollowsALinkToA_TopTargetedPage() {
    driver.get(pages.framesetPage);

    driver.switchTo().frame(0);
    driver.findElement(By.linkText("top")).click();

    String expectedTitle = "XHTML Test Page";

    wait.until(titleIs(expectedTitle));
  }

  @Test
  void testShouldAllowAUserToSwitchFromAnIframeBackToTheMainContentOfThePage() {
    driver.get(pages.iframePage);
    driver.switchTo().frame(0);

    driver.switchTo().defaultContent();
    driver.findElement(By.id("iframe_page_heading"));
  }

  @Test
  void testShouldAllowTheUserToSwitchToAnIFrameAndRemainFocusedOnIt() {
    driver.get(pages.iframePage);
    driver.switchTo().frame(0);

    driver.findElement(By.id("submitButton")).click();

    assertThat(getTextOfGreetingElement()).isEqualTo("Success!");
  }

  public String getTextOfGreetingElement() {
    return wait.until(presenceOfElementLocated(By.id("greeting"))).getText();
  }

  @Test
  void testShouldBeAbleToClickInAFrame() {
    driver.get(pages.framesetPage);
    driver.switchTo().frame("third");

    // This should replace frame "third" ...
    driver.findElement(By.id("submitButton")).click();
    // driver should still be focused on frame "third" ...
    assertThat(getTextOfGreetingElement()).isEqualTo("Success!");
    // Make sure it was really frame "third" which was replaced ...
    driver.switchTo().defaultContent().switchTo().frame("third");
    assertThat(getTextOfGreetingElement()).isEqualTo("Success!");
  }

  @Test
  void testShouldBeAbleToClickInAFrameThatRewritesTopWindowLocation() {
    driver.get(appServer.whereIs("click_tests/issue5237.html"));
    driver.switchTo().frame("search");
    driver.findElement(By.id("submit")).click();
    driver.switchTo().defaultContent();
    wait.until(titleIs("Target page for issue 5237"));
  }

  @Test
  void testShouldBeAbleToClickInASubFrame() {
    driver.get(pages.framesetPage);
    driver.switchTo().frame("sixth").switchTo().frame("iframe1");

    // This should replace frame "iframe1" inside frame "sixth" ...
    driver.findElement(By.id("submitButton")).click();
    // driver should still be focused on frame "iframe1" inside frame "sixth" ...
    assertThat(getTextOfGreetingElement()).isEqualTo("Success!");
    // Make sure it was really frame "iframe1" inside frame "sixth" which was replaced ...
    driver.switchTo().defaultContent().switchTo().frame("sixth").switchTo().frame("iframe1");
    assertThat(driver.findElement(By.id("greeting")).getText()).isEqualTo("Success!");
  }

  @Test
  void testShouldBeAbleToFindElementsInIframesByXPath() {
    driver.get(pages.iframePage);

    driver.switchTo().frame("iframe1");

    WebElement element = driver.findElement(By.xpath("//*[@id = 'changeme']"));

    assertThat(element).isNotNull();
  }

  @Test
  void testGetCurrentUrlReturnsTopLevelBrowsingContextUrl() {
    driver.get(pages.framesetPage);
    assertThat(driver.getCurrentUrl()).isEqualTo(pages.framesetPage);

    driver.switchTo().frame("second");
    assertThat(driver.getCurrentUrl()).isEqualTo(pages.framesetPage);
  }

  @Test
  void testGetCurrentUrlReturnsTopLevelBrowsingContextUrlForIframes() {
    driver.get(pages.iframePage);
    assertThat(driver.getCurrentUrl()).isEqualTo(pages.iframePage);

    driver.switchTo().frame("iframe1");
    assertThat(driver.getCurrentUrl()).isEqualTo(pages.iframePage);
  }

  @Test
  @Ignore(SAFARI)
  public void testShouldBeAbleToSwitchToTheTopIfTheFrameIsDeletedFromUnderUs() {
    driver.get(appServer.whereIs("frame_switching_tests/deletingFrame.html"));

    driver.switchTo().frame("iframe1");

    WebElement killIframe = driver.findElement(By.id("killIframe"));
    killIframe.click();
    driver.switchTo().defaultContent();

    assertFrameNotPresent("iframe1");

    WebElement addIFrame = driver.findElement(By.id("addBackFrame"));
    addIFrame.click();
    wait.until(presenceOfElementLocated(By.id("iframe1")));

    driver.switchTo().frame("iframe1");

    wait.until(presenceOfElementLocated(By.id("success")));
  }

  @Test
  @Ignore(SAFARI)
  public void testShouldBeAbleToSwitchToTheTopIfTheFrameIsDeletedFromUnderUsWithFrameIndex() {
    driver.get(appServer.whereIs("frame_switching_tests/deletingFrame.html"));
    int iframe = 0;
    wait.until(frameToBeAvailableAndSwitchToIt(iframe));
    // we should be in the frame now
    WebElement killIframe = driver.findElement(By.id("killIframe"));
    killIframe.click();

    driver.switchTo().defaultContent();

    WebElement addIFrame = driver.findElement(By.id("addBackFrame"));
    addIFrame.click();
    wait.until(frameToBeAvailableAndSwitchToIt(iframe));

    wait.until(presenceOfElementLocated(By.id("success")));
  }

  @Test
  @Ignore(SAFARI)
  public void testShouldBeAbleToSwitchToTheTopIfTheFrameIsDeletedFromUnderUsWithWebElement() {
    driver.get(appServer.whereIs("frame_switching_tests/deletingFrame.html"));
    WebElement iframe = driver.findElement(By.id("iframe1"));
    wait.until(frameToBeAvailableAndSwitchToIt(iframe));
    // we should be in the frame now
    WebElement killIframe = driver.findElement(By.id("killIframe"));
    killIframe.click();

    driver.switchTo().defaultContent();

    WebElement addIFrame = driver.findElement(By.id("addBackFrame"));
    addIFrame.click();

    iframe = driver.findElement(By.id("iframe1"));
    wait.until(frameToBeAvailableAndSwitchToIt(iframe));
    wait.until(presenceOfElementLocated(By.id("success")));
  }

  @Test
  @NotYetImplemented(value = CHROME, reason = "Throws NoSuchElementException")
  @NotYetImplemented(value = EDGE, reason = "Throws NoSuchElementException")
  @Ignore(IE)
  @Ignore(SAFARI)
  public void testShouldNotBeAbleToDoAnythingTheFrameIsDeletedFromUnderUs() {
    driver.get(appServer.whereIs("frame_switching_tests/deletingFrame.html"));

    driver.switchTo().frame("iframe1");
    driver.findElement(By.id("killIframe")).click();

    assertThatExceptionOfType(NoSuchWindowException.class)
        .isThrownBy(() -> driver.findElement(By.id("killIframe")));
  }

  @Test
  void testShouldReturnWindowTitleInAFrameset() {
    driver.get(pages.framesetPage);
    driver.switchTo().frame("third");
    assertThat(driver.getTitle()).isEqualTo("Unique title");
  }

  @Test
  void testJavaScriptShouldExecuteInTheContextOfTheCurrentFrame() {
    JavascriptExecutor executor = (JavascriptExecutor) driver;

    driver.get(pages.framesetPage);
    assertThat((Boolean) executor.executeScript("return window == window.top")).isTrue();
    driver.switchTo().frame("third");
    assertThat((Boolean) executor.executeScript("return window != window.top")).isTrue();
  }

  @Test
  void testShouldNotSwitchMagicallyToTheTopWindow() {
    String baseUrl = appServer.whereIs("frame_switching_tests/");
    driver.get(baseUrl + "bug4876.html");
    driver.switchTo().frame(0);
    wait.until(presenceOfElementLocated(By.id("inputText")));

    for (int i = 0; i < 20; i++) {
      try {
        WebElement input = wait.until(presenceOfElementLocated(By.id("inputText")));
        WebElement submit = wait.until(presenceOfElementLocated(By.id("submitButton")));
        input.clear();
        random = new Random();
        input.sendKeys("rand" + random.nextInt());
        submit.click();
      } finally {
        String url =
            (String) ((JavascriptExecutor) driver).executeScript("return window.location.href");
        // IE6 and Chrome add "?"-symbol to the end of the URL
        if (url.endsWith("?")) {
          url = url.substring(0, url.length() - 1);
        }
        assertThat(url).isEqualTo(baseUrl + "bug4876_iframe.html");
      }
    }
  }

  @Test
  void testGetShouldSwitchToDefaultContext() {
    driver.get(pages.iframePage);
    driver.switchTo().frame(driver.findElement(By.id("iframe1")));
    driver.findElement(By.id("cheese")); // Found on formPage.html but not on iframes.html.

    driver.get(pages.iframePage); // This must effectively switchTo().defaultContent(), too.
    driver.findElement(By.id("iframe1"));
  }

  private void assertFrameNotPresent(String locator) {
    driver.switchTo().defaultContent();
    wait.until(not(frameToBeAvailableAndSwitchToIt(locator)));
    driver.switchTo().defaultContent();
  }
}
