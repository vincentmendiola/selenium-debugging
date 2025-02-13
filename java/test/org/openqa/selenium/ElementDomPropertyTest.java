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

import java.util.List;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.support.ui.ExpectedConditions;
import org.openqa.selenium.testing.JupiterTestBase;

class ElementDomPropertyTest extends JupiterTestBase {

  @Test
  void testShouldReturnNullWhenGettingTheValueOfAPropertyThatDoesNotExist() {
    driver.get(pages.simpleTestPage);
    WebElement head = driver.findElement(By.xpath("/html"));
    assertThat(head.getDomProperty("cheese")).isNull();
  }

  @Test
  void testShouldReturnAnAbsoluteUrlWhenGettingSrcAttributeOfAValidImgTag() {
    driver.get(pages.simpleTestPage);
    WebElement img = driver.findElement(By.id("validImgTag"));
    assertThat(img.getDomProperty("src")).isEqualTo(appServer.whereIs("icon.gif"));
  }

  @Test
  void testShouldReturnAnAbsoluteUrlWhenGettingHrefAttributeOfAValidAnchorTag() {
    driver.get(pages.simpleTestPage);
    WebElement img = driver.findElement(By.id("validAnchorTag"));
    assertThat(img.getDomProperty("href")).isEqualTo(appServer.whereIs("icon.gif"));
  }

  @Test
  void testShouldReturnTheValueOfTheIndexAttributeEvenIfItIsMissing() {
    driver.get(pages.formPage);
    WebElement multiSelect = driver.findElement(By.id("multi"));
    List<WebElement> options = multiSelect.findElements(By.tagName("option"));
    assertThat(options.get(1).getDomProperty("index")).isEqualTo("1");
  }

  @Test
  void testShouldReturnTheValueOfCheckedForACheckboxOnlyIfItIsChecked() {
    driver.get(pages.formPage);
    WebElement checkbox = driver.findElement(By.xpath("//input[@id='checky']"));
    assertThat(checkbox.getDomProperty("checked")).isEqualTo("false");
    checkbox.click();
    assertThat(checkbox.getDomProperty("checked")).isEqualTo("true");
  }

  @Test
  void testShouldReturnTheValueOfSelectedForOptionsOnlyIfTheyAreSelected() {
    driver.get(pages.formPage);
    WebElement selectBox = driver.findElement(By.xpath("//select[@name='selectomatic']"));
    List<WebElement> options = selectBox.findElements(By.tagName("option"));
    WebElement one = options.get(0);
    WebElement two = options.get(1);
    assertThat(one.isSelected()).isTrue();
    assertThat(two.isSelected()).isFalse();
    assertThat(one.getDomProperty("selected")).isEqualTo("true");
    assertThat(two.getDomProperty("selected")).isEqualTo("false");
    assertThat(selectBox.getDomProperty("selectedIndex")).isEqualTo("0");
  }

  @Test
  public void testShouldGetClassPropertiesOfAnElement() {
    driver.get(pages.xhtmlTestPage);
    WebElement heading = driver.findElement(By.cssSelector(".nameA"));
    assertThat(heading.getDomProperty("class")).isNull();
    assertThat(heading.getDomProperty("className")).isEqualTo("nameA nameBnoise   nameC");
    assertThat(heading.getDomProperty("classList")).isEqualTo("[nameA, nameBnoise, nameC]");
  }

  @Test
  void testShouldReturnTheContentsOfATextAreaAsItsValue() {
    driver.get(pages.formPage);
    WebElement withText = driver.findElement(By.id("withText"));
    assertThat(withText.getDomProperty("value")).isEqualTo("Example text");
  }

  @Test
  void testShouldReturnInnerHtml() {
    driver.get(pages.simpleTestPage);
    WebElement wrapping = driver.findElement(By.id("wrappingtext"));
    assertThat(wrapping.getDomProperty("innerHTML")).contains("<tbody>");
  }

  @Test
  void testShouldReturnHiddenTextForTextContentProperty() {
    driver.get(pages.simpleTestPage);
    WebElement element = driver.findElement(By.id("hiddenline"));
    assertThat(element.getDomProperty("textContent")).isEqualTo("A hidden line of text");
  }

  @Test
  void testShouldGetNumericProperty() {
    driver.get(pages.formPage);
    WebElement element = driver.findElement(By.id("withText"));
    assertThat(element.getDomProperty("rows")).isEqualTo("5");
  }

  @Test
  public void testCanReturnATextApproximationOfTheStyleProperty() {
    driver.get(pages.javascriptPage);
    WebElement element = driver.findElement(By.id("red-item"));
    assertThat(element.getDomProperty("style").toLowerCase()).contains("background-color");
  }

  @Test
  void testPropertyNamesAreCaseSensitive() {
    driver.get(pages.tables);
    WebElement th1 = driver.findElement(By.id("th1"));
    assertThat(th1.getDomProperty("colspan")).isNull();
    assertThat(th1.getDomProperty("COLSPAN")).isNull();
    assertThat(th1.getDomProperty("colSpan")).isEqualTo("3");
  }

  @Test
  void testCanRetrieveTheCurrentValueOfATextFormField_textInput() {
    driver.get(pages.formPage);
    WebElement element = driver.findElement(By.id("working"));
    assertThat(element.getDomProperty("value")).isEmpty();
    element.sendKeys("hello world");
    shortWait.until(ExpectedConditions.domPropertyToBe(element, "value", "hello world"));
  }

  @Test
  void testCanRetrieveTheCurrentValueOfATextFormField_emailInput() {
    driver.get(pages.formPage);
    WebElement element = driver.findElement(By.id("email"));
    assertThat(element.getDomProperty("value")).isEmpty();
    element.sendKeys("hello@example.com");
    shortWait.until(ExpectedConditions.domPropertyToBe(element, "value", "hello@example.com"));
  }

  @Test
  void testCanRetrieveTheCurrentValueOfATextFormField_textArea() {
    driver.get(pages.formPage);
    WebElement element = driver.findElement(By.id("emptyTextArea"));
    assertThat(element.getDomProperty("value")).isEmpty();
    element.sendKeys("hello world");
    shortWait.until(ExpectedConditions.domPropertyToBe(element, "value", "hello world"));
  }

  @Test
  void testMultiplePropertyShouldBeTrueWhenSelectHasMultipleWithValueAsBlank() {
    driver.get(pages.selectPage);
    WebElement element = driver.findElement(By.id("selectWithEmptyStringMultiple"));
    assertThat(element.getDomProperty("multiple")).isEqualTo("true");
  }

  @Test
  void testMultiplePropertyShouldBeTrueWhenSelectHasMultipleWithoutAValue() {
    driver.get(pages.selectPage);
    WebElement element = driver.findElement(By.id("selectWithMultipleWithoutValue"));
    assertThat(element.getDomProperty("multiple")).isEqualTo("true");
  }

  @Test
  void testMultiplePropertyShouldBeTrueWhenSelectHasMultipleWithValueAsSomethingElse() {
    driver.get(pages.selectPage);
    WebElement element = driver.findElement(By.id("selectWithRandomMultipleValue"));
    assertThat(element.getDomProperty("multiple")).isEqualTo("true");
  }

  @Test
  void testGetValueOfUserDefinedProperty() {
    driver.get(pages.userDefinedProperty);
    WebElement element = driver.findElement(By.id("d"));
    assertThat(element.getDomProperty("dynamicProperty")).isEqualTo("sampleValue");
  }
}
