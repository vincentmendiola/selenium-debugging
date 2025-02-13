// <copyright file="EdgeDriver.cs" company="Selenium Committers">
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

using OpenQA.Selenium.Chromium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OpenQA.Selenium.Edge
{
    /// <summary>
    /// Provides a mechanism to write tests against Edge
    /// </summary>
    public class EdgeDriver : ChromiumDriver
    {
        private static Dictionary<string, CommandInfo> edgeCustomCommands = new Dictionary<string, CommandInfo>()
        {
            { ExecuteCdp, new HttpCommandInfo(HttpCommandInfo.PostCommand, "/session/{sessionId}/ms/cdp/execute") },
            { GetCastSinksCommand, new HttpCommandInfo(HttpCommandInfo.GetCommand, "/session/{sessionId}/ms/cast/get_sinks") },
            { SelectCastSinkCommand, new HttpCommandInfo(HttpCommandInfo.PostCommand, "/session/{sessionId}/ms/cast/set_sink_to_use") },
            { StartCastTabMirroringCommand, new HttpCommandInfo(HttpCommandInfo.PostCommand, "/session/{sessionId}/ms/cast/start_tab_mirroring") },
            { StartCastDesktopMirroringCommand, new HttpCommandInfo(HttpCommandInfo.PostCommand, "/session/{sessionId}/ms/cast/start_desktop_mirroring") },
            { GetCastIssueMessageCommand, new HttpCommandInfo(HttpCommandInfo.GetCommand, "/session/{sessionId}/ms/cast/get_issue_message") },
            { StopCastingCommand, new HttpCommandInfo(HttpCommandInfo.PostCommand, "/session/{sessionId}/ms/cast/stop_casting") }
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeDriver"/> class.
        /// </summary>
        public EdgeDriver()
            : this(new EdgeOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeDriver"/> class using the specified options.
        /// </summary>
        /// <param name="options">The <see cref="EdgeOptions"/> to be used with the Edge driver.</param>
        public EdgeDriver(EdgeOptions options)
            : this(EdgeDriverService.CreateDefaultService(), options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeDriver"/> class using the specified driver service.
        /// </summary>
        /// <param name="service">The <see cref="EdgeDriverService"/> used to initialize the driver.</param>
        public EdgeDriver(EdgeDriverService service)
            : this(service, new EdgeOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeDriver"/> class using the specified path
        /// to the directory containing the WebDriver executable.
        /// </summary>
        /// <param name="edgeDriverDirectory">The full path to the directory containing the WebDriver executable.</param>
        public EdgeDriver(string edgeDriverDirectory)
            : this(edgeDriverDirectory, new EdgeOptions())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeDriver"/> class using the specified path
        /// to the directory containing the WebDriver executable and options.
        /// </summary>
        /// <param name="edgeDriverDirectory">The full path to the directory containing the WebDriver executable.</param>
        /// <param name="options">The <see cref="EdgeOptions"/> to be used with the Edge driver.</param>
        public EdgeDriver(string edgeDriverDirectory, EdgeOptions options)
            : this(edgeDriverDirectory, options, RemoteWebDriver.DefaultCommandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeDriver"/> class using the specified path
        /// to the directory containing the WebDriver executable, options, and command timeout.
        /// </summary>
        /// <param name="edgeDriverDirectory">The full path to the directory containing the WebDriver executable.</param>
        /// <param name="options">The <see cref="EdgeOptions"/> to be used with the Edge driver.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        public EdgeDriver(string edgeDriverDirectory, EdgeOptions options, TimeSpan commandTimeout)
            : this(EdgeDriverService.CreateDefaultService(edgeDriverDirectory), options, commandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeDriver"/> class using the specified
        /// <see cref="EdgeDriverService"/> and options.
        /// </summary>
        /// <param name="service">The <see cref="EdgeDriverService"/> to use.</param>
        /// <param name="options">The <see cref="EdgeOptions"/> used to initialize the driver.</param>
        public EdgeDriver(EdgeDriverService service, EdgeOptions options)
            : this(service, options, RemoteWebDriver.DefaultCommandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EdgeDriver"/> class using the specified <see cref="EdgeDriverService"/>.
        /// </summary>
        /// <param name="service">The <see cref="EdgeDriverService"/> to use.</param>
        /// <param name="options">The <see cref="EdgeOptions"/> to be used with the Edge driver.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        public EdgeDriver(EdgeDriverService service, EdgeOptions options, TimeSpan commandTimeout)
            : base(service, options, commandTimeout)
        {
            this.AddCustomEdgeCommands();
        }

        /// <summary>
        /// Gets a read-only dictionary of the custom WebDriver commands defined for ChromeDriver.
        /// The keys of the dictionary are the names assigned to the command; the values are the
        /// <see cref="CommandInfo"/> objects describing the command behavior.
        /// </summary>
        public static IReadOnlyDictionary<string, CommandInfo> CustomCommandDefinitions
        {
            get
            {
                Dictionary<string, CommandInfo> customCommands = new Dictionary<string, CommandInfo>();
                foreach (KeyValuePair<string, CommandInfo> entry in ChromiumCustomCommands)
                {
                    customCommands[entry.Key] = entry.Value;
                }

                foreach (KeyValuePair<string, CommandInfo> entry in edgeCustomCommands)
                {
                    customCommands[entry.Key] = entry.Value;
                }

                return new ReadOnlyDictionary<string, CommandInfo>(customCommands);
            }
        }

        private void AddCustomEdgeCommands()
        {
            foreach (KeyValuePair<string, CommandInfo> entry in CustomCommandDefinitions)
            {
                this.RegisterInternalDriverCommand(entry.Key, entry.Value);
            }
        }
    }
}
