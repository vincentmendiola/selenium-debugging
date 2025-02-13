// <copyright file="Screenshot.cs" company="Selenium Committers">
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
using System.IO;

#nullable enable

namespace OpenQA.Selenium
{
    /// <summary>
    /// Represents an image of the page currently loaded in the browser.
    /// </summary>
    [Serializable]
    public class Screenshot : EncodedFile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Screenshot"/> class.
        /// </summary>
        /// <param name="base64EncodedScreenshot">The image of the page as a Base64-encoded string.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="base64EncodedScreenshot"/> is <see langword="null"/>.</exception>
        /// <exception cref="FormatException">
        /// <para>The length of <paramref name="base64EncodedScreenshot"/>, ignoring white-space characters, is not zero or a multiple of 4.</para>
        /// <para>-or-</para>
        /// <para>The format of <paramref name="base64EncodedScreenshot"/> is invalid. <paramref name="base64EncodedScreenshot"/> contains a non-base-64 character,
        /// more than two padding characters, or a non-white space-character among the padding characters.</para>
        /// </exception>
        public Screenshot(string base64EncodedScreenshot) : base(base64EncodedScreenshot)
        {
        }

        /// <summary>
        /// Saves the screenshot to a Portable Network Graphics (PNG) file, overwriting the
        /// file if it already exists.
        /// </summary>
        /// <param name="fileName">The full path and file name to save the screenshot to.</param>
        public override void SaveAsFile(string fileName)
        {
            File.WriteAllBytes(fileName, this.AsByteArray);
        }
    }
}
