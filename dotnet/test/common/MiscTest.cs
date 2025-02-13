// <copyright file="MiscTest.cs" company="Selenium Committers">
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
using System.Collections.Generic;

namespace OpenQA.Selenium
{
    [TestFixture]
    public class MiscTest : DriverTestFixture
    {
        [Test]
        public void ShouldReturnTitleOfPageIfSet()
        {
            driver.Url = xhtmlTestPage;
            Assert.That(driver.Title, Is.EqualTo("XHTML Test Page"));

            driver.Url = simpleTestPage;
            Assert.That(driver.Title, Is.EqualTo("Hello WebDriver"));
        }

        [Test]
        public void ShouldReportTheCurrentUrlCorrectly()
        {
            driver.Url = macbethPage;
            Assert.That(driver.Url, Is.EqualTo(macbethPage));

            driver.Url = simpleTestPage;
            Assert.That(driver.Url, Is.EqualTo(simpleTestPage));

            driver.Url = javascriptPage;
            Assert.That(driver.Url, Is.EqualTo(javascriptPage));
        }

        [Test]
        public void ShouldReturnTagName()
        {
            driver.Url = formsPage;
            IWebElement selectBox = driver.FindElement(By.Id("cheese"));
            Assert.That(selectBox.TagName.ToLower(), Is.EqualTo("input"));
        }

        [Test]
        public void ShouldReturnTheSourceOfAPage()
        {
            string pageSource;
            driver.Url = simpleTestPage;
            pageSource = driver.PageSource.ToLower();

            Assert.That(pageSource, Does.StartWith("<html"));
            Assert.That(pageSource, Does.EndWith("</html>"));
            Assert.That(pageSource, Does.Contain("an inline element"));
            Assert.That(pageSource, Does.Contain("<p id="));
            Assert.That(pageSource, Does.Contain("lotsofspaces"));
            Assert.That(pageSource, Does.Contain("with document.write and with document.write again"));
        }

        [Test]
        [IgnoreBrowser(Browser.Chrome, "returns XML content formatted for display as HTML document")]
        [IgnoreBrowser(Browser.Edge, "returns XML content formatted for display as HTML document")]
        [IgnoreBrowser(Browser.Safari, "returns XML content formatted for display as HTML document")]
        [IgnoreBrowser(Browser.IE, "returns XML content formatted for display as HTML document")]
        public void ShouldBeAbleToGetTheSourceOfAnXmlDocument()
        {
            driver.Url = simpleXmlDocument;
            string source = driver.PageSource.ToLower();
            source = System.Text.RegularExpressions.Regex.Replace(source, "\\s", string.Empty);
            Assert.That(source, Is.EqualTo("<xml><foo><bar>baz</bar></foo></xml>"));
        }

        // Test is ignored for all browsers, but is kept here in the source code for
        // ease of comparison to Java test suite.
        //[Test]
        //[IgnoreBrowser(Browser.All, "issue 2282")]
        //public void StimulatesStrangeOnloadInteractionInFirefox()
        //{
        //    driver.Url = documentWrite;

        //    // If this command succeeds, then all is well.
        //    driver.FindElement(By.XPath("//body"));

        //    driver.Url = simpleTestPage;
        //    driver.FindElement(By.Id("links"));
        //}

        [Test]
        public void ClickingShouldNotTrampleWOrHInGlobalScope()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("globalscope.html");
            List<string> values = new List<string>() { "w", "h" };

            foreach (string val in values)
            {
                Assert.That(GetGlobalVar(driver, val), Is.EqualTo(val));
            }

            driver.FindElement(By.Id("toclick")).Click();

            foreach (string val in values)
            {
                Assert.That(GetGlobalVar(driver, val), Is.EqualTo(val));
            }
        }

        private string GetGlobalVar(IWebDriver driver, string value)
        {
            object val = ((IJavaScriptExecutor)driver).ExecuteScript("return window." + value + ";");
            return val == null ? "null" : val.ToString();
        }
    }
}
