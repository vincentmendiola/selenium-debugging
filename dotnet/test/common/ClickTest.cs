// <copyright file="ClickTest.cs" company="Selenium Committers">
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
// </copyright>

using NUnit.Framework;
using OpenQA.Selenium.Environment;
using System;

namespace OpenQA.Selenium
{
    [TestFixture]
    public class ClickTest : DriverTestFixture
    {
        [SetUp]
        public void SetupMethod()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("clicks.html");
        }

        [TearDown]
        public void TearDownMethod()
        {
            driver.SwitchTo().DefaultContent();
        }

        [Test]
        public void CanClickOnALinkAndFollowIt()
        {
            driver.FindElement(By.Id("normal")).Click();
            WaitFor(() => { return driver.Title == "XHTML Test Page"; }, "Browser title was not 'XHTML Test Page'");
            Assert.That(driver.Title, Is.EqualTo("XHTML Test Page"));
        }

        [Test]
        public void CanClickOnALinkThatOverflowsAndFollowIt()
        {
            driver.FindElement(By.Id("overflowLink")).Click();

            WaitFor(() => { return driver.Title == "XHTML Test Page"; }, "Browser title was not 'XHTML Test Page'");
        }

        [Test]
        public void CanClickOnAnAnchorAndNotReloadThePage()
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("document.latch = true");

            driver.FindElement(By.Id("anchor")).Click();

            bool samePage = (bool)((IJavaScriptExecutor)driver).ExecuteScript("return document.latch");

            Assert.That(samePage, Is.True, "Latch was reset");
        }

        [Test]
        public void CanClickOnALinkThatUpdatesAnotherFrame()
        {
            driver.SwitchTo().Frame("source");

            driver.FindElement(By.Id("otherframe")).Click();
            driver.SwitchTo().DefaultContent().SwitchTo().Frame("target");

            Assert.That(driver.PageSource, Does.Contain("Hello WebDriver"));
        }

        [Test]
        public void ElementsFoundByJsCanLoadUpdatesInAnotherFrame()
        {
            driver.SwitchTo().Frame("source");

            IWebElement toClick = (IWebElement)((IJavaScriptExecutor)driver).ExecuteScript("return document.getElementById('otherframe');");
            toClick.Click();
            driver.SwitchTo().DefaultContent().SwitchTo().Frame("target");

            Assert.That(driver.PageSource, Does.Contain("Hello WebDriver"));
        }

        [Test]
        public void JsLocatedElementsCanUpdateFramesIfFoundSomehowElse()
        {
            driver.SwitchTo().Frame("source");

            // Prime the cache of elements
            driver.FindElement(By.Id("otherframe"));

            // This _should_ return the same element
            IWebElement toClick = (IWebElement)((IJavaScriptExecutor)driver).ExecuteScript("return document.getElementById('otherframe');");
            toClick.Click();
            driver.SwitchTo().DefaultContent().SwitchTo().Frame("target");

            Assert.That(driver.PageSource, Does.Contain("Hello WebDriver"));
        }

        [Test]

        public void CanClickOnAnElementWithTopSetToANegativeNumber()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("styledPage.html");
            IWebElement searchBox = driver.FindElement(By.Name("searchBox"));
            searchBox.SendKeys("Cheese");
            driver.FindElement(By.Name("btn")).Click();

            string log = driver.FindElement(By.Id("log")).Text;
            Assert.That(log, Is.EqualTo("click"));
        }

        [Test]
        public void ShouldSetRelatedTargetForMouseOver()
        {
            driver.Url = javascriptPage;

            driver.FindElement(By.Id("movable")).Click();

            string log = driver.FindElement(By.Id("result")).Text;

            // Note: It is not guaranteed that the relatedTarget property of the mouseover
            // event will be the parent, when using native events. Only check that the mouse
            // has moved to this element, not that the parent element was the related target.
            if (this.IsNativeEventsEnabled)
            {
                Assert.That(log, Does.StartWith("parent matches?"));
            }
            else
            {
                Assert.That(log, Is.EqualTo("parent matches? true"));
            }
        }

        [Test]
        public void ShouldClickOnFirstBoundingClientRectWithNonZeroSize()
        {
            driver.FindElement(By.Id("twoClientRects")).Click();
            WaitFor(() => { return driver.Title == "XHTML Test Page"; }, "Browser title was not 'XHTML Test Page'");
            Assert.That(driver.Title, Is.EqualTo("XHTML Test Page"));
        }

        [Test]
        [IgnoreBrowser(Browser.IE, "Edge in IE Mode does not properly handle multiple windows")]
        [NeedsFreshDriver(IsCreatedAfterTest = true)]
        public void ShouldOnlyFollowHrefOnce()
        {
            driver.Url = clicksPage;
            int windowHandlesBefore = driver.WindowHandles.Count;

            driver.FindElement(By.Id("new-window")).Click();
            WaitFor(() => { return driver.WindowHandles.Count >= windowHandlesBefore + 1; }, "Window handles was not " + (windowHandlesBefore + 1).ToString());
            Assert.That(driver.WindowHandles, Has.Exactly(windowHandlesBefore + 1).Items);
        }

        [Test]
        public void ClickingLabelShouldSetCheckbox()
        {
            driver.Url = formsPage;

            driver.FindElement(By.Id("label-for-checkbox-with-label")).Click();

            Assert.That(driver.FindElement(By.Id("checkbox-with-label")).Selected, "Checkbox should be selected");
        }

        [Test]
        public void CanClickOnALinkWithEnclosedImage()
        {
            driver.FindElement(By.Id("link-with-enclosed-image")).Click();
            WaitFor(() => { return driver.Title == "XHTML Test Page"; }, "Browser title was not 'XHTML Test Page'");
            Assert.That(driver.Title, Is.EqualTo("XHTML Test Page"));
        }

        [Test]
        public void CanClickOnAnImageEnclosedInALink()
        {
            driver.FindElement(By.Id("link-with-enclosed-image")).FindElement(By.TagName("img")).Click();
            WaitFor(() => { return driver.Title == "XHTML Test Page"; }, "Browser title was not 'XHTML Test Page'");
            Assert.That(driver.Title, Is.EqualTo("XHTML Test Page"));
        }

        [Test]
        public void CanClickOnALinkThatContainsTextWrappedInASpan()
        {
            driver.FindElement(By.Id("link-with-enclosed-span")).Click();
            WaitFor(() => { return driver.Title == "XHTML Test Page"; }, "Browser title was not 'XHTML Test Page'");
            Assert.That(driver.Title, Is.EqualTo("XHTML Test Page"));
        }

        [Test]
        [IgnoreBrowser(Browser.Firefox, "https://github.com/mozilla/geckodriver/issues/653")]
        public void CanClickOnALinkThatContainsEmbeddedBlockElements()
        {
            driver.FindElement(By.Id("embeddedBlock")).Click();
            WaitFor(() => { return driver.Title == "XHTML Test Page"; }, "Browser title was not 'XHTML Test Page'");
            Assert.That(driver.Title, Is.EqualTo("XHTML Test Page"));
        }

        [Test]
        public void CanClickOnAnElementEnclosedInALink()
        {
            driver.FindElement(By.Id("link-with-enclosed-span")).FindElement(By.TagName("span")).Click();
            WaitFor(() => { return driver.Title == "XHTML Test Page"; }, "Browser title was not 'XHTML Test Page'");
            Assert.That(driver.Title, Is.EqualTo("XHTML Test Page"));
        }

        [Test]
        public void ShouldBeAbleToClickOnAnElementInTheViewport()
        {
            string url = EnvironmentManager.Instance.UrlBuilder.WhereIs("click_out_of_bounds.html");

            driver.Url = url;
            IWebElement button = driver.FindElement(By.Id("button"));
            button.Click();
        }

        [Test]
        public void ClicksASurroundingStrongTag()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("ClickTest_testClicksASurroundingStrongTag.html");
            driver.FindElement(By.TagName("a")).Click();
            WaitFor(() => { return driver.Title == "XHTML Test Page"; }, "Browser title was not 'XHTML Test Page'");
        }

        [Test]
        [IgnoreBrowser(Browser.Firefox, "https://bugzilla.mozilla.org/show_bug.cgi?id=1502636")]
        public void CanClickAnImageMapArea()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("click_tests/google_map.html");
            driver.FindElement(By.Id("rectG")).Click();
            WaitFor(() => { return driver.Title == "Target Page 1"; }, "Browser title was not 'Target Page 1'");

            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("click_tests/google_map.html");
            driver.FindElement(By.Id("circleO")).Click();
            WaitFor(() => { return driver.Title == "Target Page 2"; }, "Browser title was not 'Target Page 2'");

            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("click_tests/google_map.html");
            driver.FindElement(By.Id("polyLE")).Click();
            WaitFor(() => { return driver.Title == "Target Page 3"; }, "Browser title was not 'Target Page 3'");
        }

        [Test]
        [IgnoreBrowser(Browser.Firefox, "https://bugzilla.mozilla.org/show_bug.cgi?id=1422272")]
        public void ShouldBeAbleToClickOnAnElementGreaterThanTwoViewports()
        {
            string url = EnvironmentManager.Instance.UrlBuilder.WhereIs("click_too_big.html");
            driver.Url = url;

            IWebElement element = driver.FindElement(By.Id("click"));

            element.Click();

            WaitFor(() => { return driver.Title == "clicks"; }, "Browser title was not 'clicks'");
        }

        [Test]
        [IgnoreBrowser(Browser.Firefox, "https://bugzilla.mozilla.org/show_bug.cgi?id=1937115")]
        public void ShouldBeAbleToClickOnAnElementInFrameGreaterThanTwoViewports()
        {
            string url = EnvironmentManager.Instance.UrlBuilder.WhereIs("click_too_big_in_frame.html");
            driver.Url = url;

            IWebElement frame = driver.FindElement(By.Id("iframe1"));
            driver.SwitchTo().Frame(frame);

            IWebElement element = driver.FindElement(By.Id("click"));

            element.Click();

            WaitFor(() => { return driver.Title == "clicks"; }, "Browser title was not 'clicks'");
        }

        [Test]
        public void ShouldBeAbleToClickOnRightToLeftLanguageLink()
        {
            String url = EnvironmentManager.Instance.UrlBuilder.WhereIs("click_rtl.html");
            driver.Url = url;

            IWebElement element = driver.FindElement(By.Id("ar_link"));
            element.Click();

            WaitFor(() => driver.Title == "clicks", "Expected title to be 'clicks'");
            Assert.That(driver.Title, Is.EqualTo("clicks"));
        }

        [Test]
        public void ShouldBeAbleToClickOnLinkInAbsolutelyPositionedFooter()
        {
            string url = EnvironmentManager.Instance.UrlBuilder.WhereIs("fixedFooterNoScroll.html");
            driver.Url = url;

            driver.FindElement(By.Id("link")).Click();
            WaitFor(() => { return driver.Title == "XHTML Test Page"; }, "Browser title was not 'XHTML Test Page'");
            Assert.That(driver.Title, Is.EqualTo("XHTML Test Page"));
        }

        [Test]
        public void ShouldBeAbleToClickOnLinkInAbsolutelyPositionedFooterInQuirksMode()
        {
            string url = EnvironmentManager.Instance.UrlBuilder.WhereIs("fixedFooterNoScrollQuirksMode.html");
            driver.Url = url;

            driver.FindElement(By.Id("link")).Click();
            WaitFor(() => { return driver.Title == "XHTML Test Page"; }, "Browser title was not 'XHTML Test Page'");
            Assert.That(driver.Title, Is.EqualTo("XHTML Test Page"));
        }

        [Test]
        public void ShouldBeAbleToClickOnLinksWithNoHrefAttribute()
        {
            driver.Url = javascriptPage;

            IWebElement element = driver.FindElement(By.LinkText("No href"));
            element.Click();

            WaitFor(() => driver.Title == "Changed", "Expected title to be 'Changed'");
            Assert.That(driver.Title, Is.EqualTo("Changed"));
        }

        [Test]
        public void ShouldBeAbleToClickOnALinkThatWrapsToTheNextLine()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("click_tests/link_that_wraps.html");

            driver.FindElement(By.Id("link")).Click();

            WaitFor(() => driver.Title == "Submitted Successfully!", "Expected title to be 'Submitted Successfully!'");
            Assert.That(driver.Title, Is.EqualTo("Submitted Successfully!"));
        }

        [Test]
        public void ShouldBeAbleToClickOnASpanThatWrapsToTheNextLine()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("click_tests/span_that_wraps.html");

            driver.FindElement(By.Id("span")).Click();

            WaitFor(() => driver.Title == "Submitted Successfully!", "Expected title to be 'Submitted Successfully!'");
            Assert.That(driver.Title, Is.EqualTo("Submitted Successfully!"));
        }

        [Test]
        public void ClickingOnADisabledElementIsANoOp()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("click_tests/disabled_element.html");

            IWebElement element = driver.FindElement(By.Name("disabled"));
            element.Click();
        }

        //------------------------------------------------------------------
        // Tests below here are not included in the Java test suite
        //------------------------------------------------------------------
        [Test]
        public void ShouldBeAbleToClickLinkContainingLineBreak()
        {
            driver.Url = simpleTestPage;
            driver.FindElement(By.Id("multilinelink")).Click();
            WaitFor(() => { return driver.Title == "We Arrive Here"; }, "Browser title was not 'We Arrive Here'");
            Assert.That(driver.Title, Is.EqualTo("We Arrive Here"));
        }
    }
}
