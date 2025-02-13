// <copyright file="CorrectEventFiringTest.cs" company="Selenium Committers">
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
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace OpenQA.Selenium
{
    [TestFixture]
    public class CorrectEventFiringTest : DriverTestFixture
    {
        [Test]
        public void ShouldFireFocusEventWhenClicking()
        {
            driver.Url = javascriptPage;

            ClickOnElementWhichRecordsEvents(driver);

            AssertEventFired("focus", driver);
        }

        [Test]
        [NeedsFreshDriver(IsCreatedBeforeTest = true, IsCreatedAfterTest = true)]
        [IgnoreBrowser(Browser.Safari, "Safari driver does not support multiple instances")]
        public void ShouldFireFocusEventInNonTopmostWindow()
        {
            IWebDriver driver2 = EnvironmentManager.Instance.CreateDriverInstance();
            try
            {
                // topmost
                driver2.Url = javascriptPage;
                ClickOnElementWhichRecordsEvents(driver2);
                AssertEventFired("focus", driver2);

                // non-topmost
                driver.Url = javascriptPage;
                ClickOnElementWhichRecordsEvents(driver);
                AssertEventFired("focus", driver);

            }
            finally
            {
                driver2.Quit();
            }
        }

        [Test]
        public void ShouldFireClickEventWhenClicking()
        {
            driver.Url = javascriptPage;

            ClickOnElementWhichRecordsEvents(driver);

            AssertEventFired("click", driver);
        }

        [Test]
        public void ShouldFireMouseDownEventWhenClicking()
        {
            driver.Url = javascriptPage;

            ClickOnElementWhichRecordsEvents(driver);

            AssertEventFired("mousedown", driver);
        }

        [Test]
        public void ShouldFireMouseUpEventWhenClicking()
        {
            driver.Url = javascriptPage;

            ClickOnElementWhichRecordsEvents(driver);

            AssertEventFired("mouseup", driver);
        }

        [Test]
        public void ShouldFireMouseOverEventWhenClicking()
        {
            driver.Url = javascriptPage;

            ClickOnElementWhichRecordsEvents(driver);

            AssertEventFired("mouseover", driver);
        }

        [Test]
        [IgnoreBrowser(Browser.Firefox, "Firefox does not report mouse move event when clicking")]
        public void ShouldFireMouseMoveEventWhenClicking()
        {
            driver.Url = javascriptPage;

            // This bears some explanation. In certain cases, if the prior test
            // leaves the mouse cursor immediately over the wrong element, then
            // the mousemove event may not get fired, because the mouse does not
            // actually move. Prevent this situation by forcing the mouse to move
            // to the origin.
            new Actions(driver).MoveToElement(driver.FindElement(By.TagName("body"))).Perform();

            ClickOnElementWhichRecordsEvents(driver);

            AssertEventFired("mousemove", driver);
        }

        [Test]
        public void ShouldNotThrowIfEventHandlerThrows()
        {
            driver.Url = javascriptPage;
            driver.FindElement(By.Id("throwing-mouseover")).Click();
        }

        [Test]
        public void ShouldFireEventsInTheRightOrder()
        {
            driver.Url = javascriptPage;

            ClickOnElementWhichRecordsEvents(driver);

            string text = driver.FindElement(By.Id("result")).Text;

            int lastIndex = -1;
            List<string> eventList = new List<string>() { "mousedown", "focus", "mouseup", "click" };
            foreach (string eventName in eventList)
            {
                int index = text.IndexOf(eventName);

                Assert.That(text, Does.Contain(eventName), eventName + " did not fire at all. Text is " + text);
                Assert.That(index, Is.GreaterThan(lastIndex), eventName + " did not fire in the correct order. Text is " + text);
                lastIndex = index;
            }
        }

        [Test]
        public void ShouldIssueMouseDownEvents()
        {
            driver.Url = javascriptPage;
            driver.FindElement(By.Id("mousedown")).Click();

            String result = driver.FindElement(By.Id("result")).Text;
            Assert.That(result, Is.EqualTo("mouse down"));
        }

        [Test]
        public void ShouldIssueClickEvents()
        {
            driver.Url = javascriptPage;
            driver.FindElement(By.Id("mouseclick")).Click();

            String result = driver.FindElement(By.Id("result")).Text;
            Assert.That(result, Is.EqualTo("mouse click"));
        }

        [Test]
        public void ShouldIssueMouseUpEvents()
        {
            driver.Url = javascriptPage;
            driver.FindElement(By.Id("mouseup")).Click();

            String result = driver.FindElement(By.Id("result")).Text;
            Assert.That(result, Is.EqualTo("mouse up"));
        }

        [Test]
        public void MouseEventsShouldBubbleUpToContainingElements()
        {
            driver.Url = javascriptPage;
            driver.FindElement(By.Id("child")).Click();

            String result = driver.FindElement(By.Id("result")).Text;
            Assert.That(result, Is.EqualTo("mouse down"));
        }

        [Test]
        [IgnoreBrowser(Browser.Firefox)]
        public void ShouldEmitOnChangeEventsWhenSelectingElements()
        {
            driver.Url = javascriptPage;
            //Intentionally not looking up the select tag.  See selenium r7937 for details.
            ReadOnlyCollection<IWebElement> allOptions = driver.FindElements(By.XPath("//select[@id='selector']//option"));

            String initialTextValue = driver.FindElement(By.Id("result")).Text;

            IWebElement foo = allOptions[0];
            IWebElement bar = allOptions[1];

            foo.Click();
            Assert.That(driver.FindElement(By.Id("result")).Text, Is.EqualTo(initialTextValue));
            bar.Click();
            Assert.That(driver.FindElement(By.Id("result")).Text, Is.EqualTo("bar"));
        }

        [Test]
        public void ShouldEmitOnClickEventsWhenSelectingElements()
        {
            driver.Url = javascriptPage;
            // Intentionally not looking up the select tag. See selenium r7937 for details.
            ReadOnlyCollection<IWebElement> allOptions = driver.FindElements(By.XPath("//select[@id='selector2']//option"));

            IWebElement foo = allOptions[0];
            IWebElement bar = allOptions[1];

            foo.Click();
            Assert.That(driver.FindElement(By.Id("result")).Text, Is.EqualTo("foo"));
            bar.Click();
            Assert.That(driver.FindElement(By.Id("result")).Text, Is.EqualTo("bar"));
        }

        [Test]
        [IgnoreBrowser(Browser.IE, "IE does not fire change event when clicking on checkbox")]
        public void ShouldEmitOnChangeEventsWhenChangingTheStateOfACheckbox()
        {
            driver.Url = javascriptPage;
            IWebElement checkbox = driver.FindElement(By.Id("checkbox"));

            checkbox.Click();
            Assert.That(driver.FindElement(By.Id("result")).Text, Is.EqualTo("checkbox thing"));
        }

        [Test]
        public void ShouldEmitClickEventWhenClickingOnATextInputElement()
        {
            driver.Url = javascriptPage;

            IWebElement clicker = driver.FindElement(By.Id("clickField"));
            clicker.Click();

            Assert.That(clicker.GetAttribute("value"), Is.EqualTo("Clicked"));
        }

        [Test]
        public void ShouldFireTwoClickEventsWhenClickingOnALabel()
        {
            driver.Url = javascriptPage;

            driver.FindElement(By.Id("labelForCheckbox")).Click();

            IWebElement result = driver.FindElement(By.Id("result"));
            Assert.That(WaitFor(() => { return result.Text.Contains("labelclick chboxclick"); }, "Did not find text: " + result.Text), Is.True);
        }


        [Test]
        public void ClearingAnElementShouldCauseTheOnChangeHandlerToFire()
        {
            driver.Url = javascriptPage;

            IWebElement element = driver.FindElement(By.Id("clearMe"));
            element.Clear();

            IWebElement result = driver.FindElement(By.Id("result"));
            Assert.That(result.Text.Trim(), Is.EqualTo("Cleared"));
        }

        [Test]
        public void SendingKeysToAnotherElementShouldCauseTheBlurEventToFire()
        {
            driver.Url = javascriptPage;
            IWebElement element = driver.FindElement(By.Id("theworks"));
            element.SendKeys("foo");
            IWebElement element2 = driver.FindElement(By.Id("changeable"));
            element2.SendKeys("bar");
            AssertEventFired("blur", driver);
        }

        [Test]
        [IgnoreBrowser(Browser.Safari, "Safari driver does not support multiple instances")]
        public void SendingKeysToAnotherElementShouldCauseTheBlurEventToFireInNonTopmostWindow()
        {
            IWebElement element = null;
            IWebElement element2 = null;
            IWebDriver driver2 = EnvironmentManager.Instance.CreateDriverInstance();
            try
            {
                // topmost
                driver2.Url = javascriptPage;
                element = driver2.FindElement(By.Id("theworks"));
                element.SendKeys("foo");
                element2 = driver2.FindElement(By.Id("changeable"));
                element2.SendKeys("bar");
                AssertEventFired("blur", driver2);

                // non-topmost
                driver.Url = javascriptPage;
                element = driver.FindElement(By.Id("theworks"));
                element.SendKeys("foo");
                element2 = driver.FindElement(By.Id("changeable"));
                element2.SendKeys("bar");
                AssertEventFired("blur", driver);
            }
            finally
            {
                driver2.Quit();
            }

            driver.Url = javascriptPage;
            element = driver.FindElement(By.Id("theworks"));
            element.SendKeys("foo");
            element2 = driver.FindElement(By.Id("changeable"));
            element2.SendKeys("bar");
            AssertEventFired("blur", driver);
        }

        [Test]
        public void SendingKeysToAnElementShouldCauseTheFocusEventToFire()
        {
            driver.Url = javascriptPage;
            IWebElement element = driver.FindElement(By.Id("theworks"));
            element.SendKeys("foo");
            AssertEventFired("focus", driver);
        }

        [Test]
        public void SendingKeysToAFocusedElementShouldNotBlurThatElement()
        {
            driver.Url = javascriptPage;
            IWebElement element = driver.FindElement(By.Id("theworks"));
            element.Click();

            //Wait until focused
            bool focused = false;
            IWebElement result = driver.FindElement(By.Id("result"));
            for (int i = 0; i < 5; ++i)
            {
                string fired = result.Text;
                if (fired.Contains("focus"))
                {
                    focused = true;
                    break;
                }

                System.Threading.Thread.Sleep(200);
            }

            Assert.That(focused, Is.True, "Clicking on element didn't focus it in time - can't proceed so failing");

            element.SendKeys("a");
            AssertEventNotFired("blur");
        }

        [Test]
        [IgnoreBrowser(Browser.IE, "Clicking on child does blur parent, whether focused or not.")]
        public void ClickingAnUnfocusableChildShouldNotBlurTheParent()
        {
            if (TestUtilities.IsOldIE(driver))
            {
                return;
            }

            driver.Url = javascriptPage;
            // Click on parent, giving it the focus.
            IWebElement parent = driver.FindElement(By.Id("hideOnBlur"));
            parent.Click();
            AssertEventNotFired("blur");
            // Click on child. It is not focusable, so focus should stay on the parent.
            driver.FindElement(By.Id("hideOnBlurChild")).Click();
            System.Threading.Thread.Sleep(2000);
            Assert.That(parent.Displayed, Is.True, "#hideOnBlur should still be displayed after click");
            AssertEventNotFired("blur");
            // Click elsewhere, and let the element disappear.
            driver.FindElement(By.Id("result")).Click();
            AssertEventFired("blur", driver);
        }

        [Test]
        public void SubmittingFormFromFormElementShouldFireOnSubmitForThatForm()
        {
            driver.Url = javascriptPage;
            IWebElement formElement = driver.FindElement(By.Id("submitListeningForm"));
            formElement.Submit();
            AssertEventFired("form-onsubmit", driver);
        }

        [Test]
        public void SubmittingFormFromFormInputSubmitElementShouldFireOnSubmitForThatForm()
        {
            driver.Url = javascriptPage;
            IWebElement submit = driver.FindElement(By.Id("submitListeningForm-submit"));
            submit.Submit();
            AssertEventFired("form-onsubmit", driver);
        }

        [Test]
        public void SubmittingFormFromFormInputTextElementShouldFireOnSubmitForThatFormAndNotClickOnThatInput()
        {
            driver.Url = javascriptPage;
            IWebElement submit = driver.FindElement(By.Id("submitListeningForm-submit"));
            submit.Submit();
            AssertEventFired("form-onsubmit", driver);
            AssertEventNotFired("text-onclick");
        }

        [Test]
        public void UploadingFileShouldFireOnChangeEvent()
        {
            driver.Url = formsPage;
            IWebElement uploadElement = driver.FindElement(By.Id("upload"));
            IWebElement result = driver.FindElement(By.Id("fileResults"));
            Assert.That(result.Text, Is.Empty);

            string filePath = System.IO.Path.Combine(EnvironmentManager.Instance.CurrentDirectory, "test.txt");
            System.IO.FileInfo inputFile = new System.IO.FileInfo(filePath);
            System.IO.StreamWriter inputFileWriter = inputFile.CreateText();
            inputFileWriter.WriteLine("Hello world");
            inputFileWriter.Close();

            uploadElement.SendKeys(inputFile.FullName);
            // Shift focus to something else because send key doesn't make the focus leave
            driver.FindElement(By.Id("id-name1")).Click();

            inputFile.Delete();
            Assert.That(result.Text, Is.EqualTo("changed"));
        }

        [Test]
        public void ShouldReportTheXAndYCoordinatesWhenClicking()
        {
            driver.Url = clickEventPage;

            IWebElement element = driver.FindElement(By.Id("eventish"));
            element.Click();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
            string clientX = driver.FindElement(By.Id("clientX")).Text;
            string clientY = driver.FindElement(By.Id("clientY")).Text;
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0);

            Assert.That(clientX, Is.Not.EqualTo("0"));
            Assert.That(clientY, Is.Not.EqualTo("0"));
        }

        [Test]
        public void ClickEventsShouldBubble()
        {
            driver.Url = clicksPage;
            driver.FindElement(By.Id("bubblesFrom")).Click();
            bool eventBubbled = (bool)((IJavaScriptExecutor)driver).ExecuteScript("return !!window.bubbledClick;");
            Assert.That(eventBubbled, Is.True, "Event didn't bubble up");
        }

        [Test]
        public void ClickOverlappingElements()
        {
            if (TestUtilities.IsOldIE(driver))
            {
                Assert.Ignore("Not supported on IE < 9");
            }

            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("click_tests/overlapping_elements.html");
            Assert.That(() => driver.FindElement(By.Id("under")).Click(), Throws.InstanceOf<ElementClickInterceptedException>().Or.InstanceOf<WebDriverException>().With.Message.Contains("Other element would receive the click"));
        }

        [Test]
        public void ClickAnElementThatDisappear()
        {
            if (TestUtilities.IsOldIE(driver))
            {
                Assert.Ignore("Not supported on IE < 9");
            }

            StringBuilder expectedLogBuilder = new StringBuilder();
            expectedLogBuilder.AppendLine("Log:");
            expectedLogBuilder.AppendLine("mousedown in over (handled by over)");
            expectedLogBuilder.AppendLine("mousedown in over (handled by body)");
            expectedLogBuilder.AppendLine("mouseup in under (handled by under)");
            expectedLogBuilder.Append("mouseup in under (handled by body)");

            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("click_tests/disappearing_element.html");
            driver.FindElement(By.Id("over")).Click();
            Assert.That(driver.FindElement(By.Id("log")).Text, Does.StartWith(expectedLogBuilder.ToString()));
        }

        private void AssertEventNotFired(string eventName)
        {
            IWebElement result = driver.FindElement(By.Id("result"));
            string text = result.Text;
            Assert.That(text, Does.Not.Contain(eventName));
        }

        private void ClickOnElementWhichRecordsEvents(IWebDriver focusedDriver)
        {
            focusedDriver.FindElement(By.Id("plainButton")).Click();
        }

        private void AssertEventFired(string eventName, IWebDriver focusedDriver)
        {
            IWebElement result = focusedDriver.FindElement(By.Id("result"));
            string text = result.Text;
            Assert.That(text, Does.Contain(eventName));
        }
    }
}
