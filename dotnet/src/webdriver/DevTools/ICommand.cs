// <copyright file="ICommand.cs" company="Selenium Committers">
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

namespace OpenQA.Selenium.DevTools
{
    /// <summary>
    /// Represents a command used by the DevTools Remote Interface
    ///</summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        string CommandName
        {
            get;
        }
    }

    /// <summary>
    /// Represents a response to a command submitted by the DevTools Remote Interface
    ///</summary>
    public interface ICommandResponse
    {
    }

    /// <summary>
    /// Represents a response to a command submitted by the DevTools Remote Interface
    ///</summary>
    public interface ICommandResponse<T> : ICommandResponse
        where T : ICommand
    {
    }
}
