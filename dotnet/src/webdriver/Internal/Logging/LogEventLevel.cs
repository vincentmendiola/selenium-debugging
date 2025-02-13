// <copyright file="LogEventLevel.cs" company="Selenium Committers">
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

#nullable enable

namespace OpenQA.Selenium.Internal.Logging
{
    /// <summary>
    /// Defines the levels of logging events.
    /// </summary>
    public enum LogEventLevel
    {
        /// <summary>
        /// The most detailed log events.
        /// </summary>
        Trace = 0,

        /// <summary>
        /// Log events that are useful for debugging purposes.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Log events that provide general information about the application's operation.
        /// </summary>
        Info = 2,

        /// <summary>
        /// Log events that indicate a potential problem or a non-critical error.
        /// </summary>
        Warn = 3,

        /// <summary>
        /// Log events that indicate a serious error or a failure that requires immediate attention.
        /// </summary>
        Error = 4,

        /// <summary>
        /// No log events.
        /// </summary>
        None = 5
    }
}
