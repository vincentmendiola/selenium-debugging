// <copyright file="AlertsTest.cs" company="Selenium Committers">
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
using System.Collections.Generic;

namespace OpenQA.Selenium
{
    [TestFixture]
    public class AlertsTest : DriverTestFixture
    {
        [Test]
        public void ShouldBeAbleToOverrideTheWindowAlertMethod()
        {
            driver.Url = CreateAlertPage("cheese");

            ((IJavaScriptExecutor)driver).ExecuteScript(
                "window.alert = function(msg) { document.getElementById('text').innerHTML = msg; }");
            driver.FindElement(By.Id("alert")).Click();
        }

        [Test]
        public void ShouldAllowUsersToAcceptAnAlertManually()
        {
            driver.Url = CreateAlertPage("cheese");

            driver.FindElement(By.Id("alert")).Click();

            IAlert alert = WaitFor<IAlert>(AlertToBePresent, "No alert found");
            alert.Accept();

            // If we can perform any action, we're good to go
            Assert.That(driver.Title, Is.EqualTo("Testing Alerts"));
        }

        [Test]
        public void ShouldThrowArgumentNullExceptionWhenKeysNull()
        {
            driver.Url = CreateAlertPage("cheese");

            driver.FindElement(By.Id("alert")).Click();
            IAlert alert = WaitFor<IAlert>(AlertToBePresent, "No alert found");
            try
            {
                Assert.That(
                    () => alert.SendKeys(null),
                    Throws.ArgumentNullException);
            }
            finally
            {
                alert.Accept();
            }
        }

        [Test]
        public void ShouldAllowUsersToAcceptAnAlertWithNoTextManually()
        {
            driver.Url = CreateAlertPage("");

            driver.FindElement(By.Id("alert")).Click();

            IAlert alert = WaitFor<IAlert>(AlertToBePresent, "No alert found");
            alert.Accept();

            // If we can perform any action, we're good to go
            Assert.That(driver.Title, Is.EqualTo("Testing Alerts"));
        }

        [Test]
        public void ShouldAllowUsersToDismissAnAlertManually()
        {
            driver.Url = CreateAlertPage("cheese");

            driver.FindElement(By.Id("alert")).Click();

            IAlert alert = WaitFor<IAlert>(AlertToBePresent, "No alert found");
            alert.Dismiss();

            // If we can perform any action, we're good to go
            Assert.That(driver.Title, Is.EqualTo("Testing Alerts"));
        }

        [Test]
        public void ShouldAllowAUserToAcceptAPrompt()
        {
            driver.Url = CreatePromptPage(null);

            driver.FindElement(By.Id("prompt")).Click();

            IAlert alert = WaitFor<IAlert>(AlertToBePresent, "No alert found");
            alert.Accept();

            // If we can perform any action, we're good to go
            Assert.That(driver.Title, Is.EqualTo("Testing Prompt"));
        }

        [Test]
        public void ShouldAllowAUserToDismissAPrompt()
        {
            driver.Url = CreatePromptPage(null);

            driver.FindElement(By.Id("prompt")).Click();

            IAlert alert = WaitFor<IAlert>(AlertToBePresent, "No alert found");
            alert.Dismiss();

            // If we can perform any action, we're good to go
            Assert.That(driver.Title, Is.EqualTo("Testing Prompt"));
        }

        [Test]
        public void ShouldAllowAUserToSetTheValueOfAPrompt()
        {
            driver.Url = CreatePromptPage(null);

            driver.FindElement(By.Id("prompt")).Click();

            IAlert alert = WaitFor<IAlert>(AlertToBePresent, "No alert found");
            alert.SendKeys("cheese");
            alert.Accept();

            string result = driver.FindElement(By.Id("text")).Text;
            Assert.That(result, Is.EqualTo("cheese"));
        }

        [Test]
        public void SettingTheValueOfAnAlertThrows()
        {
            driver.Url = CreateAlertPage("cheese");

            driver.FindElement(By.Id("alert")).Click();

            IAlert alert = WaitFor<IAlert>(AlertToBePresent, "No alert found");

            try
            {
                Assert.That(
                    () => alert.SendKeys("cheese"),
                    Throws.TypeOf<ElementNotInteractableException>());
            }
            finally
            {
                alert.Accept();
            }
        }

        [Test]
        public void ShouldAllowTheUserToGetTheTextOfAnAlert()
        {
            driver.Url = CreateAlertPage("cheese");

            driver.FindElement(By.Id("alert")).Click();

            IAlert alert = WaitFor<IAlert>(AlertToBePresent, "No alert found");
            string value = alert.Text;
            alert.Accept();

            Assert.That(value, Is.EqualTo("cheese"));
        }

        [Test]
        public void ShouldAllowTheUserToGetTheTextOfAPrompt()
        {
            driver.Url = CreatePromptPage(null);

            driver.FindElement(By.Id("prompt")).Click();

            IAlert alert = WaitFor<IAlert>(AlertToBePresent, "No alert found");
            string value = alert.Text;
            alert.Accept();

            Assert.That(value, Is.EqualTo("Enter something"));
        }

        [Test]
        public void AlertShouldNotAllowAdditionalCommandsIfDismissed()
        {
            driver.Url = CreateAlertPage("cheese");

            driver.FindElement(By.Id("alert")).Click();

            IAlert alert = WaitFor<IAlert>(AlertToBePresent, "No alert found");
            alert.Dismiss();

            Assert.That(
                () => alert.Text,
                Throws.TypeOf<NoAlertPresentException>());
        }

        [Test]
        public void ShouldAllowUsersToAcceptAnAlertInAFrame()
        {
            string iframe = EnvironmentManager.Instance.UrlBuilder.CreateInlinePage(new InlinePage()
                .WithBody("<a href='#' id='alertInFrame' onclick='alert(\"framed cheese\");'>click me</a>"));
            driver.Url = EnvironmentManager.Instance.UrlBuilder.CreateInlinePage(new InlinePage()
                .WithTitle("Testing Alerts")
                .WithBody(String.Format("<iframe src='{0}' name='iframeWithAlert'></iframe>", iframe)));

            driver.SwitchTo().Frame("iframeWithAlert");

            driver.FindElement(By.Id("alertInFrame")).Click();

            IAlert alert = WaitFor<IAlert>(AlertToBePresent, "No alert found");
            alert.Accept();

            // If we can perform any action, we're good to go
            Assert.That(driver.Title, Is.EqualTo("Testing Alerts"));
        }

        [Test]
        public void ShouldAllowUsersToAcceptAnAlertInANestedFrame()
        {
            string iframe = EnvironmentManager.Instance.UrlBuilder.CreateInlinePage(new InlinePage()
                .WithBody("<a href='#' id='alertInFrame' onclick='alert(\"framed cheese\");'>click me</a>"));
            string iframe2 = EnvironmentManager.Instance.UrlBuilder.CreateInlinePage(new InlinePage()
                .WithBody(string.Format("<iframe src='{0}' name='iframeWithAlert'></iframe>", iframe)));
            driver.Url = EnvironmentManager.Instance.UrlBuilder.CreateInlinePage(new InlinePage()
                .WithTitle("Testing Alerts")
                .WithBody(string.Format("<iframe src='{0}' name='iframeWithIframe'></iframe>", iframe2)));

            driver.SwitchTo().Frame("iframeWithIframe").SwitchTo().Frame("iframeWithAlert");

            driver.FindElement(By.Id("alertInFrame")).Click();

            IAlert alert = WaitFor<IAlert>(AlertToBePresent, "No alert found");
            alert.Accept();

            // If we can perform any action, we're good to go
            Assert.That(driver.Title, Is.EqualTo("Testing Alerts"));
        }

        [Test]
        public void SwitchingToMissingAlertThrows()
        {
            driver.Url = CreateAlertPage("cheese");

            Assert.That(
                () => AlertToBePresent(),
                Throws.TypeOf<NoAlertPresentException>());
        }

        [Test]
        [IgnoreBrowser(Browser.IE, "Edge in IE Mode does not properly handle multiple windows")]
        public void SwitchingToMissingAlertInAClosedWindowThrows()
        {
            string blank = EnvironmentManager.Instance.UrlBuilder.CreateInlinePage(new InlinePage());
            driver.Url = EnvironmentManager.Instance.UrlBuilder.CreateInlinePage(new InlinePage()
                .WithBody(String.Format(
                    "<a id='open-new-window' href='{0}' target='newwindow'>open new window</a>", blank)));

            string mainWindow = driver.CurrentWindowHandle;
            try
            {
                driver.FindElement(By.Id("open-new-window")).Click();
                WaitFor(WindowHandleCountToBe(2), "Window count was not 2");
                WaitFor(WindowWithName("newwindow"), "Could not find window with name 'newwindow'");
                driver.Close();
                WaitFor(WindowHandleCountToBe(1), "Window count was not 1");

                Assert.That(
                    () => AlertToBePresent().Accept(),
                    Throws.TypeOf<NoSuchWindowException>());

            }
            finally
            {
                driver.SwitchTo().Window(mainWindow);
                WaitFor(ElementTextToEqual(driver.FindElement(By.Id("open-new-window")), "open new window"), "Could not find element with text 'open new window'");
            }
        }

        [Test]
        public void PromptShouldUseDefaultValueIfNoKeysSent()
        {
            driver.Url = CreatePromptPage("This is a default value");
            driver.FindElement(By.Id("prompt")).Click();

            IAlert alert = WaitFor<IAlert>(AlertToBePresent, "No alert found");
            alert.Accept();

            IWebElement element = driver.FindElement(By.Id("text"));
            WaitFor(ElementTextToEqual(element, "This is a default value"), "Element text was not 'This is a default value'");
            Assert.That(element.Text, Is.EqualTo("This is a default value"));
        }

        [Test]
        public void PromptShouldHaveNullValueIfDismissed()
        {
            driver.Url = CreatePromptPage("This is a default value");
            driver.FindElement(By.Id("prompt")).Click();

            IAlert alert = WaitFor<IAlert>(AlertToBePresent, "No alert found");
            alert.Dismiss();
            IWebElement element = driver.FindElement(By.Id("text"));
            WaitFor(ElementTextToEqual(element, "null"), "Element text was not 'null'");
            Assert.That(element.Text, Is.EqualTo("null"));
        }

        [Test]
        [IgnoreBrowser(Browser.Remote)]
        public void HandlesTwoAlertsFromOneInteraction()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.CreateInlinePage(new InlinePage()
                .WithScripts(
                    """
                    function setInnerText(id, value) {
                      document.getElementById(id).innerHTML = '<p>' + value + '</p>';
                    }

                    function displayTwoPrompts() {
                      setInnerText('text1', prompt('First'));
                      setInnerText('text2', prompt('Second'));
                    }
                    """)
                .WithBody(
                    """
                    <a href='#' id='double-prompt' onclick='displayTwoPrompts();'>click me</a>
                    <div id='text1'></div>
                    <div id='text2'></div>
                    """));

            driver.FindElement(By.Id("double-prompt")).Click();

            IAlert alert1 = WaitFor<IAlert>(AlertToBePresent, "No alert found");
            alert1.SendKeys("brie");
            alert1.Accept();

            IAlert alert2 = WaitFor<IAlert>(AlertToBePresent, "No alert found");
            alert2.SendKeys("cheddar");
            alert2.Accept();

            IWebElement element1 = driver.FindElement(By.Id("text1"));
            WaitFor(ElementTextToEqual(element1, "brie"), "Element text was not 'brie'");
            Assert.That(element1.Text, Is.EqualTo("brie"));
            IWebElement element2 = driver.FindElement(By.Id("text2"));
            WaitFor(ElementTextToEqual(element2, "cheddar"), "Element text was not 'cheddar'");
            Assert.That(element2.Text, Is.EqualTo("cheddar"));
        }

        [Test]
        public void ShouldHandleAlertOnPageLoad()
        {
            string pageWithOnLoad = EnvironmentManager.Instance.UrlBuilder.CreateInlinePage(new InlinePage()
                .WithOnLoad("""javascript:alert("onload")""")
                .WithBody("<p>Page with onload event handler</p>"));
            driver.Url = EnvironmentManager.Instance.UrlBuilder.CreateInlinePage(new InlinePage()
                .WithBody(string.Format("<a id='open-page-with-onload-alert' href='{0}'>open new page</a>", pageWithOnLoad)));

            driver.FindElement(By.Id("open-page-with-onload-alert")).Click();

            IAlert alert = WaitFor<IAlert>(AlertToBePresent, "No alert found");
            string value = alert.Text;
            alert.Accept();

            Assert.That(value, Is.EqualTo("onload"));
            IWebElement element = driver.FindElement(By.TagName("p"));
            WaitFor(ElementTextToEqual(element, "Page with onload event handler"), "Element text was not 'Page with onload event handler'");
        }

        [Test]

        public void ShouldHandleAlertOnPageLoadUsingGet()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.CreateInlinePage(new InlinePage()
                .WithOnLoad("javascript:alert(\"onload\")")
                .WithBody("<p>Page with onload event handler</p>"));

            IAlert alert = WaitFor<IAlert>(AlertToBePresent, "No alert found");
            string value = alert.Text;
            alert.Accept();

            Assert.That(value, Is.EqualTo("onload"));
            WaitFor(ElementTextToEqual(driver.FindElement(By.TagName("p")), "Page with onload event handler"), "Could not find element with text 'Page with onload event handler'");
        }

        [Test]
        [IgnoreBrowser(Browser.IE, "Edge in IE Mode does not properly handle multiple windows")]
        [IgnoreBrowser(Browser.Chrome, "Test with onLoad alert hangs Chrome.")]
        [IgnoreBrowser(Browser.Edge, "Test with onLoad alert hangs Edge.")]
        [IgnoreBrowser(Browser.Safari, "Safari driver does not allow commands in any window when an alert is active")]
        public void ShouldNotHandleAlertInAnotherWindow()
        {
            string pageWithOnLoad = EnvironmentManager.Instance.UrlBuilder.CreateInlinePage(new InlinePage()
                .WithOnLoad("javascript:alert(\"onload\")")
                .WithBody("<p>Page with onload event handler</p>"));
            driver.Url = EnvironmentManager.Instance.UrlBuilder.CreateInlinePage(new InlinePage()
                .WithBody(string.Format(
                    "<a id='open-new-window' href='{0}' target='newwindow'>open new window</a>", pageWithOnLoad)));

            string mainWindow = driver.CurrentWindowHandle;
            string onloadWindow = null;
            try
            {
                driver.FindElement(By.Id("open-new-window")).Click();
                List<String> allWindows = new List<string>(driver.WindowHandles);
                allWindows.Remove(mainWindow);
                Assert.That(allWindows, Has.One.Items);
                onloadWindow = allWindows[0];

                Assert.That(() =>
                {
                    IWebElement el = driver.FindElement(By.Id("open-new-window"));
                    WaitFor<IAlert>(AlertToBePresent, TimeSpan.FromSeconds(5), "No alert found");
                },
                Throws.TypeOf<WebDriverException>());

            }
            finally
            {
                driver.SwitchTo().Window(onloadWindow);
                WaitFor<IAlert>(AlertToBePresent, "No alert found").Dismiss();
                driver.Close();
                driver.SwitchTo().Window(mainWindow);
                WaitFor(ElementTextToEqual(driver.FindElement(By.Id("open-new-window")), "open new window"), "Could not find element with text 'open new window'");
            }
        }

        [Test]
        [IgnoreBrowser(Browser.Firefox, "Driver chooses not to return text from unhandled alert")]
        public void IncludesAlertTextInUnhandledAlertException()
        {
            driver.Url = CreateAlertPage("cheese");

            driver.FindElement(By.Id("alert")).Click();
            WaitFor<IAlert>(AlertToBePresent, "No alert found");

            Assert.That(
                () => driver.Title,
                Throws.TypeOf<UnhandledAlertException>().With.Property(nameof(UnhandledAlertException.AlertText)).EqualTo("cheese"));
        }

        [Test]
        [NeedsFreshDriver(IsCreatedAfterTest = true)]
        public void CanQuitWhenAnAlertIsPresent()
        {
            driver.Url = CreateAlertPage("cheese");
            driver.FindElement(By.Id("alert")).Click();
            IAlert alert = WaitFor<IAlert>(AlertToBePresent, "No alert found");
            EnvironmentManager.Instance.CloseCurrentDriver();
        }

        [Test]
        public void ShouldHandleAlertOnFormSubmit()
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.CreateInlinePage(new InlinePage()
                .WithTitle("Testing Alerts").
                WithBody("<form id='theForm' action='javascript:alert(\"Tasty cheese\");'>",
                    "<input id='unused' type='submit' value='Submit'>",
                    "</form>"));

            IWebElement element = driver.FindElement(By.Id("theForm"));
            element.Submit();
            IAlert alert = WaitFor<IAlert>(AlertToBePresent, "No alert found");
            string text = alert.Text;
            alert.Accept();

            Assert.That(text, Is.EqualTo("Tasty cheese"));
            Assert.That(driver.Title, Is.EqualTo("Testing Alerts"));
        }

        private IAlert AlertToBePresent()
        {
            return driver.SwitchTo().Alert();
        }

        private string CreateAlertPage(string alertText)
        {
            return EnvironmentManager.Instance.UrlBuilder.CreateInlinePage(new InlinePage()
                .WithTitle("Testing Alerts")
                .WithBody("<a href='#' id='alert' onclick='alert(\"" + alertText + "\");'>click me</a>"));
        }

        private string CreatePromptPage(string defaultText)
        {
            return EnvironmentManager.Instance.UrlBuilder.CreateInlinePage(new InlinePage()
                .WithTitle("Testing Prompt")
                .WithScripts(
                    "function setInnerText(id, value) {",
                    "  document.getElementById(id).innerHTML = '<p>' + value + '</p>';",
                    "}",
                    defaultText == null
                      ? "function displayPrompt() { setInnerText('text', prompt('Enter something')); }"
                      : "function displayPrompt() { setInnerText('text', prompt('Enter something', '" + defaultText + "')); }")

                .WithBody(
                    "<a href='#' id='prompt' onclick='displayPrompt();'>click me</a>",
                    "<div id='text'>acceptor</div>"));
        }

        private void SetSimpleOnBeforeUnload(string returnText)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript(
                "var returnText = arguments[0]; window.onbeforeunload = function() { return returnText; }",
                returnText);
        }

        private Func<IWebElement> ElementToBePresent(By locator)
        {
            return () =>
            {
                try
                {
                    return driver.FindElement(By.Id("open-page-with-onunload-alert"));
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            };
        }

        private Func<bool> ElementTextToEqual(IWebElement element, string text)
        {
            return () =>
            {
                return element.Text == text;
            };
        }

        private Func<bool> WindowWithName(string name)
        {
            return () =>
            {
                try
                {
                    driver.SwitchTo().Window(name);
                    return true;
                }
                catch (NoSuchWindowException)
                {
                    return false;
                }
            };
        }

        private Func<bool> WindowHandleCountToBe(int count)
        {
            return () =>
            {
                return driver.WindowHandles.Count == count;
            };
        }

    }
}
