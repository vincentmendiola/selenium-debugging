// <copyright file="EventFiringWebDriver.cs" company="Selenium Committers">
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading.Tasks;

namespace OpenQA.Selenium.Support.Events
{
    /// <summary>
    /// A wrapper around an arbitrary WebDriver instance which supports registering for
    /// events, e.g. for logging purposes.
    /// </summary>
    public class EventFiringWebDriver : IWebDriver, IJavaScriptExecutor, ITakesScreenshot, IWrapsDriver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventFiringWebDriver"/> class.
        /// </summary>
        /// <param name="parentDriver">The driver to register events for.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="parentDriver"/> is <see langword="null"/>.</exception>
        public EventFiringWebDriver(IWebDriver parentDriver)
        {
            this.WrappedDriver = parentDriver ?? throw new ArgumentNullException(nameof(parentDriver));
        }

        /// <summary>
        /// Fires before the driver begins navigation.
        /// </summary>
        public event EventHandler<WebDriverNavigationEventArgs>? Navigating;

        /// <summary>
        /// Fires after the driver completes navigation
        /// </summary>
        public event EventHandler<WebDriverNavigationEventArgs>? Navigated;

        /// <summary>
        /// Fires before the driver begins navigation back one entry in the browser history list.
        /// </summary>
        public event EventHandler<WebDriverNavigationEventArgs>? NavigatingBack;

        /// <summary>
        /// Fires after the driver completes navigation back one entry in the browser history list.
        /// </summary>
        public event EventHandler<WebDriverNavigationEventArgs>? NavigatedBack;

        /// <summary>
        /// Fires before the driver begins navigation forward one entry in the browser history list.
        /// </summary>
        public event EventHandler<WebDriverNavigationEventArgs>? NavigatingForward;

        /// <summary>
        /// Fires after the driver completes navigation forward one entry in the browser history list.
        /// </summary>
        public event EventHandler<WebDriverNavigationEventArgs>? NavigatedForward;

        /// <summary>
        /// Fires before the driver clicks on an element.
        /// </summary>
        public event EventHandler<WebElementEventArgs>? ElementClicking;

        /// <summary>
        /// Fires after the driver has clicked on an element.
        /// </summary>
        public event EventHandler<WebElementEventArgs>? ElementClicked;

        /// <summary>
        /// Fires before the driver changes the value of an element via Clear(), SendKeys() or Toggle().
        /// </summary>
        public event EventHandler<WebElementValueEventArgs>? ElementValueChanging;

        /// <summary>
        /// Fires after the driver has changed the value of an element via Clear(), SendKeys() or Toggle().
        /// </summary>
        public event EventHandler<WebElementValueEventArgs>? ElementValueChanged;

        /// <summary>
        /// Fires before the driver starts to find an element.
        /// </summary>
        public event EventHandler<FindElementEventArgs>? FindingElement;

        /// <summary>
        /// Fires after the driver completes finding an element.
        /// </summary>
        public event EventHandler<FindElementEventArgs>? FindElementCompleted;

        /// <summary>
        /// Fires before the driver starts to get a shadow root.
        /// </summary>
        public event EventHandler<GetShadowRootEventArgs>? GettingShadowRoot;

        /// <summary>
        /// Fires after the driver completes getting a shadow root.
        /// </summary>
        public event EventHandler<GetShadowRootEventArgs>? GetShadowRootCompleted;

        /// <summary>
        /// Fires before a script is executed.
        /// </summary>
        public event EventHandler<WebDriverScriptEventArgs>? ScriptExecuting;

        /// <summary>
        /// Fires after a script is executed.
        /// </summary>
        public event EventHandler<WebDriverScriptEventArgs>? ScriptExecuted;

        /// <summary>
        /// Fires when an exception is thrown.
        /// </summary>
        public event EventHandler<WebDriverExceptionEventArgs>? ExceptionThrown;

        /// <summary>
        /// Gets the <see cref="IWebDriver"/> wrapped by this EventsFiringWebDriver instance.
        /// </summary>
        public IWebDriver WrappedDriver { get; }

        /// <summary>
        /// Gets or sets the URL the browser is currently displaying.
        /// </summary>
        /// <remarks>
        /// Setting the <see cref="Url"/> property will load a new web page in the current browser window.
        /// This is done using an HTTP GET operation, and the method will block until the
        /// load is complete. This will follow redirects issued either by the server or
        /// as a meta-redirect from within the returned HTML. Should a meta-redirect "rest"
        /// for any duration of time, it is best to wait until this timeout is over, since
        /// should the underlying page change while your test is executing the results of
        /// future calls against this interface will be against the freshly loaded page.
        /// </remarks>
        /// <seealso cref="INavigation.GoToUrl(string)"/>
        /// <seealso cref="INavigation.GoToUrl(System.Uri)"/>
        public string Url
        {
            get
            {
                string url;
                try
                {
                    url = this.WrappedDriver.Url;
                }
                catch (Exception ex)
                {
                    this.OnException(new WebDriverExceptionEventArgs(this.WrappedDriver, ex));
                    throw;
                }

                return url;
            }

            set
            {
                try
                {
                    WebDriverNavigationEventArgs e = new WebDriverNavigationEventArgs(this.WrappedDriver, value);
                    this.OnNavigating(e);
                    this.WrappedDriver.Url = value;
                    this.OnNavigated(e);
                }
                catch (Exception ex)
                {
                    this.OnException(new WebDriverExceptionEventArgs(this.WrappedDriver, ex));
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets the title of the current browser window.
        /// </summary>
        public string Title
        {
            get
            {
                string title;
                try
                {
                    title = this.WrappedDriver.Title;
                }
                catch (Exception ex)
                {
                    this.OnException(new WebDriverExceptionEventArgs(this.WrappedDriver, ex));
                    throw;
                }

                return title;
            }
        }

        /// <summary>
        /// Gets the source of the page last loaded by the browser.
        /// </summary>
        /// <remarks>
        /// If the page has been modified after loading (for example, by JavaScript)
        /// there is no guarantee that the returned text is that of the modified page.
        /// Please consult the documentation of the particular driver being used to
        /// determine whether the returned text reflects the current state of the page
        /// or the text last sent by the web server. The page source returned is a
        /// representation of the underlying DOM: do not expect it to be formatted
        /// or escaped in the same way as the response sent from the web server.
        /// </remarks>
        public string PageSource
        {
            get
            {
                string source;
                try
                {
                    source = this.WrappedDriver.PageSource;
                }
                catch (Exception ex)
                {
                    this.OnException(new WebDriverExceptionEventArgs(this.WrappedDriver, ex));
                    throw;
                }

                return source;
            }
        }

        /// <summary>
        /// Gets the current window handle, which is an opaque handle to this
        /// window that uniquely identifies it within this driver instance.
        /// </summary>
        public string CurrentWindowHandle
        {
            get
            {
                string handle;
                try
                {
                    handle = this.WrappedDriver.CurrentWindowHandle;
                }
                catch (Exception ex)
                {
                    this.OnException(new WebDriverExceptionEventArgs(this.WrappedDriver, ex));
                    throw;
                }

                return handle;
            }
        }

        /// <summary>
        /// Gets the window handles of open browser windows.
        /// </summary>
        public ReadOnlyCollection<string> WindowHandles
        {
            get
            {
                ReadOnlyCollection<string> handles;
                try
                {
                    handles = this.WrappedDriver.WindowHandles;
                }
                catch (Exception ex)
                {
                    this.OnException(new WebDriverExceptionEventArgs(this.WrappedDriver, ex));
                    throw;
                }

                return handles;
            }
        }

        /// <summary>
        /// Close the current window, quitting the browser if it is the last window currently open.
        /// </summary>
        public void Close()
        {
            try
            {
                this.WrappedDriver.Close();
            }
            catch (Exception ex)
            {
                this.OnException(new WebDriverExceptionEventArgs(this.WrappedDriver, ex));
                throw;
            }
        }

        /// <summary>
        /// Quits this driver, closing every associated window.
        /// </summary>
        public void Quit()
        {
            try
            {
                this.WrappedDriver.Quit();
            }
            catch (Exception ex)
            {
                this.OnException(new WebDriverExceptionEventArgs(this.WrappedDriver, ex));
                throw;
            }
        }

        /// <summary>
        /// Instructs the driver to change its settings.
        /// </summary>
        /// <returns>An <see cref="IOptions"/> object allowing the user to change
        /// the settings of the driver.</returns>
        public IOptions Manage()
        {
            return new EventFiringOptions(this);
        }

        /// <summary>
        /// Instructs the driver to navigate the browser to another location.
        /// </summary>
        /// <returns>An <see cref="INavigation"/> object allowing the user to access
        /// the browser's history and to navigate to a given URL.</returns>
        public INavigation Navigate()
        {
            return new EventFiringNavigation(this);
        }

        /// <summary>
        /// Instructs the driver to send future commands to a different frame or window.
        /// </summary>
        /// <returns>An <see cref="ITargetLocator"/> object which can be used to select
        /// a frame or window.</returns>
        public ITargetLocator SwitchTo()
        {
            return new EventFiringTargetLocator(this);
        }

        /// <summary>
        /// Find the first <see cref="IWebElement"/> using the given method.
        /// </summary>
        /// <param name="by">The locating mechanism to use.</param>
        /// <returns>The first matching <see cref="IWebElement"/> on the current context.</returns>
        /// <exception cref="NoSuchElementException">If no element matches the criteria.</exception>
        public IWebElement FindElement(By by)
        {
            IWebElement wrappedElement;
            try
            {
                FindElementEventArgs e = new FindElementEventArgs(this.WrappedDriver, by);
                this.OnFindingElement(e);
                IWebElement element = this.WrappedDriver.FindElement(by);
                this.OnFindElementCompleted(e);
                wrappedElement = this.WrapElement(element);
            }
            catch (Exception ex)
            {
                this.OnException(new WebDriverExceptionEventArgs(this.WrappedDriver, ex));
                throw;
            }

            return wrappedElement;
        }

        /// <summary>
        /// Find all <see cref="IWebElement">IWebElements</see> within the current context
        /// using the given mechanism.
        /// </summary>
        /// <param name="by">The locating mechanism to use.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> of all <see cref="IWebElement">WebElements</see>
        /// matching the current criteria, or an empty list if nothing matches.</returns>
        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            try
            {
                FindElementEventArgs e = new FindElementEventArgs(this.WrappedDriver, by);
                this.OnFindingElement(e);
                ReadOnlyCollection<IWebElement> elements = this.WrappedDriver.FindElements(by);
                this.OnFindElementCompleted(e);

                List<IWebElement> wrappedElementList = new List<IWebElement>(elements.Count);
                foreach (IWebElement element in elements)
                {
                    IWebElement wrappedElement = this.WrapElement(element);
                    wrappedElementList.Add(wrappedElement);
                }
                return wrappedElementList.AsReadOnly();
            }
            catch (Exception ex)
            {
                this.OnException(new WebDriverExceptionEventArgs(this.WrappedDriver, ex));
                throw;
            }

        }

        /// <summary>
        /// Frees all managed and unmanaged resources used by this instance.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Executes JavaScript in the context of the currently selected frame or window.
        /// </summary>
        /// <param name="script">The JavaScript code to execute.</param>
        /// <param name="args">The arguments to the script.</param>
        /// <returns>The value returned by the script.</returns>
        /// <remarks>
        /// <para>
        /// The ExecuteScript method executes JavaScript in the context of
        /// the currently selected frame or window. This means that "document" will refer
        /// to the current document. If the script has a return value, then the following
        /// steps will be taken:
        /// </para>
        /// <para>
        /// <list type="bullet">
        /// <item><description>For an HTML element, this method returns a <see cref="IWebElement"/></description></item>
        /// <item><description>For a number, a <see cref="long"/> is returned</description></item>
        /// <item><description>For a boolean, a <see cref="bool"/> is returned</description></item>
        /// <item><description>For all other cases a <see cref="string"/> is returned.</description></item>
        /// <item><description>For an array,we check the first element, and attempt to return a
        /// <see cref="List{T}"/> of that type, following the rules above. Nested lists are not
        /// supported.</description></item>
        /// <item><description>If the value is null or there is no return value,
        /// <see langword="null"/> is returned.</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// Arguments must be a number (which will be converted to a <see cref="long"/>),
        /// a <see cref="bool"/>, a <see cref="string"/> or a <see cref="IWebElement"/>,
        /// or a <see cref="IWrapsElement"/>.
        /// An exception will be thrown if the arguments do not meet these criteria.
        /// The arguments will be made available to the JavaScript via the "arguments" magic
        /// variable, as if the function were called via "Function.apply"
        /// </para>
        /// </remarks>
        public object ExecuteScript(string script, params object?[] args)
        {
            if (this.WrappedDriver is not IJavaScriptExecutor javascriptDriver)
            {
                throw new NotSupportedException("Underlying driver instance does not support executing JavaScript");
            }

            object scriptResult;
            try
            {
                object?[] unwrappedArgs = UnwrapElementArguments(args);

                WebDriverScriptEventArgs e = new WebDriverScriptEventArgs(this.WrappedDriver, script);
                this.OnScriptExecuting(e);
                scriptResult = javascriptDriver.ExecuteScript(script, unwrappedArgs);
                this.OnScriptExecuted(e);
            }
            catch (Exception ex)
            {
                this.OnException(new WebDriverExceptionEventArgs(this.WrappedDriver, ex));
                throw;
            }

            return scriptResult;
        }


        /// <summary>
        /// Executes JavaScript in the context of the currently selected frame or window.
        /// </summary>
        /// <param name="script">A <see cref="PinnedScript"/> object containing the code to execute.</param>
        /// <param name="args">The arguments to the script.</param>
        /// <returns>The value returned by the script.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="script"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// <para>
        /// The ExecuteScript method executes JavaScript in the context of
        /// the currently selected frame or window. This means that "document" will refer
        /// to the current document. If the script has a return value, then the following
        /// steps will be taken:
        /// </para>
        /// <para>
        /// <list type="bullet">
        /// <item><description>For an HTML element, this method returns a <see cref="IWebElement"/></description></item>
        /// <item><description>For a number, a <see cref="long"/> is returned</description></item>
        /// <item><description>For a boolean, a <see cref="bool"/> is returned</description></item>
        /// <item><description>For all other cases a <see cref="string"/> is returned.</description></item>
        /// <item><description>For an array,we check the first element, and attempt to return a
        /// <see cref="List{T}"/> of that type, following the rules above. Nested lists are not
        /// supported.</description></item>
        /// <item><description>If the value is null or there is no return value,
        /// <see langword="null"/> is returned.</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// Arguments must be a number (which will be converted to a <see cref="long"/>),
        /// a <see cref="bool"/>, a <see cref="string"/> or a <see cref="IWebElement"/>,
        /// or a <see cref="IWrapsElement"/>.
        /// An exception will be thrown if the arguments do not meet these criteria.
        /// The arguments will be made available to the JavaScript via the "arguments" magic
        /// variable, as if the function were called via "Function.apply"
        /// </para>
        /// </remarks>
        public object ExecuteScript(PinnedScript script, params object?[] args)
        {
            if (script == null)
            {
                throw new ArgumentNullException(nameof(script));
            }

            if (this.WrappedDriver is not IJavaScriptExecutor javascriptDriver)
            {
                throw new NotSupportedException("Underlying driver instance does not support executing JavaScript");
            }

            object scriptResult;
            try
            {
                object?[] unwrappedArgs = UnwrapElementArguments(args);

                WebDriverScriptEventArgs e = new WebDriverScriptEventArgs(this.WrappedDriver, script.Source);
                this.OnScriptExecuting(e);
                scriptResult = javascriptDriver.ExecuteScript(script, unwrappedArgs);
                this.OnScriptExecuted(e);
            }
            catch (Exception ex)
            {
                this.OnException(new WebDriverExceptionEventArgs(this.WrappedDriver, ex));
                throw;
            }

            return scriptResult;
        }

        /// <summary>
        /// Executes JavaScript asynchronously in the context of the currently selected frame or window.
        /// </summary>
        /// <param name="script">The JavaScript code to execute.</param>
        /// <param name="args">The arguments to the script.</param>
        /// <returns>The value returned by the script.</returns>
        public object ExecuteAsyncScript(string script, params object?[] args)
        {
            if (this.WrappedDriver is not IJavaScriptExecutor javascriptDriver)
            {
                throw new NotSupportedException("Underlying driver instance does not support executing JavaScript");
            }

            object scriptResult;
            try
            {
                object?[] unwrappedArgs = UnwrapElementArguments(args);

                WebDriverScriptEventArgs e = new WebDriverScriptEventArgs(this.WrappedDriver, script);
                this.OnScriptExecuting(e);
                scriptResult = javascriptDriver.ExecuteAsyncScript(script, unwrappedArgs);
                this.OnScriptExecuted(e);
            }
            catch (Exception ex)
            {
                this.OnException(new WebDriverExceptionEventArgs(this.WrappedDriver, ex));
                throw;
            }

            return scriptResult;
        }

        /// <summary>
        /// Gets a <see cref="Screenshot"/> object representing the image of the page on the screen.
        /// </summary>
        /// <returns>A <see cref="Screenshot"/> object containing the image.</returns>
        public Screenshot GetScreenshot()
        {
            if (this.WrappedDriver is not ITakesScreenshot screenshotDriver)
            {
                throw new NotSupportedException("Underlying driver instance does not support taking screenshots");
            }

            return screenshotDriver.GetScreenshot();
        }

        /// <summary>
        /// Frees all managed and, optionally, unmanaged resources used by this instance.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to dispose of only managed resources;
        /// <see langword="false"/> to dispose of managed and unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.WrappedDriver.Dispose();
            }
        }

        /// <summary>
        /// Raises the <see cref="Navigating"/> event.
        /// </summary>
        /// <param name="e">A <see cref="WebDriverNavigationEventArgs"/> that contains the event data.</param>
        protected virtual void OnNavigating(WebDriverNavigationEventArgs e)
        {
            if (this.Navigating != null)
            {
                this.Navigating(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="Navigated"/> event.
        /// </summary>
        /// <param name="e">A <see cref="WebDriverNavigationEventArgs"/> that contains the event data.</param>
        protected virtual void OnNavigated(WebDriverNavigationEventArgs e)
        {
            if (this.Navigated != null)
            {
                this.Navigated(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="NavigatingBack"/> event.
        /// </summary>
        /// <param name="e">A <see cref="WebDriverNavigationEventArgs"/> that contains the event data.</param>
        protected virtual void OnNavigatingBack(WebDriverNavigationEventArgs e)
        {
            if (this.NavigatingBack != null)
            {
                this.NavigatingBack(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="NavigatedBack"/> event.
        /// </summary>
        /// <param name="e">A <see cref="WebDriverNavigationEventArgs"/> that contains the event data.</param>
        protected virtual void OnNavigatedBack(WebDriverNavigationEventArgs e)
        {
            if (this.NavigatedBack != null)
            {
                this.NavigatedBack(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="NavigatingForward"/> event.
        /// </summary>
        /// <param name="e">A <see cref="WebDriverNavigationEventArgs"/> that contains the event data.</param>
        protected virtual void OnNavigatingForward(WebDriverNavigationEventArgs e)
        {
            if (this.NavigatingForward != null)
            {
                this.NavigatingForward(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="NavigatedForward"/> event.
        /// </summary>
        /// <param name="e">A <see cref="WebDriverNavigationEventArgs"/> that contains the event data.</param>
        protected virtual void OnNavigatedForward(WebDriverNavigationEventArgs e)
        {
            if (this.NavigatedForward != null)
            {
                this.NavigatedForward(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ElementClicking"/> event.
        /// </summary>
        /// <param name="e">A <see cref="WebElementEventArgs"/> that contains the event data.</param>
        protected virtual void OnElementClicking(WebElementEventArgs e)
        {
            if (this.ElementClicking != null)
            {
                this.ElementClicking(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ElementClicked"/> event.
        /// </summary>
        /// <param name="e">A <see cref="WebElementEventArgs"/> that contains the event data.</param>
        protected virtual void OnElementClicked(WebElementEventArgs e)
        {
            if (this.ElementClicked != null)
            {
                this.ElementClicked(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ElementValueChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="WebElementValueEventArgs"/> that contains the event data.</param>
        protected virtual void OnElementValueChanging(WebElementValueEventArgs e)
        {
            if (this.ElementValueChanging != null)
            {
                this.ElementValueChanging(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ElementValueChanged"/> event.
        /// </summary>
        /// <param name="e">A <see cref="WebElementValueEventArgs"/> that contains the event data.</param>
        protected virtual void OnElementValueChanged(WebElementValueEventArgs e)
        {
            if (this.ElementValueChanged != null)
            {
                this.ElementValueChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="FindingElement"/> event.
        /// </summary>
        /// <param name="e">A <see cref="FindElementEventArgs"/> that contains the event data.</param>
        protected virtual void OnFindingElement(FindElementEventArgs e)
        {
            if (this.FindingElement != null)
            {
                this.FindingElement(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="FindElementCompleted"/> event.
        /// </summary>
        /// <param name="e">A <see cref="FindElementEventArgs"/> that contains the event data.</param>
        protected virtual void OnFindElementCompleted(FindElementEventArgs e)
        {
            if (this.FindElementCompleted != null)
            {
                this.FindElementCompleted(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="OnGettingShadowRoot"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GetShadowRootEventArgs"/> that contains the event data.</param>
        protected virtual void OnGettingShadowRoot(GetShadowRootEventArgs e)
        {
            if (this.GettingShadowRoot != null)
            {
                this.GettingShadowRoot(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="OnGetShadowRootCompleted"/> event.
        /// </summary>
        /// <param name="e">A <see cref="GetShadowRootEventArgs"/> that contains the event data.</param>
        protected virtual void OnGetShadowRootCompleted(GetShadowRootEventArgs e)
        {
            if (this.GetShadowRootCompleted != null)
            {
                this.GetShadowRootCompleted(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ScriptExecuting"/> event.
        /// </summary>
        /// <param name="e">A <see cref="WebDriverScriptEventArgs"/> that contains the event data.</param>
        protected virtual void OnScriptExecuting(WebDriverScriptEventArgs e)
        {
            if (this.ScriptExecuting != null)
            {
                this.ScriptExecuting(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ScriptExecuted"/> event.
        /// </summary>
        /// <param name="e">A <see cref="WebDriverScriptEventArgs"/> that contains the event data.</param>
        protected virtual void OnScriptExecuted(WebDriverScriptEventArgs e)
        {
            if (this.ScriptExecuted != null)
            {
                this.ScriptExecuted(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="ExceptionThrown"/> event.
        /// </summary>
        /// <param name="e">A <see cref="WebDriverExceptionEventArgs"/> that contains the event data.</param>
        protected virtual void OnException(WebDriverExceptionEventArgs e)
        {
            if (this.ExceptionThrown != null)
            {
                this.ExceptionThrown(this, e);
            }
        }

        private static object?[] UnwrapElementArguments(object?[] args)
        {
            if (args is null)
            {
                throw new InvalidOperationException("Cannot unwrap null args");
            }

            // Walk the args: the various drivers expect unwrapped versions of the elements
            List<object?> unwrappedArgs = new List<object?>(args.Length);
            foreach (object? arg in args)
            {
                if (arg is IWrapsElement eventElementArg)
                {
                    unwrappedArgs.Add(eventElementArg.WrappedElement);
                }
                else
                {
                    unwrappedArgs.Add(arg);
                }
            }

            return unwrappedArgs.ToArray();
        }

        private IWebElement WrapElement(IWebElement underlyingElement)
        {
            return new EventFiringWebElement(this, underlyingElement);
        }

        /// <summary>
        /// Provides a mechanism for Navigating with the driver.
        /// </summary>
        private class EventFiringNavigation : INavigation
        {
            private readonly EventFiringWebDriver parentDriver;
            private readonly INavigation wrappedNavigation;

            /// <summary>
            /// Initializes a new instance of the <see cref="EventFiringNavigation"/> class
            /// </summary>
            /// <param name="driver">Driver in use</param>
            public EventFiringNavigation(EventFiringWebDriver driver)
            {
                this.parentDriver = driver ?? throw new ArgumentNullException(nameof(driver));
                this.wrappedNavigation = this.parentDriver.WrappedDriver.Navigate();
            }

            /// <summary>
            /// Move the browser back
            /// </summary>
            public void Back()
            {
                Task.Run(async delegate
                {
                    await this.BackAsync();
                }).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Move the browser back as an asynchronous task.
            /// </summary>
            /// <returns>A task object representing the asynchronous operation</returns>
            public async Task BackAsync()
            {
                try
                {
                    WebDriverNavigationEventArgs e = new WebDriverNavigationEventArgs(this.parentDriver);

                    this.parentDriver.OnNavigatingBack(e);
                    await this.wrappedNavigation.BackAsync().ConfigureAwait(false);
                    this.parentDriver.OnNavigatedBack(e);
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }
            }

            /// <summary>
            /// Move a single "item" forward in the browser's history.
            /// </summary>
            public void Forward()
            {
                Task.Run(async delegate
                {
                    await this.ForwardAsync();
                }).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Move a single "item" forward in the browser's history as an asynchronous task.
            /// </summary>
            /// <returns>A task object representing the asynchronous operation.</returns>
            public async Task ForwardAsync()
            {
                try
                {
                    WebDriverNavigationEventArgs e = new WebDriverNavigationEventArgs(this.parentDriver);
                    this.parentDriver.OnNavigatingForward(e);
                    await this.wrappedNavigation.ForwardAsync().ConfigureAwait(false);
                    this.parentDriver.OnNavigatedForward(e);
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }
            }

            /// <summary>
            /// Navigate to a url.
            /// </summary>
            /// <param name="url">String of where you want the browser to go to</param>
            public void GoToUrl(string url)
            {
                Task.Run(async delegate
                {
                    await this.GoToUrlAsync(url);
                }).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Navigate to a url as an asynchronous task.
            /// </summary>
            /// <param name="url">String of where you want the browser to go.</param>
            /// <returns>A task object representing the asynchronous operation.</returns>
            public async Task GoToUrlAsync(string url)
            {
                if (url == null)
                {
                    throw new ArgumentNullException(nameof(url), "url cannot be null");
                }

                try
                {
                    WebDriverNavigationEventArgs e = new WebDriverNavigationEventArgs(this.parentDriver, url);
                    this.parentDriver.OnNavigating(e);
                    await this.wrappedNavigation.GoToUrlAsync(url).ConfigureAwait(false);
                    this.parentDriver.OnNavigated(e);
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }
            }

            /// <summary>
            /// Navigate to a url.
            /// </summary>
            /// <param name="url">Uri object of where you want the browser to go to</param>
            public void GoToUrl(Uri url)
            {
                Task.Run(async delegate
                {
                    await this.GoToUrlAsync(url);
                }).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Navigate to a url as an asynchronous task.
            /// </summary>
            /// <param name="url">Uri object of where you want the browser to go.</param>
            /// <returns>A task object representing the asynchronous operation.</returns>
            public async Task GoToUrlAsync(Uri url)
            {
                if (url == null)
                {
                    throw new ArgumentNullException(nameof(url), "url cannot be null");
                }

                await this.GoToUrlAsync(url.ToString()).ConfigureAwait(false);
            }

            /// <summary>
            /// Reload the current page.
            /// </summary>
            public void Refresh()
            {
                Task.Run(async delegate
                {
                    await this.RefreshAsync();
                }).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Reload the current page as an asynchronous task.
            /// </summary>
            /// <returns>A task object representing the asynchronous operation.</returns>
            public async Task RefreshAsync()
            {
                try
                {
                    await this.wrappedNavigation.RefreshAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }
            }
        }

        /// <summary>
        /// Provides a mechanism for setting options needed for the driver during the test.
        /// </summary>
        private class EventFiringOptions : IOptions
        {
            private readonly IOptions wrappedOptions;

            /// <summary>
            /// Initializes a new instance of the <see cref="EventFiringOptions"/> class
            /// </summary>
            /// <param name="driver">Instance of the driver currently in use</param>
            public EventFiringOptions(EventFiringWebDriver driver)
            {
                this.wrappedOptions = driver.WrappedDriver.Manage();
            }

            /// <summary>
            /// Gets an object allowing the user to manipulate cookies on the page.
            /// </summary>
            public ICookieJar Cookies => this.wrappedOptions.Cookies;

            /// <summary>
            /// Gets an object allowing the user to manipulate the currently-focused browser window.
            /// </summary>
            /// <remarks>"Currently-focused" is defined as the browser window having the window handle
            /// returned when IWebDriver.CurrentWindowHandle is called.</remarks>
            public IWindow Window => this.wrappedOptions.Window;

            public ILogs Logs => this.wrappedOptions.Logs;

            public INetwork Network => this.wrappedOptions.Network;

            /// <summary>
            /// Provides access to the timeouts defined for this driver.
            /// </summary>
            /// <returns>An object implementing the <see cref="ITimeouts"/> interface.</returns>
            public ITimeouts Timeouts()
            {
                return new EventFiringTimeouts(this.wrappedOptions);
            }
        }

        /// <summary>
        /// Provides a mechanism for finding elements on the page with locators.
        /// </summary>
        private class EventFiringTargetLocator : ITargetLocator
        {
            private readonly ITargetLocator wrappedLocator;
            private readonly EventFiringWebDriver parentDriver;

            /// <summary>
            /// Initializes a new instance of the <see cref="EventFiringTargetLocator"/> class
            /// </summary>
            /// <param name="driver">The driver that is currently in use</param>
            public EventFiringTargetLocator(EventFiringWebDriver driver)
            {
                this.parentDriver = driver ?? throw new ArgumentNullException(nameof(driver));
                this.wrappedLocator = this.parentDriver.WrappedDriver.SwitchTo();
            }

            /// <summary>
            /// Move to a different frame using its index
            /// </summary>
            /// <param name="frameIndex">The index of the </param>
            /// <returns>A WebDriver instance that is currently in use</returns>
            public IWebDriver Frame(int frameIndex)
            {
                IWebDriver driver;
                try
                {
                    driver = this.wrappedLocator.Frame(frameIndex);
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }

                return driver;
            }

            /// <summary>
            /// Move to different frame using its name
            /// </summary>
            /// <param name="frameName">name of the frame</param>
            /// <returns>A WebDriver instance that is currently in use</returns>
            public IWebDriver Frame(string frameName)
            {
                IWebDriver driver;
                try
                {
                    driver = this.wrappedLocator.Frame(frameName);
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }

                return driver;
            }

            /// <summary>
            /// Move to a frame element.
            /// </summary>
            /// <param name="frameElement">a previously found FRAME or IFRAME element.</param>
            /// <returns>A WebDriver instance that is currently in use.</returns>
            public IWebDriver Frame(IWebElement frameElement)
            {
                IWebDriver driver;
                try
                {
                    IWrapsElement wrapper = (IWrapsElement)frameElement;
                    driver = this.wrappedLocator.Frame(wrapper.WrappedElement);
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }

                return driver;
            }

            /// <summary>
            /// Select the parent frame of the currently selected frame.
            /// </summary>
            /// <returns>An <see cref="IWebDriver"/> instance focused on the specified frame.</returns>
            public IWebDriver ParentFrame()
            {
                IWebDriver driver;
                try
                {
                    driver = this.wrappedLocator.ParentFrame();
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }

                return driver;
            }

            /// <summary>
            /// Change to the Window by passing in the name
            /// </summary>
            /// <param name="windowName">name of the window that you wish to move to</param>
            /// <returns>A WebDriver instance that is currently in use</returns>
            public IWebDriver Window(string windowName)
            {
                IWebDriver driver;
                try
                {
                    driver = this.wrappedLocator.Window(windowName);
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }

                return driver;
            }

            /// <summary>
            /// Creates a new browser window and switches the focus for future commands
            /// of this driver to the new window.
            /// </summary>
            /// <param name="typeHint">The type of new browser window to be created.
            /// The created window is not guaranteed to be of the requested type; if
            /// the driver does not support the requested type, a new browser window
            /// will be created of whatever type the driver does support.</param>
            /// <returns>An <see cref="IWebDriver"/> instance focused on the new browser.</returns>
            public IWebDriver NewWindow(WindowType typeHint)
            {
                IWebDriver driver;
                try
                {
                    driver = this.wrappedLocator.NewWindow(typeHint);
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }

                return driver;
            }

            /// <summary>
            /// Change the active frame to the default
            /// </summary>
            /// <returns>Element of the default</returns>
            public IWebDriver DefaultContent()
            {
                IWebDriver driver;
                try
                {
                    driver = this.wrappedLocator.DefaultContent();
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }

                return driver;
            }

            /// <summary>
            /// Finds the active element on the page and returns it
            /// </summary>
            /// <returns>Element that is active</returns>
            public IWebElement ActiveElement()
            {
                IWebElement element;
                try
                {
                    element = this.wrappedLocator.ActiveElement();
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }

                return element;
            }

            /// <summary>
            /// Switches to the currently active modal dialog for this particular driver instance.
            /// </summary>
            /// <returns>A handle to the dialog.</returns>
            public IAlert Alert()
            {
                IAlert alert;
                try
                {
                    alert = this.wrappedLocator.Alert();
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }

                return alert;
            }
        }

        /// <summary>
        /// Defines the interface through which the user can define timeouts.
        /// </summary>
        private class EventFiringTimeouts : ITimeouts
        {
            private readonly ITimeouts wrappedTimeouts;

            /// <summary>
            /// Initializes a new instance of the <see cref="EventFiringTimeouts"/> class
            /// </summary>
            /// <param name="options">The <see cref="IOptions"/> object to wrap.</param>
            public EventFiringTimeouts(IOptions options)
            {
                this.wrappedTimeouts = options.Timeouts();
            }

            /// <summary>
            /// Gets or sets the implicit wait timeout, which is the  amount of time the
            /// driver should wait when searching for an element if it is not immediately
            /// present.
            /// </summary>
            /// <remarks>
            /// When searching for a single element, the driver should poll the page
            /// until the element has been found, or this timeout expires before throwing
            /// a <see cref="NoSuchElementException"/>. When searching for multiple elements,
            /// the driver should poll the page until at least one element has been found
            /// or this timeout has expired.
            /// <para>
            /// Increasing the implicit wait timeout should be used judiciously as it
            /// will have an adverse effect on test run time, especially when used with
            /// slower location strategies like XPath.
            /// </para>
            /// </remarks>
            public TimeSpan ImplicitWait
            {
                get => this.wrappedTimeouts.ImplicitWait;
                set => this.wrappedTimeouts.ImplicitWait = value;
            }

            /// <summary>
            /// Gets or sets the asynchronous script timeout, which is the amount
            /// of time the driver should wait when executing JavaScript asynchronously.
            /// This timeout only affects the <see cref="IJavaScriptExecutor.ExecuteAsyncScript(string, object[])"/>
            /// method.
            /// </summary>
            public TimeSpan AsynchronousJavaScript
            {
                get => this.wrappedTimeouts.AsynchronousJavaScript;
                set => this.wrappedTimeouts.AsynchronousJavaScript = value;
            }

            /// <summary>
            /// Gets or sets the page load timeout, which is the amount of time the driver
            /// should wait for a page to load when setting the <see cref="IWebDriver.Url"/>
            /// property.
            /// </summary>
            public TimeSpan PageLoad
            {
                get => this.wrappedTimeouts.PageLoad;
                set => this.wrappedTimeouts.PageLoad = value;
            }
        }

        /// <summary>
        /// EventFiringWebElement allows you to have access to specific items that are found on the page
        /// </summary>
        private class EventFiringWebElement : ITakesScreenshot, IWebElement, IWrapsElement, IWrapsDriver
        {
            private readonly EventFiringWebDriver parentDriver;

            /// <summary>
            /// Initializes a new instance of the <see cref="EventFiringWebElement"/> class.
            /// </summary>
            /// <param name="driver">The <see cref="EventFiringWebDriver"/> instance hosting this element.</param>
            /// <param name="element">The <see cref="IWebElement"/> to wrap for event firing.</param>
            public EventFiringWebElement(EventFiringWebDriver driver, IWebElement element)
            {
                this.WrappedElement = element ?? throw new ArgumentNullException(nameof(element));
                this.parentDriver = driver ?? throw new ArgumentNullException(nameof(driver));
            }

            /// <summary>
            /// Gets the underlying wrapped <see cref="IWebElement"/>.
            /// </summary>
            public IWebElement WrappedElement { get; }

            /// <summary>
            /// Gets the underlying parent wrapped <see cref="IWebDriver"/>
            /// </summary>
            public IWebDriver WrappedDriver => this.parentDriver;

            /// <summary>
            /// Gets the DOM Tag of element
            /// </summary>
            public string TagName
            {
                get
                {
                    string tagName;
                    try
                    {
                        tagName = this.WrappedElement.TagName;
                    }
                    catch (Exception ex)
                    {
                        this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                        throw;
                    }

                    return tagName;
                }
            }

            /// <summary>
            /// Gets the text from the element
            /// </summary>
            public string Text
            {
                get
                {
                    string text;
                    try
                    {
                        text = this.WrappedElement.Text;
                    }
                    catch (Exception ex)
                    {
                        this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                        throw;
                    }

                    return text;
                }
            }

            /// <summary>
            /// Gets a value indicating whether an element is currently enabled
            /// </summary>
            public bool Enabled
            {
                get
                {
                    bool enabled;
                    try
                    {
                        enabled = this.WrappedElement.Enabled;
                    }
                    catch (Exception ex)
                    {
                        this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                        throw;
                    }

                    return enabled;
                }
            }

            /// <summary>
            /// Gets a value indicating whether this element is selected or not. This operation only applies to input elements such as checkboxes, options in a select and radio buttons.
            /// </summary>
            public bool Selected
            {
                get
                {
                    bool selected;
                    try
                    {
                        selected = this.WrappedElement.Selected;
                    }
                    catch (Exception ex)
                    {
                        this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                        throw;
                    }

                    return selected;
                }
            }

            /// <summary>
            /// Gets the Location of an element and returns a Point object
            /// </summary>
            public Point Location
            {
                get
                {
                    Point location;
                    try
                    {
                        location = this.WrappedElement.Location;
                    }
                    catch (Exception ex)
                    {
                        this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                        throw;
                    }

                    return location;
                }
            }

            /// <summary>
            /// Gets the <see cref="Size"/> of the element on the page
            /// </summary>
            public Size Size
            {
                get
                {
                    Size size;
                    try
                    {
                        size = this.WrappedElement.Size;
                    }
                    catch (Exception ex)
                    {
                        this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                        throw;
                    }

                    return size;
                }
            }

            /// <summary>
            /// Gets a value indicating whether the element is currently being displayed
            /// </summary>
            public bool Displayed
            {
                get
                {
                    bool displayed;
                    try
                    {
                        displayed = this.WrappedElement.Displayed;
                    }
                    catch (Exception ex)
                    {
                        this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                        throw;
                    }

                    return displayed;
                }
            }

            /// <summary>
            /// Gets the underlying EventFiringWebDriver for this element.
            /// </summary>
            protected EventFiringWebDriver ParentDriver => this.parentDriver;

            /// <summary>
            /// Method to clear the text out of an Input element
            /// </summary>
            public void Clear()
            {
                try
                {
                    WebElementValueEventArgs e = new WebElementValueEventArgs(this.parentDriver.WrappedDriver, this.WrappedElement, null);
                    this.parentDriver.OnElementValueChanging(e);
                    this.WrappedElement.Clear();
                    this.parentDriver.OnElementValueChanged(e);
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }
            }

            /// <summary>
            /// Method for sending native key strokes to the browser
            /// </summary>
            /// <param name="text">String containing what you would like to type onto the screen</param>
            public void SendKeys(string text)
            {
                try
                {
                    WebElementValueEventArgs e = new WebElementValueEventArgs(this.parentDriver.WrappedDriver, this.WrappedElement, text);
                    this.parentDriver.OnElementValueChanging(e);
                    this.WrappedElement.SendKeys(text);
                    this.parentDriver.OnElementValueChanged(e);
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }
            }

            /// <summary>
            /// If this current element is a form, or an element within a form, then this will be submitted to the remote server.
            /// If this causes the current page to change, then this method will block until the new page is loaded.
            /// </summary>
            public void Submit()
            {
                try
                {
                    this.WrappedElement.Submit();
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }
            }

            /// <summary>
            /// Click this element. If this causes a new page to load, this method will block until
            /// the page has loaded. At this point, you should discard all references to this element
            /// and any further operations performed on this element will have undefined behavior unless
            /// you know that the element and the page will still be present. If this element is not
            /// clickable, then this operation is a no-op since it's pretty common for someone to
            /// accidentally miss  the target when clicking in Real Life
            /// </summary>
            public void Click()
            {
                try
                {
                    WebElementEventArgs e = new WebElementEventArgs(this.parentDriver.WrappedDriver, this.WrappedElement);
                    this.parentDriver.OnElementClicking(e);
                    this.WrappedElement.Click();
                    this.parentDriver.OnElementClicked(e);
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }
            }

            /// <summary>
            /// If this current element is a form, or an element within a form, then this will be submitted to the remote server. If this causes the current page to change, then this method will block until the new page is loaded.
            /// </summary>
            /// <param name="attributeName">Attribute you wish to get details of</param>
            /// <returns>The attribute's current value or null if the value is not set.</returns>
            public string GetAttribute(string attributeName)
            {
                string attribute;
                try
                {
                    attribute = this.WrappedElement.GetAttribute(attributeName);
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }

                return attribute;
            }

            /// <summary>
            /// Gets the value of a declared HTML attribute of this element.
            /// </summary>
            /// <param name="attributeName">The name of the HTML attribute to get the value of.</param>
            /// <returns>The HTML attribute's current value. Returns a <see langword="null"/> if the
            /// value is not set or the declared attribute does not exist.</returns>
            /// <remarks>
            /// As opposed to the <see cref="GetAttribute(string)"/> method, this method
            /// only returns attributes declared in the element's HTML markup. To access the value
            /// of an IDL property of the element, either use the <see cref="GetAttribute(string)"/>
            /// method or the <see cref="GetDomProperty(string)"/> method.
            /// </remarks>
            public string GetDomAttribute(string attributeName)
            {
                string attribute;
                try
                {
                    attribute = this.WrappedElement.GetDomAttribute(attributeName);
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }

                return attribute;
            }

            /// <summary>
            /// Gets the value of a JavaScript property of this element.
            /// </summary>
            /// <param name="propertyName">The name of the JavaScript property to get the value of.</param>
            /// <returns>The JavaScript property's current value. Returns a <see langword="null"/> if the
            /// value is not set or the property does not exist.</returns>
            public string GetDomProperty(string propertyName)
            {
                string elementProperty;
                try
                {
                    elementProperty = this.WrappedElement.GetDomProperty(propertyName);
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }

                return elementProperty;
            }

            /// <summary>
            /// Method to return the value of a CSS Property
            /// </summary>
            /// <param name="propertyName">CSS property key</param>
            /// <returns>string value of the CSS property</returns>
            public string GetCssValue(string propertyName)
            {
                string cssValue;
                try
                {
                    cssValue = this.WrappedElement.GetCssValue(propertyName);
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }

                return cssValue;
            }

            /// <summary>
            /// Gets the representation of an element's shadow root for accessing the shadow DOM of a web component.
            /// </summary>
            /// <exception cref="NoSuchShadowRootException">Thrown when this element does not have a shadow root.</exception>
            /// <returns>A shadow root representation.</returns>
            public ISearchContext GetShadowRoot()
            {
                ISearchContext shadowRoot;
                try
                {
                    GetShadowRootEventArgs e = new GetShadowRootEventArgs(this.parentDriver.WrappedDriver, this.WrappedElement);
                    this.parentDriver.OnGettingShadowRoot(e);
                    shadowRoot = this.WrappedElement.GetShadowRoot();
                    this.parentDriver.OnGetShadowRootCompleted(e);
                    shadowRoot = new EventFiringShadowRoot(this.parentDriver, shadowRoot);
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }

                return shadowRoot;
            }

            /// <summary>
            /// Finds the first element in the page that matches the <see cref="By"/> object
            /// </summary>
            /// <param name="by">By mechanism to find the element</param>
            /// <returns>IWebElement object so that you can interaction that object</returns>
            public IWebElement FindElement(By by)
            {
                IWebElement wrappedElement;
                try
                {
                    FindElementEventArgs e = new FindElementEventArgs(this.parentDriver.WrappedDriver, this.WrappedElement, by);
                    this.parentDriver.OnFindingElement(e);
                    IWebElement element = this.WrappedElement.FindElement(by);
                    this.parentDriver.OnFindElementCompleted(e);
                    wrappedElement = this.parentDriver.WrapElement(element);
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }

                return wrappedElement;
            }

            /// <summary>
            /// Finds the elements on the page by using the <see cref="By"/> object and returns a ReadOnlyCollection of the Elements on the page
            /// </summary>
            /// <param name="by">By mechanism to find the element</param>
            /// <returns>ReadOnlyCollection of IWebElement</returns>
            public ReadOnlyCollection<IWebElement> FindElements(By by)
            {
                try
                {
                    FindElementEventArgs e = new FindElementEventArgs(this.parentDriver.WrappedDriver, this.WrappedElement, by);
                    this.parentDriver.OnFindingElement(e);
                    ReadOnlyCollection<IWebElement> elements = this.WrappedElement.FindElements(by);
                    this.parentDriver.OnFindElementCompleted(e);

                    List<IWebElement> wrappedElementList = new List<IWebElement>(elements.Count);
                    foreach (IWebElement element in elements)
                    {
                        IWebElement wrappedElement = this.parentDriver.WrapElement(element);
                        wrappedElementList.Add(wrappedElement);
                    }

                    return wrappedElementList.AsReadOnly();
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }
            }

            /// <summary>
            /// Gets a <see cref="Screenshot"/> object representing the image of the page on the screen.
            /// </summary>
            /// <returns>A <see cref="Screenshot"/> object containing the image.</returns>
            public Screenshot GetScreenshot()
            {
                if (this.WrappedElement is not ITakesScreenshot screenshotDriver)
                {
                    throw new NotSupportedException("Underlying element instance does not support taking screenshots");
                }

                return screenshotDriver.GetScreenshot();
            }

            /// <summary>
            /// Determines whether the specified <see cref="EventFiringWebElement"/> is equal to the current <see cref="EventFiringWebElement"/>.
            /// </summary>
            /// <param name="obj">The <see cref="EventFiringWebElement"/> to compare to the current <see cref="EventFiringWebElement"/>.</param>
            /// <returns><see langword="true"/> if the specified <see cref="EventFiringWebElement"/> is equal to the current <see cref="EventFiringWebElement"/>; otherwise, <see langword="false"/>.</returns>
            public override bool Equals(object obj)
            {
                if (obj is not IWebElement other)
                {
                    return false;
                }

                if (other is IWrapsElement otherWrapper)
                {
                    other = otherWrapper.WrappedElement;
                }

                return WrappedElement.Equals(other);
            }

            /// <summary>
            /// Return the hash code for this <see cref="EventFiringWebElement"/>.
            /// </summary>
            /// <returns>A 32-bit signed integer hash code.</returns>
            public override int GetHashCode()
            {
                return this.WrappedElement.GetHashCode();
            }
        }

        /// <summary>
        /// EventFiringShadowElement allows you to have access to specific shadow elements
        /// </summary>
        private class EventFiringShadowRoot : ISearchContext, IWrapsDriver
        {
            private readonly EventFiringWebDriver parentDriver;

            /// <summary>
            /// Initializes a new instance of the <see cref="EventFiringShadowRoot"/> class.
            /// </summary>
            /// <param name="driver">The <see cref="EventFiringWebDriver"/> instance hosting this element.</param>
            /// <param name="searchContext">The <see cref="ISearchContext"/> to wrap for event firing.</param>
            public EventFiringShadowRoot(EventFiringWebDriver driver, ISearchContext searchContext)
            {
                this.WrappedSearchContext = searchContext ?? throw new ArgumentNullException(nameof(searchContext));
                this.parentDriver = driver;
            }

            /// <summary>
            /// Gets the underlying wrapped <see cref="ISearchContext"/>.
            /// </summary>
            public ISearchContext WrappedSearchContext { get; }

            /// <summary>
            /// Gets the underlying parent wrapped <see cref="IWebDriver"/>
            /// </summary>
            public IWebDriver WrappedDriver => this.parentDriver;

            /// <summary>
            /// Finds the first element in the page that matches the <see cref="By"/> object
            /// </summary>
            /// <param name="by">By mechanism to find the element</param>
            /// <returns>IWebElement object so that you can interaction that object</returns>
            public IWebElement FindElement(By by)
            {
                IWebElement wrappedElement;
                try
                {
                    GetShadowRootEventArgs e = new GetShadowRootEventArgs(this.parentDriver.WrappedDriver, this.WrappedSearchContext);
                    this.parentDriver.OnGettingShadowRoot(e);
                    IWebElement element = this.WrappedSearchContext.FindElement(by);
                    this.parentDriver.OnGetShadowRootCompleted(e);
                    wrappedElement = new EventFiringWebElement(this.parentDriver, element);
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }

                return wrappedElement;
            }

            /// <summary>
            /// Finds the elements on the page by using the <see cref="By"/> object and returns a ReadOnlyCollection of the Elements on the page
            /// </summary>
            /// <param name="by">By mechanism to find the element</param>
            /// <returns>ReadOnlyCollection of IWebElement</returns>
            public ReadOnlyCollection<IWebElement> FindElements(By by)
            {
                try
                {
                    GetShadowRootEventArgs e = new GetShadowRootEventArgs(this.parentDriver.WrappedDriver, this.WrappedSearchContext);
                    this.parentDriver.OnGettingShadowRoot(e);
                    ReadOnlyCollection<IWebElement> elements = this.WrappedSearchContext.FindElements(by);
                    this.parentDriver.OnGetShadowRootCompleted(e);

                    List<IWebElement> wrappedElementList = new List<IWebElement>(elements.Count);
                    foreach (IWebElement element in elements)
                    {
                        IWebElement wrappedElement = this.parentDriver.WrapElement(element);
                        wrappedElementList.Add(wrappedElement);
                    }

                    return wrappedElementList.AsReadOnly();
                }
                catch (Exception ex)
                {
                    this.parentDriver.OnException(new WebDriverExceptionEventArgs(this.parentDriver, ex));
                    throw;
                }

            }

            /// <summary>
            /// Determines whether the specified <see cref="EventFiringShadowRoot"/> is equal to the current <see cref="EventFiringShadowRoot"/>.
            /// </summary>
            /// <param name="obj">The <see cref="EventFiringWebElement"/> to compare to the current <see cref="EventFiringShadowRoot"/>.</param>
            /// <returns><see langword="true"/> if the specified <see cref="EventFiringShadowRoot"/> is equal to the current <see cref="EventFiringShadowRoot"/>; otherwise, <see langword="false"/>.</returns>
            public override bool Equals(object obj)
            {
                if (obj is not ISearchContext other)
                {
                    return false;
                }

                return WrappedSearchContext.Equals(other);
            }

            /// <summary>
            /// Return the hash code for this <see cref="EventFiringWebElement"/>.
            /// </summary>
            /// <returns>A 32-bit signed integer hash code.</returns>
            public override int GetHashCode()
            {
                return this.WrappedSearchContext.GetHashCode();
            }
        }
    }
}
