// <copyright file="GetUserContextsCommand.cs" company="Selenium Committers">
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

using OpenQA.Selenium.BiDi.Communication;
using System.Collections;
using System.Collections.Generic;

#nullable enable

namespace OpenQA.Selenium.BiDi.Modules.Browser;

internal class GetUserContextsCommand() : Command<CommandParameters>(CommandParameters.Empty);

public record GetUserContextsOptions : CommandOptions;

public record GetUserContextsResult : IReadOnlyList<UserContextInfo>
{
    private readonly IReadOnlyList<UserContextInfo> _userContexts;

    internal GetUserContextsResult(IReadOnlyList<UserContextInfo> userContexts)
    {
        _userContexts = userContexts;
    }

    public UserContextInfo this[int index] => _userContexts[index];

    public int Count => _userContexts.Count;

    public IEnumerator<UserContextInfo> GetEnumerator() => _userContexts.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (_userContexts as IEnumerable).GetEnumerator();
}
