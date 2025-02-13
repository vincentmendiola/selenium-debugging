// <copyright file="DriverCommand.cs" company="Selenium Committers">
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

using System.Collections.Generic;

namespace OpenQA.Selenium
{
    /// <summary>
    /// Values describing the list of commands understood by a remote server using the JSON wire protocol.
    /// </summary>
    public static class DriverCommand
    {
        /// <summary>
        /// Represents the Status command.
        /// </summary>
        public static readonly string Status = "status";

        /// <summary>
        /// Represents a New Session command
        /// </summary>
        public static readonly string NewSession = "newSession";

        /// <summary>
        /// Represents a Browser close command
        /// </summary>
        public static readonly string Close = "close";

        /// <summary>
        /// Represents a browser quit command
        /// </summary>
        public static readonly string Quit = "quit";

        /// <summary>
        /// Represents a GET command
        /// </summary>
        public static readonly string Get = "get";

        /// <summary>
        /// Represents a Browser going back command
        /// </summary>
        public static readonly string GoBack = "goBack";

        /// <summary>
        /// Represents a Browser going forward command
        /// </summary>
        public static readonly string GoForward = "goForward";

        /// <summary>
        /// Represents a Browser refreshing command
        /// </summary>
        public static readonly string Refresh = "refresh";

        /// <summary>
        /// Represents adding a cookie command
        /// </summary>
        public static readonly string AddCookie = "addCookie";

        /// <summary>
        /// Represents getting all cookies command
        /// </summary>
        public static readonly string GetAllCookies = "getCookies";

        /// <summary>
        /// Represents getting cookie command
        /// </summary>
        public static readonly string GetCookie = "getCookie";

        /// <summary>
        /// Represents deleting a cookie command
        /// </summary>
        public static readonly string DeleteCookie = "deleteCookie";

        /// <summary>
        /// Represents Deleting all cookies command
        /// </summary>
        public static readonly string DeleteAllCookies = "deleteAllCookies";

        /// <summary>
        /// Represents FindElement command
        /// </summary>
        public static readonly string FindElement = "findElement";

        /// <summary>
        /// Represents FindElements command
        /// </summary>
        public static readonly string FindElements = "findElements";

        /// <summary>
        /// Represents FindChildElement command
        /// </summary>
        public static readonly string FindChildElement = "findChildElement";

        /// <summary>
        /// Represents FindChildElements command
        /// </summary>
        public static readonly string FindChildElements = "findChildElements";

        /// <summary>
        /// Represents FindShadowChildElement command
        /// </summary>
        public static readonly string FindShadowChildElement = "findShadowChildElement";

        /// <summary>
        /// Represents FindShadosChildElements command
        /// </summary>
        public static readonly string FindShadowChildElements = "findShadowChildElements";

        /// <summary>
        /// Represents ClearElement command
        /// </summary>
        public static readonly string ClearElement = "clearElement";

        /// <summary>
        /// Represents ClickElement command
        /// </summary>
        public static readonly string ClickElement = "clickElement";

        /// <summary>
        /// Represents SendKeysToElements command
        /// </summary>
        public static readonly string SendKeysToElement = "sendKeysToElement";

        /// <summary>
        /// Represents GetCurrentWindowHandle command
        /// </summary>
        public static readonly string GetCurrentWindowHandle = "getCurrentWindowHandle";

        /// <summary>
        /// Represents GetWindowHandles command
        /// </summary>
        public static readonly string GetWindowHandles = "getWindowHandles";

        /// <summary>
        /// Represents SwitchToWindow command
        /// </summary>
        public static readonly string SwitchToWindow = "switchToWindow";

        /// <summary>
        /// Represents NewWindow command
        /// </summary>
        public static readonly string NewWindow = "newWindow";

        /// <summary>
        /// Represents SwitchToFrame command
        /// </summary>
        public static readonly string SwitchToFrame = "switchToFrame";

        /// <summary>
        /// Represents SwitchToParentFrame command
        /// </summary>
        public static readonly string SwitchToParentFrame = "switchToParentFrame";

        /// <summary>
        /// Represents GetActiveElement command
        /// </summary>
        public static readonly string GetActiveElement = "getActiveElement";

        /// <summary>
        /// Represents GetCurrentUrl command
        /// </summary>
        public static readonly string GetCurrentUrl = "getCurrentUrl";

        /// <summary>
        /// Represents GetPageSource command
        /// </summary>
        public static readonly string GetPageSource = "getPageSource";

        /// <summary>
        /// Represents GetTitle command
        /// </summary>
        public static readonly string GetTitle = "getTitle";

        /// <summary>
        /// Represents ExecuteScript command
        /// </summary>
        public static readonly string ExecuteScript = "executeScript";

        /// <summary>
        /// Represents ExecuteAsyncScript command
        /// </summary>
        public static readonly string ExecuteAsyncScript = "executeAsyncScript";

        /// <summary>
        /// Represents GetElementText command
        /// </summary>
        public static readonly string GetElementText = "getElementText";

        /// <summary>
        /// Represents GetElementTagName command
        /// </summary>
        public static readonly string GetElementTagName = "getElementTagName";

        /// <summary>
        /// Represents IsElementSelected command
        /// </summary>
        public static readonly string IsElementSelected = "isElementSelected";

        /// <summary>
        /// Represents IsElementEnabled command
        /// </summary>
        public static readonly string IsElementEnabled = "isElementEnabled";

        /// <summary>
        /// Represents IsElementDisplayed command
        /// </summary>
        public static readonly string IsElementDisplayed = "isElementDisplayed";

        /// <summary>
        /// Represents GetElementRect command
        /// </summary>
        public static readonly string GetElementRect = "getElementRect";

        /// <summary>
        /// Represents GetElementAttribute command
        /// </summary>
        public static readonly string GetElementAttribute = "getElementAttribute";

        /// <summary>
        /// Represents GetElementProperty command
        /// </summary>
        public static readonly string GetElementProperty = "getElementProperty";

        /// <summary>
        /// Represents GetElementValueOfCSSProperty command
        /// </summary>
        public static readonly string GetElementValueOfCssProperty = "getElementValueOfCssProperty";

        /// <summary>
        /// Represents GetComputedAccessibleLabel command
        /// </summary>
        public static readonly string GetComputedAccessibleLabel = "getComputedAccessibleLabel";

        /// <summary>
        /// Represents GetComputedAccessibleRole command
        /// </summary>
        public static readonly string GetComputedAccessibleRole = "getComputedAccessibleRole";

        /// <summary>
        /// Represents the GetElementShadowRoot command.
        /// </summary>
        public static readonly string GetElementShadowRoot = "getElementShadowRoot";

        /// <summary>
        /// Represents ElementEquals command
        /// </summary>
        public static readonly string ElementEquals = "elementEquals";

        /// <summary>
        /// Represents Screenshot command
        /// </summary>
        public static readonly string Screenshot = "screenshot";

        /// <summary>
        /// Represents the ElementScreenshot command
        /// </summary>
        public static readonly string ElementScreenshot = "elementScreenshot";

        /// <summary>
        /// Represents the Print command
        /// </summary>
        public static readonly string Print = "print";

        /// <summary>
        /// Represents GetWindowRect command
        /// </summary>
        public static readonly string GetWindowRect = "getWindowRect";

        /// <summary>
        /// Represents SetWindowRect command
        /// </summary>
        public static readonly string SetWindowRect = "setWindowRect";

        /// <summary>
        /// Represents MaximizeWindow command
        /// </summary>
        public static readonly string MaximizeWindow = "maximizeWindow";

        /// <summary>
        /// Represents MinimizeWindow command
        /// </summary>
        public static readonly string MinimizeWindow = "minimizeWindow";

        /// <summary>
        /// Represents FullScreenWindow command
        /// </summary>
        public static readonly string FullScreenWindow = "fullScreenWindow";

        /// <summary>
        /// Represents the DismissAlert command
        /// </summary>
        public static readonly string DismissAlert = "dismissAlert";

        /// <summary>
        /// Represents the AcceptAlert command
        /// </summary>
        public static readonly string AcceptAlert = "acceptAlert";

        /// <summary>
        /// Represents the GetAlertText command
        /// </summary>
        public static readonly string GetAlertText = "getAlertText";

        /// <summary>
        /// Represents the SetAlertValue command
        /// </summary>
        public static readonly string SetAlertValue = "setAlertValue";

        /// <summary>
        /// Represents the SetTimeout command
        /// </summary>
        public static readonly string SetTimeouts = "setTimeouts";

        /// <summary>
        /// Represents the SetTimeout command
        /// </summary>
        public static readonly string GetTimeouts = "getTimeouts";

        /// <summary>
        /// Represents the Actions command.
        /// </summary>
        public static readonly string Actions = "actions";

        /// <summary>
        /// Represents the CancelActions command.
        /// </summary>
        public static readonly string CancelActions = "cancelActions";

        /// <summary>
        /// Represents the UploadFile command.
        /// </summary>
        public static readonly string UploadFile = "uploadFile";

        /// <summary>
        /// Represents the GetAvailableLogTypes command.
        /// </summary>
        public static readonly string GetAvailableLogTypes = "getAvailableLogTypes";

        /// <summary>
        /// Represents the GetLog command.
        /// </summary>
        public static readonly string GetLog = "getLog";

        // Virtual Authenticator API
        // http://w3c.github.io/webauthn#sctn-automation
        /// <summary>
        /// Represents the AddVirtualAuthenticator command.
        /// </summary>
        public static readonly string AddVirtualAuthenticator = "addVirtualAuthenticator";

        /// <summary>
        /// Represents the RemoveVirtualAuthenticator command.
        /// </summary>
        public static readonly string RemoveVirtualAuthenticator = "removeVirtualAuthenticator";

        /// <summary>
        /// Represents the AddCredential command
        /// </summary>
        public static readonly string AddCredential = "addCredential";

        /// <summary>
        /// Represents the GetCredentials command.
        /// </summary>
        public static readonly string GetCredentials = "getCredentials";

        /// <summary>
        /// Represents the RemoveCredential command.
        /// </summary>
        public static readonly string RemoveCredential = "removeCredential";

        /// <summary>
        /// Represents the RemoveAllCredentials command.
        /// </summary>
        public static readonly string RemoveAllCredentials = "removeAllCredentials";

        /// <summary>
        /// Represents the SetUserVerified command.
        /// </summary>
        public static readonly string SetUserVerified = "setUserVerified";

        /// <summary>
        /// Represents the GetDownloadableFiles command.
        /// </summary>
        public static readonly string GetDownloadableFiles = "getDownloadableFiles";

        /// <summary>
        /// Represents the DownloadFile command.
        /// </summary>
        public static readonly string DownloadFile = "downloadFile";

        /// <summary>
        /// Represents the DeleteDownloadableFiles command.
        /// </summary>
        public static readonly string DeleteDownloadableFiles = "deleteDownloadableFiles";

        /// <summary>
        /// Lists the set of known commands valid for the Selenium library.
        /// </summary>
        public static readonly IList<string> KnownCommands = new List<string>()
        {
            Status,
            NewSession,
            Quit,
            GetTimeouts,
            SetTimeouts,
            Get,
            GetCurrentUrl,
            GoBack,
            GoForward,
            Refresh,
            GetTitle,
            GetCurrentWindowHandle,
            Close,
            SwitchToWindow,
            GetWindowHandles,
            SwitchToFrame,
            SwitchToParentFrame,
            GetWindowRect,
            SetWindowRect,
            MaximizeWindow,
            MinimizeWindow,
            FullScreenWindow,
            FindElement,
            FindElements,
            FindChildElement,
            FindChildElements,
            FindShadowChildElement,
            FindShadowChildElements,
            GetActiveElement,
            GetElementShadowRoot,
            IsElementSelected,
            GetElementAttribute,
            GetElementProperty,
            GetElementValueOfCssProperty,
            GetElementText,
            GetElementTagName,
            GetElementRect,
            IsElementEnabled,
            GetComputedAccessibleRole,
            GetComputedAccessibleLabel,
            ClickElement,
            ClearElement,
            SendKeysToElement,
            GetPageSource,
            ExecuteScript,
            ExecuteAsyncScript,
            GetAllCookies,
            GetCookie,
            AddCookie,
            DeleteCookie,
            DeleteAllCookies,
            Actions,
            CancelActions,
            AcceptAlert,
            DismissAlert,
            GetAlertText,
            SetAlertValue,
            Screenshot,
            ElementScreenshot,
            Print,
            IsElementDisplayed,
            UploadFile,
            GetLog,
            GetAvailableLogTypes,
            AddVirtualAuthenticator,
            RemoveVirtualAuthenticator,
            AddCredential,
            GetCredentials,
            RemoveCredential,
            RemoveAllCredentials,
            SetUserVerified,
            GetDownloadableFiles,
            DownloadFile,
            DeleteDownloadableFiles
        }.AsReadOnly();
    }
}
