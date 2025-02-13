// <copyright file="FormHandlingTests.cs" company="Selenium Committers">
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
    public class FormHandlingTests : DriverTestFixture
    {
        [Test]
        public void ShouldClickOnSubmitInputElements()
        {
            driver.Url = formsPage;
            driver.FindElement(By.Id("submitButton")).Click();
            WaitFor(TitleToBe("We Arrive Here"), "Browser title is not 'We Arrive Here'");
            Assert.That(driver.Title, Is.EqualTo("We Arrive Here"));
        }

        [Test]
        public void ClickingOnUnclickableElementsDoesNothing()
        {
            driver.Url = formsPage;
            driver.FindElement(By.XPath("//body")).Click();
        }

        [Test]
        public void ShouldBeAbleToClickImageButtons()
        {
            driver.Url = formsPage;
            driver.FindElement(By.Id("imageButton")).Click();
            WaitFor(TitleToBe("We Arrive Here"), "Browser title is not 'We Arrive Here'");
            Assert.That(driver.Title, Is.EqualTo("We Arrive Here"));
        }

        [Test]
        public void ShouldBeAbleToSubmitForms()
        {
            driver.Url = formsPage;
            driver.FindElement(By.Name("login")).Submit();
            WaitFor(TitleToBe("We Arrive Here"), "Browser title is not 'We Arrive Here'");
            Assert.That(driver.Title, Is.EqualTo("We Arrive Here"));
        }

        [Test]
        public void ShouldSubmitAFormWhenAnyInputElementWithinThatFormIsSubmitted()
        {
            driver.Url = formsPage;
            driver.FindElement(By.Id("checky")).Submit();
            WaitFor(TitleToBe("We Arrive Here"), "Browser title is not 'We Arrive Here'");
            Assert.That(driver.Title, Is.EqualTo("We Arrive Here"));
        }

        [Test]
        public void ShouldSubmitAFormWhenAnyElementWithinThatFormIsSubmitted()
        {
            driver.Url = formsPage;
            driver.FindElement(By.XPath("//form/p")).Submit();
            WaitFor(TitleToBe("We Arrive Here"), "Browser title is not 'We Arrive Here'");
            Assert.That(driver.Title, Is.EqualTo("We Arrive Here"));
        }

        [Test]
        public void ShouldSubmitAFormWithIdSubmit()
        {
            driver.Url = formsPage;
            driver.FindElement(By.Id("submit")).Submit();
            WaitFor(TitleToBe("We Arrive Here"), "Browser title is not 'We Arrive Here'");
            Assert.That(driver.Title, Is.EqualTo("We Arrive Here"));
        }

        [Test]
        [IgnoreBrowser(Browser.IE, "Does not work")]
        public void ShouldSubmitAFormWithNameSubmit()
        {
            driver.Url = formsPage;
            driver.FindElement(By.Name("submit")).Submit();
            WaitFor(TitleToBe("We Arrive Here"), "Browser title is not 'We Arrive Here'");
            Assert.That(driver.Title, Is.EqualTo("We Arrive Here"));
        }

        [Test]
        public void ShouldNotBeAbleToSubmitAnInputOutsideAForm()
        {
            driver.Url = formsPage;
            Assert.That(() => driver.FindElement(By.Name("SearchableText")).Submit(), Throws.InstanceOf<WebDriverException>());
        }

        [Test]
        public void ShouldBeAbleToEnterTextIntoATextAreaBySettingItsValue()
        {
            driver.Url = javascriptPage;
            IWebElement textarea = driver.FindElement(By.Id("keyUpArea"));
            string cheesey = "Brie and cheddar";
            textarea.SendKeys(cheesey);
            Assert.That(textarea.GetAttribute("value"), Is.EqualTo(cheesey));
        }

        [Test]
        public void SendKeysKeepsCapitalization()
        {
            driver.Url = javascriptPage;
            IWebElement textarea = driver.FindElement(By.Id("keyUpArea"));
            string cheesey = "BrIe And CheDdar";
            textarea.SendKeys(cheesey);
            Assert.That(textarea.GetAttribute("value"), Is.EqualTo(cheesey));
        }

        [Test]
        [IgnoreBrowser(Browser.Firefox)]
        public void ShouldSubmitAFormUsingTheNewlineLiteral()
        {
            driver.Url = formsPage;
            IWebElement nestedForm = driver.FindElement(By.Id("nested_form"));
            IWebElement input = nestedForm.FindElement(By.Name("x"));
            input.SendKeys("\n");

            WaitFor(TitleToBe("We Arrive Here"), "Browser title is not 'We Arrive Here'");
            Assert.That(driver.Title, Is.EqualTo("We Arrive Here"));
            Assert.That(driver.Url, Does.EndWith("?x=name"));
        }

        [Test]
        public void ShouldSubmitAFormUsingTheEnterKey()
        {
            driver.Url = formsPage;
            IWebElement nestedForm = driver.FindElement(By.Id("nested_form"));
            IWebElement input = nestedForm.FindElement(By.Name("x"));
            input.SendKeys(Keys.Enter);

            WaitFor(TitleToBe("We Arrive Here"), "Browser title is not 'We Arrive Here'");
            Assert.That(driver.Title, Is.EqualTo("We Arrive Here"));
            Assert.That(driver.Url, Does.EndWith("?x=name"));
        }

        [Test]
        public void ShouldEnterDataIntoFormFields()
        {
            driver.Url = xhtmlTestPage;
            IWebElement element = driver.FindElement(By.XPath("//form[@name='someForm']/input[@id='username']"));
            String originalValue = element.GetAttribute("value");
            Assert.That(originalValue, Is.EqualTo("change"));

            element.Clear();
            element.SendKeys("some text");

            element = driver.FindElement(By.XPath("//form[@name='someForm']/input[@id='username']"));
            String newFormValue = element.GetAttribute("value");
            Assert.That(newFormValue, Is.EqualTo("some text"));
        }

        [Test]
        public void ShouldBeAbleToAlterTheContentsOfAFileUploadInputElement()
        {
            string testFileName = string.Format("test-{0}.txt", Guid.NewGuid().ToString("D"));
            driver.Url = formsPage;
            IWebElement uploadElement = driver.FindElement(By.Id("upload"));
            Assert.That(uploadElement.GetAttribute("value"), Is.Null.Or.Empty);

            string filePath = System.IO.Path.Combine(EnvironmentManager.Instance.CurrentDirectory, testFileName);
            System.IO.FileInfo inputFile = new System.IO.FileInfo(filePath);
            System.IO.StreamWriter inputFileWriter = inputFile.CreateText();
            inputFileWriter.WriteLine("Hello world");
            inputFileWriter.Close();

            uploadElement.SendKeys(inputFile.FullName);

            string uploadElementValue = uploadElement.GetAttribute("value");
            System.IO.FileInfo outputFile = new System.IO.FileInfo(uploadElementValue.Replace('\\', System.IO.Path.DirectorySeparatorChar));
            Assert.That(inputFile.Name, Is.EqualTo(outputFile.Name));
            inputFile.Delete();
        }

        [Test]
        public void ShouldBeAbleToSendKeysToAFileUploadInputElementInAnXhtmlDocument()
        {
            // IE before 9 doesn't handle pages served with an XHTML content type, and just prompts for to
            // download it
            if (TestUtilities.IsOldIE(driver))
            {
                return;
            }

            driver.Url = xhtmlFormPage;
            IWebElement uploadElement = driver.FindElement(By.Id("file"));
            Assert.That(uploadElement.GetAttribute("value"), Is.Empty);

            string testFileName = string.Format("test-{0}.txt", Guid.NewGuid().ToString("D"));
            string filePath = System.IO.Path.Combine(EnvironmentManager.Instance.CurrentDirectory, testFileName);
            System.IO.FileInfo inputFile = new System.IO.FileInfo(filePath);
            System.IO.StreamWriter inputFileWriter = inputFile.CreateText();
            inputFileWriter.WriteLine("Hello world");
            inputFileWriter.Close();

            uploadElement.SendKeys(inputFile.FullName);

            string uploadElementValue = uploadElement.GetAttribute("value");
            System.IO.FileInfo outputFile = new System.IO.FileInfo(uploadElementValue.Replace('\\', System.IO.Path.DirectorySeparatorChar));
            Assert.That(outputFile.Name, Is.EqualTo(inputFile.Name));
            inputFile.Delete();
        }

        [Test]
        [IgnoreBrowser(Browser.Safari, "Driver does not allow uploading same file multiple times.")]
        public void ShouldBeAbleToUploadTheSameFileTwice()
        {
            string testFileName = string.Format("test-{0}.txt", Guid.NewGuid().ToString("D"));
            string filePath = System.IO.Path.Combine(EnvironmentManager.Instance.CurrentDirectory, testFileName);
            System.IO.FileInfo inputFile = new System.IO.FileInfo(filePath);
            System.IO.StreamWriter inputFileWriter = inputFile.CreateText();
            inputFileWriter.WriteLine("Hello world");
            inputFileWriter.Close();

            for (int i = 0; i < 2; ++i)
            {
                driver.Url = formsPage;
                IWebElement uploadElement = driver.FindElement(By.Id("upload"));
                Assert.That(uploadElement.GetAttribute("value"), Is.Null.Or.EqualTo(string.Empty));

                uploadElement.SendKeys(inputFile.FullName);
                uploadElement.Submit();

                // Explicitly wait next page to be loaded, Firefox is not handling elements submitting
                WaitFor(() => driver.Url.EndsWith("resultPage.html"), "We are not redirected to the resultPage after submitting web element");
            }

            inputFile.Delete();
            // If we get this far, then we're all good.
        }

        [Test]
        public void SendingKeyboardEventsShouldAppendTextInInputs()
        {
            driver.Url = formsPage;
            IWebElement element = driver.FindElement(By.Id("working"));
            element.SendKeys("Some");
            String value = element.GetAttribute("value");
            Assert.That(value, Is.EqualTo("Some"));

            element.SendKeys(" text");
            value = element.GetAttribute("value");
            Assert.That(value, Is.EqualTo("Some text"));
        }

        [Test]
        public void SendingKeyboardEventsShouldAppendTextInInputsWithExistingValue()
        {
            driver.Url = formsPage;
            IWebElement element = driver.FindElement(By.Id("inputWithText"));
            element.SendKeys(". Some text");
            string value = element.GetAttribute("value");

            Assert.That(value, Is.EqualTo("Example text. Some text"));
        }

        [Test]
        public void SendingKeyboardEventsShouldAppendTextInTextAreas()
        {
            driver.Url = formsPage;
            IWebElement element = driver.FindElement(By.Id("withText"));

            element.SendKeys(". Some text");
            String value = element.GetAttribute("value");

            Assert.That(value, Is.EqualTo("Example text. Some text"));
        }

        [Test]
        public void EmptyTextBoxesShouldReturnAnEmptyStringNotNull()
        {
            driver.Url = formsPage;
            IWebElement emptyTextBox = driver.FindElement(By.Id("working"));
            Assert.That(emptyTextBox.GetAttribute("value"), Is.Empty);

            IWebElement emptyTextArea = driver.FindElement(By.Id("emptyTextArea"));
            Assert.That(emptyTextBox.GetAttribute("value"), Is.Empty);
        }

        [Test]
        public void HandleFormWithJavascriptAction()
        {
            string url = EnvironmentManager.Instance.UrlBuilder.WhereIs("form_handling_js_submit.html");
            driver.Url = url;
            IWebElement element = driver.FindElement(By.Id("theForm"));
            element.Submit();
            IAlert alert = WaitFor<IAlert>(() =>
            {
                try
                {
                    return driver.SwitchTo().Alert();
                }
                catch (NoAlertPresentException)
                {
                    return null;
                }
            }, "No alert found before timeout.");

            string text = alert.Text;
            alert.Dismiss();

            Assert.That(text, Is.EqualTo("Tasty cheese"));
        }

        [Test]
        public void CanClickOnASubmitButton()
        {
            CheckSubmitButton("internal_explicit_submit");
        }

        [Test]
        public void CanClickOnASubmitButtonNestedSpan()
        {
            CheckSubmitButton("internal_span_submit");
        }

        [Test]
        public void CanClickOnAnImplicitSubmitButton()
        {
            CheckSubmitButton("internal_implicit_submit");
        }

        [Test]
        [IgnoreBrowser(Browser.IE, "IE does not support the HTML5 'form' attribute on <button> elements")]
        public void CanClickOnAnExternalSubmitButton()
        {
            CheckSubmitButton("external_explicit_submit");
        }

        [Test]
        [IgnoreBrowser(Browser.IE, "IE does not support the HTML5 'form' attribute on <button> elements")]
        public void CanClickOnAnExternalImplicitSubmitButton()
        {
            CheckSubmitButton("external_implicit_submit");
        }

        [Test]
        [IgnoreTarget("net48", "Cannot create inline page with UrlBuilder")]
        public void CanSubmitFormWithSubmitButtonIdEqualToSubmit()
        {
            string blank = EnvironmentManager.Instance.UrlBuilder.CreateInlinePage(new InlinePage()
                .WithTitle("Submitted Successfully!"));
            driver.Url = EnvironmentManager.Instance.UrlBuilder.CreateInlinePage(new InlinePage()
                .WithBody(string.Format("<form action='{0}'>", blank),
                "  <input type='submit' id='submit' value='Submit'>",
                "</form>"));

            driver.FindElement(By.Id("submit")).Submit();
            WaitFor(TitleToBe("Submitted Successfully!"), "Title was not expected value");
        }

        [Test]
        [IgnoreTarget("net48", "Cannot create inline page with UrlBuilder")]
        public void CanSubmitFormWithSubmitButtonNameEqualToSubmit()
        {
            string blank = EnvironmentManager.Instance.UrlBuilder.CreateInlinePage(new InlinePage()
                .WithTitle("Submitted Successfully!"));
            driver.Url = EnvironmentManager.Instance.UrlBuilder.CreateInlinePage(new InlinePage()
                .WithBody(string.Format("<form action='{0}'>", blank),
                "  <input type='submit' name='submit' value='Submit'>",
                "</form>"));

            driver.FindElement(By.Name("submit")).Submit();
            WaitFor(TitleToBe("Submitted Successfully!"), "Title was not expected value");
        }

        //------------------------------------------------------------------
        // Tests below here are not included in the Java test suite
        //------------------------------------------------------------------
        [Test]
        public void ShouldBeAbleToClearTextFromInputElements()
        {
            driver.Url = formsPage;
            IWebElement element = driver.FindElement(By.Id("working"));
            element.SendKeys("Some text");
            String value = element.GetAttribute("value");
            Assert.That(value, Is.Not.Empty);

            element.Clear();
            value = element.GetAttribute("value");

            Assert.That(value, Is.Empty);
        }

        [Test]
        public void ShouldBeAbleToClearTextFromTextAreas()
        {
            driver.Url = formsPage;
            IWebElement element = driver.FindElement(By.Id("withText"));
            element.SendKeys("Some text");
            String value = element.GetAttribute("value");
            Assert.That(value, Is.Not.Empty);

            element.Clear();
            value = element.GetAttribute("value");

            Assert.That(value, Is.Empty);
        }

        private void CheckSubmitButton(string buttonId)
        {
            driver.Url = EnvironmentManager.Instance.UrlBuilder.WhereIs("click_tests/html5_submit_buttons.html");
            string name = "Gromit";

            driver.FindElement(By.Id("name")).SendKeys(name);
            driver.FindElement(By.Id(buttonId)).Click();

            WaitFor(TitleToBe("Submitted Successfully!"), "Browser title is not 'Submitted Successfully!'");

            Assert.That(driver.Url, Does.Contain("name=" + name), "URL does not contain 'name=" + name + "'. Actual URL:" + driver.Url);
        }

        private Func<bool> TitleToBe(string desiredTitle)
        {
            return () =>
            {
                return driver.Title == desiredTitle;
            };
        }
    }
}
