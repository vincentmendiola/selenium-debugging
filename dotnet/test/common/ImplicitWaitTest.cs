// <copyright file="ImplicitWaitTest.cs" company="Selenium Committers">
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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OpenQA.Selenium
{
    [TestFixture]
    public class ImplicitWaitTest : DriverTestFixture
    {
        [TearDown]
        public void ResetImplicitWaitTimeout()
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(0);
        }

        [Test]
        public void ShouldImplicitlyWaitForASingleElement()
        {
            driver.Url = dynamicPage;
            IWebElement add = driver.FindElement(By.Id("adder"));

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(3000);

            add.Click();
            driver.FindElement(By.Id("box0"));  // All is well if this doesn't throw.
        }

        [Test]
        public void ShouldStillFailToFindAnElementWhenImplicitWaitsAreEnabled()
        {
            driver.Url = dynamicPage;
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);
            Assert.That(() => driver.FindElement(By.Id("box0")), Throws.InstanceOf<NoSuchElementException>());
        }

        [Test]
        [NeedsFreshDriver]
        public void ShouldReturnAfterFirstAttemptToFindOneAfterDisablingImplicitWaits()
        {
            driver.Url = dynamicPage;
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(3000);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(0);
            Assert.That(() => driver.FindElement(By.Id("box0")), Throws.InstanceOf<NoSuchElementException>());
        }

        [Test]
        [NeedsFreshDriver]
        public void ShouldImplicitlyWaitUntilAtLeastOneElementIsFoundWhenSearchingForMany()
        {
            driver.Url = dynamicPage;
            IWebElement add = driver.FindElement(By.Id("adder"));

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(2000);
            add.Click();
            add.Click();

            ReadOnlyCollection<IWebElement> elements = driver.FindElements(By.ClassName("redbox"));
            Assert.That(elements, Has.Count.GreaterThanOrEqualTo(1));
        }

        [Test]
        [NeedsFreshDriver]
        public void ShouldStillFailToFindElementsWhenImplicitWaitsAreEnabled()
        {
            driver.Url = dynamicPage;

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(By.ClassName("redbox"));
            Assert.That(elements, Is.Empty);
        }

        [Test]
        [NeedsFreshDriver]
        public void ShouldReturnAfterFirstAttemptToFindManyAfterDisablingImplicitWaits()
        {
            driver.Url = dynamicPage;
            IWebElement add = driver.FindElement(By.Id("adder"));
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(1100);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(0);
            add.Click();
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(By.ClassName("redbox"));
            Assert.That(elements, Is.Empty);
        }

        [Test]
        [IgnoreBrowser(Browser.IE, "Driver does not implement waiting for element visible for interaction")]
        [IgnoreBrowser(Browser.Firefox, "Driver does not implement waiting for element visible for interaction")]
        [IgnoreBrowser(Browser.Safari, "Driver does not implement waiting for element visible for interaction")]
        public void ShouldImplicitlyWaitForAnElementToBeVisibleBeforeInteracting()
        {
            driver.Url = dynamicPage;

            IWebElement reveal = driver.FindElement(By.Id("reveal"));
            IWebElement revealed = driver.FindElement(By.Id("revealed"));
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(5000);

            Assert.That(revealed.Displayed, Is.False, "revealed should not be visible");
            reveal.Click();

            try
            {
                revealed.SendKeys("hello world");
                // This is what we want
            }
            catch (ElementNotInteractableException)
            {
                Assert.Fail("Element should have been visible");
            }
        }

        [Test]
        [IgnoreBrowser(Browser.IE, "Edge in IE Mode does not properly handle multiple windows")]
        public void ShouldRetainImplicitlyWaitFromTheReturnedWebDriverOfWindowSwitchTo()
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(1);
            driver.Url = xhtmlTestPage;
            driver.FindElement(By.Name("windowOne")).Click();

            string originalHandle = driver.CurrentWindowHandle;
            WaitFor(() => driver.WindowHandles.Count == 2, "Window handle count was not 2");
            List<string> handles = new List<string>(driver.WindowHandles);
            handles.Remove(originalHandle);

            IWebDriver newWindow = driver.SwitchTo().Window(handles[0]);

            DateTime start = DateTime.Now;
            newWindow.FindElements(By.Id("this-crazy-thing-does-not-exist"));
            DateTime end = DateTime.Now;
            TimeSpan time = end - start;

            driver.Close();
            driver.SwitchTo().Window(originalHandle);
            Assert.That(time.TotalMilliseconds, Is.GreaterThanOrEqualTo(1000));
        }
    }
}
