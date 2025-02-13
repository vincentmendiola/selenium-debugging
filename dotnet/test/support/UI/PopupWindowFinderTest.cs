// <copyright file="PopupWindowFinderTest.cs" company="Selenium Committers">
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
using System.Threading.Tasks;

namespace OpenQA.Selenium.Support.UI
{
    [TestFixture]
    public class PopupWindowFinderTest : DriverTestFixture
    {
        //TODO: Move these to a standalone class when more tests rely on the server being up
        [OneTimeSetUp]
        public async Task RunBeforeAnyTestAsync()
        {
            await EnvironmentManager.Instance.WebServer.StartAsync();
        }

        [OneTimeTearDown]
        public async Task RunAfterAnyTestsAsync()
        {
            EnvironmentManager.Instance.CloseCurrentDriver();
            await EnvironmentManager.Instance.WebServer.StopAsync();
        }

        [Test]
        public void ShouldFindPopupWindowUsingAction()
        {
            driver.Url = xhtmlTestPage;
            string current = driver.CurrentWindowHandle;

            PopupWindowFinder finder = new PopupWindowFinder(driver);
            string newHandle = finder.Invoke(() => { driver.FindElement(By.LinkText("Open new window")).Click(); });

            Assert.That(newHandle, Is.Not.Null.And.Not.Empty);
            Assert.That(newHandle, Is.Not.EqualTo(current));

            driver.SwitchTo().Window(newHandle);
            Assert.That(driver.Title, Is.EqualTo("We Arrive Here"));
            driver.Close();

            driver.SwitchTo().Window(current);
        }

        [Test]
        public void ShouldFindPopupWindowUsingElementClick()
        {
            driver.Url = xhtmlTestPage;
            string current = driver.CurrentWindowHandle;

            PopupWindowFinder finder = new PopupWindowFinder(driver);
            string newHandle = finder.Click(driver.FindElement(By.LinkText("Open new window")));

            Assert.That(newHandle, Is.Not.Null.And.Not.Empty);
            Assert.That(newHandle, Is.Not.EqualTo(current));

            driver.SwitchTo().Window(newHandle);
            Assert.That(driver.Title, Is.EqualTo("We Arrive Here"));
            driver.Close();

            driver.SwitchTo().Window(current);
        }

        [Test]
        public void ShouldFindMultiplePopupWindowsInSuccession()
        {
            driver.Url = xhtmlTestPage;
            string first = driver.CurrentWindowHandle;

            PopupWindowFinder finder = new PopupWindowFinder(driver);
            string second = finder.Click(driver.FindElement(By.Name("windowOne")));
            Assert.That(second, Is.Not.Null.And.Not.Empty);
            Assert.That(second, Is.Not.EqualTo(first));

            finder = new PopupWindowFinder(driver);
            string third = finder.Click(driver.FindElement(By.Name("windowTwo")));
            Assert.That(third, Is.Not.Null.And.Not.Empty);
            Assert.That(third, Is.Not.EqualTo(first));
            Assert.That(third, Is.Not.EqualTo(second));

            driver.SwitchTo().Window(second);
            driver.Close();

            driver.SwitchTo().Window(third);
            driver.Close();

            driver.SwitchTo().Window(first);
        }

        [Test]
        public void ShouldNotFindPopupWindowWhenNoneExists()
        {
            driver.Url = xhtmlTestPage;
            PopupWindowFinder finder = new PopupWindowFinder(driver);
            Assert.That(
                () => finder.Click(driver.FindElement(By.Id("linkId"))),
                Throws.TypeOf<WebDriverTimeoutException>());
        }
    }
}
