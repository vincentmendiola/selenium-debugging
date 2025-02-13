// <copyright file="Alert.cs" company="Selenium Committers">
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

#nullable enable

namespace OpenQA.Selenium
{
    /// <summary>
    /// Defines the interface through which the user can manipulate JavaScript alerts.
    /// </summary>
    internal class Alert : IAlert
    {
        private readonly WebDriver driver;

        /// <summary>
        /// Initializes a new instance of the <see cref="Alert"/> class.
        /// </summary>
        /// <param name="driver">The <see cref="WebDriver"/> for which the alerts will be managed.</param>
        public Alert(WebDriver driver)
        {
            this.driver = driver;
        }

        /// <summary>
        /// Gets the text of the alert.
        /// </summary>
        public string? Text
        {
            get
            {
                Response commandResponse = this.driver.InternalExecute(DriverCommand.GetAlertText, null);
                return (string?)commandResponse.Value;
            }
        }

        /// <summary>
        /// Dismisses the alert.
        /// </summary>
        public void Dismiss()
        {
            this.driver.InternalExecute(DriverCommand.DismissAlert, null);
        }

        /// <summary>
        /// Accepts the alert.
        /// </summary>
        public void Accept()
        {
            this.driver.InternalExecute(DriverCommand.AcceptAlert, null);
        }

        /// <summary>
        /// Sends keys to the alert.
        /// </summary>
        /// <param name="keysToSend">The keystrokes to send.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="keysToSend" /> is <see langword="null"/>.</exception>
        public void SendKeys(string keysToSend)
        {
            if (keysToSend is null)
            {
                throw new ArgumentNullException(nameof(keysToSend), "Keys to send must not be null.");
            }

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("text", keysToSend);

            this.driver.InternalExecute(DriverCommand.SetAlertValue, parameters);
        }
    }
}
