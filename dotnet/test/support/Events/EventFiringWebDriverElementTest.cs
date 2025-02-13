// <copyright file="EventFiringWebDriverElementTest.cs" company="Selenium Committers">
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

namespace OpenQA.Selenium.Support.Events
{
    [TestFixture]
    public class EventFiringWebDriverElementTest : DriverTestFixture
    {
        [SetUp]
        public void Setup()
        {
            driver.Url = formsPage;
        }

        [Test]
        public void CanTakeEventFiringWebElementScreenshot()
        {
            var firingDriver = new EventFiringWebDriver(driver);
            IWebElement element = firingDriver.FindElement(By.Name("checky"));
            Screenshot screenshot = ((ITakesScreenshot)element).GetScreenshot();

            Assert.That(screenshot, Is.Not.Null);
        }
    }
}
