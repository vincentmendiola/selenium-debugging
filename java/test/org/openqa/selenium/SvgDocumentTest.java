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
import static org.openqa.selenium.testing.drivers.Browser.SAFARI;

import org.junit.jupiter.api.Test;
import org.openqa.selenium.testing.JupiterTestBase;
import org.openqa.selenium.testing.NotYetImplemented;

class SvgDocumentTest extends JupiterTestBase {

  @Test
  @NotYetImplemented(SAFARI)
  public void testClickOnSvgElement() {
    driver.get(pages.svgTestPage);
    WebElement rect = driver.findElement(By.id("rect"));

    assertThat(rect.getAttribute("fill")).isEqualTo("blue");
    rect.click();
    assertThat(rect.getAttribute("fill")).isEqualTo("green");
  }

  @Test
  public void testExecuteScriptInSvgDocument() {
    driver.get(pages.svgTestPage);
    WebElement rect = driver.findElement(By.id("rect"));

    assertThat(rect.getAttribute("fill")).isEqualTo("blue");
    ((JavascriptExecutor) driver)
        .executeScript("document.getElementById('rect').setAttribute('fill', 'yellow');");
    assertThat(rect.getAttribute("fill")).isEqualTo("yellow");
  }
}
