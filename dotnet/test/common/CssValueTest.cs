// <copyright file="CssValueTest.cs" company="Selenium Committers">
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

namespace OpenQA.Selenium
{
    [TestFixture]
    public class CssValueTest : DriverTestFixture
    {
        [Test]
        public void ShouldPickUpStyleOfAnElement()
        {
            driver.Url = javascriptPage;

            IWebElement element = driver.FindElement(By.Id("green-parent"));
            string backgroundColour = element.GetCssValue("background-color");

            Assert.That(backgroundColour, Is.EqualTo("#008000").Or.EqualTo("rgba(0, 128, 0, 1)").Or.EqualTo("rgb(0, 128, 0)"));

            element = driver.FindElement(By.Id("red-item"));
            backgroundColour = element.GetCssValue("background-color");

            Assert.That(backgroundColour, Is.EqualTo("#ff0000").Or.EqualTo("rgba(255, 0, 0, 1)").Or.EqualTo("rgb(255, 0, 0)"));
        }

        [Test]
        public void GetCssValueShouldReturnStandardizedColour()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("colorPage.html");

            IWebElement element = driver.FindElement(By.Id("namedColor"));
            string backgroundColour = element.GetCssValue("background-color");
            Assert.That(backgroundColour, Is.EqualTo("rgba(0, 128, 0, 1)").Or.EqualTo("rgb(0, 128, 0)"));

            element = driver.FindElement(By.Id("rgb"));
            backgroundColour = element.GetCssValue("background-color");
            Assert.That(backgroundColour, Is.EqualTo("rgba(0, 128, 0, 1)").Or.EqualTo("rgb(0, 128, 0)"));
        }

        [Test]
        public void ShouldAllowInheritedStylesToBeUsed()
        {
            driver.Url = javascriptPage;

            IWebElement element = driver.FindElement(By.Id("green-item"));
            string backgroundColour = element.GetCssValue("background-color");

            Assert.That(backgroundColour, Is.EqualTo("transparent").Or.EqualTo("rgba(0, 0, 0, 0)"));
        }
    }
}
