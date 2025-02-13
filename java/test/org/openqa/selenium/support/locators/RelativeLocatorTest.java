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

package org.openqa.selenium.support.locators;

import static org.assertj.core.api.Assertions.assertThat;
import static org.assertj.core.api.Assertions.assertThatExceptionOfType;
import static org.openqa.selenium.By.cssSelector;
import static org.openqa.selenium.By.tagName;
import static org.openqa.selenium.By.xpath;
import static org.openqa.selenium.support.locators.RelativeLocator.with;

import java.util.List;
import java.util.stream.Collectors;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.By;
import org.openqa.selenium.NoSuchElementException;
import org.openqa.selenium.WebElement;
import org.openqa.selenium.environment.webserver.Page;
import org.openqa.selenium.testing.JupiterTestBase;

class RelativeLocatorTest extends JupiterTestBase {

  @Test
  void shouldBeAbleToFindElementsAboveAnotherWithTagName() {
    driver.get(appServer.whereIs("relative_locators.html"));

    WebElement lowest = driver.findElement(By.id("below"));

    List<WebElement> elements = driver.findElements(with(tagName("p")).above(lowest));
    List<String> ids =
        elements.stream().map(e -> e.getAttribute("id")).collect(Collectors.toList());

    assertThat(ids).containsExactly("mid", "above");
  }

  @Test
  void shouldBeAbleToFindElementsAboveAnotherWithXpath() {
    driver.get(appServer.whereIs("relative_locators.html"));

    WebElement lowest = driver.findElement(By.id("bottomLeft"));

    List<WebElement> seen = driver.findElements(with(xpath("//td[1]")).above(lowest));

    List<String> ids = seen.stream().map(e -> e.getAttribute("id")).collect(Collectors.toList());

    assertThat(ids).containsExactly("left", "topLeft");
  }

  @Test
  void shouldBeAbleToFindElementsAboveAnotherWithCssSelector() {
    driver.get(appServer.whereIs("relative_locators.html"));

    WebElement lowest = driver.findElement(By.id("below"));

    List<WebElement> elements = driver.findElements(with(cssSelector("p")).above(lowest));
    List<String> ids =
        elements.stream().map(e -> e.getAttribute("id")).collect(Collectors.toList());

    assertThat(ids).containsExactly("mid", "above");
  }

  @Test
  void shouldBeAbleToFindElementsBelowAnother() {
    driver.get(appServer.whereIs("relative_locators.html"));

    WebElement midpoint = driver.findElement(By.id("mid"));

    List<WebElement> elements = driver.findElements(with(tagName("p")).below(midpoint));
    List<String> ids =
        elements.stream().map(e -> e.getAttribute("id")).collect(Collectors.toList());
    assertThat(ids).containsExactly("below");
  }

  @Test
  void shouldFindElementsAboveAnother() {
    driver.get(appServer.whereIs("relative_locators.html"));

    WebElement midpoint = driver.findElement(By.id("center"));

    List<WebElement> elements = driver.findElements(with(tagName("td")).above(midpoint));
    List<String> ids =
        elements.stream().map(e -> e.getAttribute("id")).collect(Collectors.toList());

    assertThat(ids).containsExactly("top", "topLeft", "topRight");
  }

  @Test
  void shouldFindElementsBelowAnother() {
    driver.get(appServer.whereIs("relative_locators.html"));

    WebElement midpoint = driver.findElement(By.id("center"));

    List<WebElement> elements = driver.findElements(with(tagName("td")).below(midpoint));
    List<String> ids =
        elements.stream().map(e -> e.getAttribute("id")).collect(Collectors.toList());

    assertThat(ids).containsExactly("bottom", "bottomLeft", "bottomRight");
  }

  @Test
  void shouldFindElementsLeftOfAnother() {
    driver.get(appServer.whereIs("relative_locators.html"));

    WebElement midpoint = driver.findElement(By.id("center"));

    List<WebElement> elements = driver.findElements(with(tagName("td")).toLeftOf(midpoint));
    List<String> ids =
        elements.stream().map(e -> e.getAttribute("id")).collect(Collectors.toList());

    assertThat(ids).containsExactly("left", "topLeft", "bottomLeft");
  }

  @Test
  void shouldFindElementsRightOfAnother() {
    driver.get(appServer.whereIs("relative_locators.html"));

    WebElement midpoint = driver.findElement(By.id("center"));

    List<WebElement> elements = driver.findElements(with(tagName("td")).toRightOf(midpoint));
    List<String> ids =
        elements.stream().map(e -> e.getAttribute("id")).collect(Collectors.toList());

    assertThat(ids).containsExactly("right", "topRight", "bottomRight");
  }

  @Test
  void shouldBeAbleToFindElementsStraightAboveAnother() {
    driver.get(appServer.whereIs("relative_locators.html"));

    WebElement bottom = driver.findElement(By.id("bottom"));

    List<WebElement> elements = driver.findElements(with(tagName("td")).straightAbove(bottom));
    List<String> ids =
        elements.stream().map(e -> e.getAttribute("id")).collect(Collectors.toList());

    assertThat(ids).containsExactly("center", "top");
  }

  @Test
  void shouldBeAbleToFindElementsStraightBelowAnother() {
    driver.get(appServer.whereIs("relative_locators.html"));

    WebElement top = driver.findElement(By.id("top"));

    List<WebElement> elements = driver.findElements(with(tagName("td")).straightBelow(top));
    List<String> ids =
        elements.stream().map(e -> e.getAttribute("id")).collect(Collectors.toList());

    assertThat(ids).containsExactly("center", "bottom");
  }

  @Test
  void shouldBeAbleToFindElementsStraightLeftOfAnother() {
    driver.get(appServer.whereIs("relative_locators.html"));

    WebElement right = driver.findElement(By.id("right"));

    List<WebElement> elements = driver.findElements(with(tagName("td")).straightLeftOf(right));
    List<String> ids =
        elements.stream().map(e -> e.getAttribute("id")).collect(Collectors.toList());

    assertThat(ids).containsExactly("center", "left");
  }

  @Test
  void shouldBeAbleToFindElementsStraightRightOfAnother() {
    driver.get(appServer.whereIs("relative_locators.html"));

    WebElement left = driver.findElement(By.id("left"));

    List<WebElement> elements = driver.findElements(with(tagName("td")).straightRightOf(left));
    List<String> ids =
        elements.stream().map(e -> e.getAttribute("id")).collect(Collectors.toList());

    assertThat(ids).containsExactly("center", "right");
  }

  @Test
  void shouldBeAbleToCombineFilters() {
    driver.get(appServer.whereIs("relative_locators.html"));

    List<WebElement> seen =
        driver.findElements(with(tagName("td")).above(By.id("center")).toRightOf(By.id("top")));

    List<String> ids = seen.stream().map(e -> e.getAttribute("id")).collect(Collectors.toList());

    assertThat(ids).containsExactly("topRight");
  }

  @Test
  void shouldBeAbleToCombineFiltersWithXpath() {
    driver.get(appServer.whereIs("relative_locators.html"));

    List<WebElement> seen =
        driver.findElements(with(xpath("//td[1]")).below(By.id("top")).above(By.id("bottomLeft")));

    List<String> ids = seen.stream().map(e -> e.getAttribute("id")).collect(Collectors.toList());

    assertThat(ids).containsExactly("left");
  }

  @Test
  void shouldBeAbleToCombineFiltersWithCssSelector() {
    driver.get(appServer.whereIs("relative_locators.html"));

    List<WebElement> seen =
        driver.findElements(with(cssSelector("td")).above(By.id("center")).toRightOf(By.id("top")));

    List<String> ids = seen.stream().map(e -> e.getAttribute("id")).collect(Collectors.toList());

    assertThat(ids).containsExactly("topRight");
  }

  @Test
  void exerciseNearLocatorWithTagName() {
    driver.get(appServer.whereIs("relative_locators.html"));

    List<WebElement> seen = driver.findElements(with(tagName("td")).near(By.id("center")));

    // Elements are sorted by proximity and then DOM insertion order.
    // Proximity is determined using distance from center points, so
    // we expect the order to be:
    // 1. Directly above (short vertical distance, first in DOM)
    // 2. Directly below (short vertical distance, later in DOM)
    // 3. Directly left (slight longer distance horizontally, first in DOM)
    // 4. Directly right (slight longer distance horizontally, later in DOM)
    // 5-8. Diagonally close (pythagoras sorting, with top row first
    //    because of DOM insertion order)
    List<String> ids = seen.stream().map(e -> e.getAttribute("id")).collect(Collectors.toList());

    assertThat(ids)
        .containsExactly(
            "top", "bottom", "left", "right", "topLeft", "topRight", "bottomLeft", "bottomRight");

    assertThat(ids)
        .containsExactly(
            "top", "bottom", "left", "right", "topLeft", "topRight", "bottomLeft", "bottomRight");
  }

  @Test
  void shouldBeAbleToCombineStraightFilters() {
    driver.get(appServer.whereIs("relative_locators.html"));

    List<WebElement> seen =
        driver.findElements(
            with(tagName("td"))
                .straightBelow(By.id("topRight"))
                .straightRightOf(By.id("bottomLeft")));

    List<String> ids = seen.stream().map(e -> e.getAttribute("id")).collect(Collectors.toList());

    assertThat(ids).containsExactly("bottomRight");
  }

  @Test
  void exerciseNearLocatorWithXpath() {
    driver.get(appServer.whereIs("relative_locators.html"));

    List<WebElement> seen = driver.findElements(with(xpath("//td")).near(By.id("center")));

    // Elements are sorted by proximity and then DOM insertion order.
    // Proximity is determined using distance from center points, so
    // we expect the order to be:
    // 1. Directly above (short vertical distance, first in DOM)
    // 2. Directly below (short vertical distance, later in DOM)
    // 3. Directly left (slight longer distance horizontally, first in DOM)
    // 4. Directly right (slight longer distance horizontally, later in DOM)
    // 5-8. Diagonally close (pythagoras sorting, with top row first
    //    because of DOM insertion order)
    List<String> ids = seen.stream().map(e -> e.getAttribute("id")).collect(Collectors.toList());

    assertThat(ids)
        .containsExactly(
            "top", "bottom", "left", "right", "topLeft", "topRight", "bottomLeft", "bottomRight");
  }

  @Test
  void exerciseNearLocatorWithCssSelector() {
    driver.get(appServer.whereIs("relative_locators.html"));

    List<WebElement> seen = driver.findElements(with(cssSelector("td")).near(By.id("center")));

    // Elements are sorted by proximity and then DOM insertion order.
    // Proximity is determined using distance from center points, so
    // we expect the order to be:
    // 1. Directly above (short vertical distance, first in DOM)
    // 2. Directly below (short vertical distance, later in DOM)
    // 3. Directly left (slight longer distance horizontally, first in DOM)
    // 4. Directly right (slight longer distance horizontally, later in DOM)
    // 5-8. Diagonally close (pythagoras sorting, with top row first
    //    because of DOM insertion order)
    List<String> ids = seen.stream().map(e -> e.getAttribute("id")).collect(Collectors.toList());

    assertThat(ids)
        .containsExactly(
            "top", "bottom", "left", "right", "topLeft", "topRight", "bottomLeft", "bottomRight");
  }

  @Test
  void ensureNoRepeatedElements() {
    String url =
        appServer.create(
            new Page()
                .withTitle("Repeated Elements")
                .withStyles(
                    " .c {\n"
                        + "    \tposition: absolute;\n"
                        + "    \tborder: 1px solid black;\n"
                        + "    \theight: 50px;\n"
                        + "    \twidth: 50px;\n"
                        + "    }")
                .withBody(
                    "<span style=\"position: relative;\">\n"
                        + "    <div id= \"a\" class=\"c\" style=\"left:25;top:0;\">El-A</div>\n"
                        + "    <div id= \"b\" class=\"c\" style=\"left:78;top:30;\">El-B</div>\n"
                        + "    <div id= \"c\" class=\"c\" style=\"left:131;top:60;\">El-C</div>\n"
                        + "    <div id= \"d\" class=\"c\" style=\"left:0;top:53;\">El-D</div>\n"
                        + "    <div id= \"e\" class=\"c\" style=\"left:53;top:83;\">El-E</div>\n"
                        + "    <div id= \"f\" class=\"c\" style=\"left:106;top:113;\">El-F</div>\n"
                        + "  </span>"));
    driver.get(url);

    WebElement base = driver.findElement(By.id("e"));
    List<WebElement> cells = driver.findElements(with(tagName("div")).above(base));

    WebElement a = driver.findElement(By.id("a"));
    WebElement b = driver.findElement(By.id("b"));

    assertThat(cells)
        .describedAs(
            cells.stream().map(e -> e.getAttribute("id")).collect(Collectors.joining(", ")))
        .isEqualTo(List.of(b, a));
  }

  @Test
  void nearLocatorShouldFindNearElements() {
    driver.get(appServer.whereIs("relative_locators.html"));

    WebElement rect1 = driver.findElement(By.id("rect1"));
    WebElement rect2 = driver.findElement(with(By.id("rect2")).near(rect1));

    assertThat(rect2.getAttribute("id")).isEqualTo("rect2");
  }

  @Test
  void nearLocatorShouldNotFindFarElements() {
    driver.get(appServer.whereIs("relative_locators.html"));

    WebElement rect = driver.findElement(By.id("rect1"));

    assertThatExceptionOfType(NoSuchElementException.class)
        .isThrownBy(() -> driver.findElement(with(By.id("rect4")).near(rect)))
        .withMessageContaining("Cannot locate an element using");
  }
}
