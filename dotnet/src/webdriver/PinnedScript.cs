// <copyright file="PinnedScript.cs" company="Selenium Committers">
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

using System.Globalization;

#nullable enable

namespace OpenQA.Selenium
{
    /// <summary>
    /// A class representing a pinned JavaScript function that can be repeatedly called
    /// without sending the entire script across the wire for every execution.
    /// </summary>
    public sealed class PinnedScript
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PinnedScript"/> class.
        /// </summary>
        /// <param name="script">The body of the JavaScript function to pin.</param>
        /// <param name="stringHandle">The unique handle for this pinned script.</param>
        /// <param name="scriptId">The internal ID of this script.</param>
        /// <remarks>
        /// This constructor is explicitly internal. Creation of pinned script objects
        /// is strictly the perview of Selenium, and should not be required by external
        /// libraries.
        /// </remarks>
        internal PinnedScript(string script, string stringHandle, string scriptId)
        {
            this.Source = script;
            this.Handle = stringHandle;
            this.ScriptId = scriptId;
        }

        /// <summary>
        /// Gets the unique handle for this pinned script.
        /// </summary>
        public string Handle { get; }

        /// <summary>
        /// Gets the source representing the body of the function in the pinned script.
        /// </summary>
        public string Source { get; }

        internal static string MakeCreationScript(string scriptHandle, string scriptSource)
        {
            return string.Format(CultureInfo.InvariantCulture, "function __webdriver_{0}(arguments) {{ {1} }}", scriptHandle, scriptSource);
        }

        /// <summary>
        /// Gets the script used to execute the pinned script in the browser.
        /// </summary>
        internal string MakeExecutionScript()
        {
            return string.Format(CultureInfo.InvariantCulture, "return __webdriver_{0}(arguments)", this.Handle);
        }

        /// <summary>
        /// Gets the script used to remove the pinned script from the browser.
        /// </summary>
        internal string MakeRemovalScript()
        {
            return string.Format(CultureInfo.InvariantCulture, "__webdriver_{0} = undefined", this.Handle);
        }

        /// <summary>
        /// Gets or sets the ID of this script.
        /// </summary>
        internal string ScriptId { get; }
    }
}
