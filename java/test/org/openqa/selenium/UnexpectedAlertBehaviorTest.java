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

import static org.assertj.core.api.Assertions.assertThatExceptionOfType;
import static org.openqa.selenium.UnexpectedAlertBehaviour.IGNORE;
import static org.openqa.selenium.WaitingConditions.elementTextToEqual;
import static org.openqa.selenium.remote.CapabilityType.UNHANDLED_PROMPT_BEHAVIOUR;
import static org.openqa.selenium.testing.drivers.Browser.CHROME;
import static org.openqa.selenium.testing.drivers.Browser.EDGE;
import static org.openqa.selenium.testing.drivers.Browser.SAFARI;

import java.time.Duration;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.support.ui.Wait;
import org.openqa.selenium.support.ui.WebDriverWait;
import org.openqa.selenium.testing.Ignore;
import org.openqa.selenium.testing.JupiterTestBase;
import org.openqa.selenium.testing.NoDriverBeforeTest;

@Ignore(value = SAFARI, reason = "Does not support alerts yet")
class UnexpectedAlertBehaviorTest extends JupiterTestBase {

  @Test
  @Ignore(value = CHROME, reason = "Legacy behaviour, not W3C conformance")
  @Ignore(value = EDGE, reason = "Legacy behaviour, not W3C conformant")
  @NoDriverBeforeTest
  public void canAcceptUnhandledAlert() {
    runScenarioWithUnhandledAlert(
        UnexpectedAlertBehaviour.ACCEPT_AND_NOTIFY, "This is a default value", false);
  }

  @Test
  @Ignore(value = CHROME, reason = "Legacy behaviour, not W3C conformant")
  @Ignore(value = EDGE, reason = "Legacy behaviour, not W3C conformant")
  @NoDriverBeforeTest
  public void canSilentlyAcceptUnhandledAlert() {
    runScenarioWithUnhandledAlert(UnexpectedAlertBehaviour.ACCEPT, "This is a default value", true);
  }

  @Test
  @Ignore(value = CHROME, reason = "Unstable Chrome behavior")
  @Ignore(value = EDGE, reason = "Unstable Chrome behavior")
  @NoDriverBeforeTest
  public void canDismissUnhandledAlert() {
    runScenarioWithUnhandledAlert(UnexpectedAlertBehaviour.DISMISS_AND_NOTIFY, "null", false);
  }

  @Test
  @Ignore(value = CHROME, reason = "Legacy behaviour, not W3C conformant")
  @Ignore(value = EDGE, reason = "Legacy behaviour, not W3C conformant")
  @NoDriverBeforeTest
  public void canSilentlyDismissUnhandledAlert() {
    runScenarioWithUnhandledAlert(UnexpectedAlertBehaviour.DISMISS, "null", true);
  }

  @Test
  @Ignore(value = CHROME, reason = "Chrome uses IGNORE mode by default")
  @Ignore(value = EDGE, reason = "Edge uses IGNORE mode by default")
  @NoDriverBeforeTest
  public void canDismissUnhandledAlertsByDefault() {
    runScenarioWithUnhandledAlert(null, "null", false);
  }

  @Test
  @Ignore(value = CHROME, reason = "Unstable Chrome behavior")
  @Ignore(value = EDGE, reason = "Unstable Chrome behavior")
  @NoDriverBeforeTest
  public void canIgnoreUnhandledAlert() {
    assertThatExceptionOfType(UnhandledAlertException.class)
        .isThrownBy(() -> runScenarioWithUnhandledAlert(IGNORE, "Text ignored", true));
    driver.switchTo().alert().dismiss();
  }

  private void runScenarioWithUnhandledAlert(
      UnexpectedAlertBehaviour behaviour, String expectedAlertText, boolean silently) {
    Capabilities caps =
        behaviour == null
            ? new ImmutableCapabilities()
            : new ImmutableCapabilities(UNHANDLED_PROMPT_BEHAVIOUR, behaviour);
    createNewDriver(caps);

    driver.get(pages.alertsPage);
    driver.findElement(By.id("prompt-with-default")).click();

    Wait<WebDriver> wait1 =
        silently
            ? wait
            : new WebDriverWait(driver, Duration.ofSeconds(10))
                .ignoring(UnhandledAlertException.class);
    wait1.until(elementTextToEqual(By.id("text"), expectedAlertText));
  }
}
