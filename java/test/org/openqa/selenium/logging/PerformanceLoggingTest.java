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

package org.openqa.selenium.logging;

import static org.assertj.core.api.Assertions.assertThat;
import static org.openqa.selenium.testing.drivers.Browser.FIREFOX;
import static org.openqa.selenium.testing.drivers.Browser.IE;
import static org.openqa.selenium.testing.drivers.Browser.SAFARI;

import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.WebDriver;
import org.openqa.selenium.testing.Ignore;
import org.openqa.selenium.testing.JupiterTestBase;

@Ignore(IE)
@Ignore(SAFARI)
@Ignore(FIREFOX)
class PerformanceLoggingTest extends JupiterTestBase {

  private WebDriver loggingDriver;

  @AfterEach
  public void quitDriver() {
    if (loggingDriver != null) {
      loggingDriver.quit();
      loggingDriver = null;
    }
  }

  @Test
  void testDisabledProfilingDoesNotLog() {
    driver.get(pages.simpleTestPage);
    assertThat(getProfilerEntries(driver).getAll())
        .describedAs("Profiler should not log when disabled")
        .isEmpty();
  }

  private LogEntries getProfilerEntries(WebDriver driver) {
    return driver.manage().logs().get(LogType.PROFILER);
  }
}
