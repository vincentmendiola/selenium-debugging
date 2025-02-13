// <copyright file="AddInterceptCommand.cs" company="Selenium Committers">
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
using OpenQA.Selenium.BiDi.Communication;

#nullable enable

namespace OpenQA.Selenium.BiDi.Modules.Network;

internal class AddInterceptCommand(AddInterceptCommandParameters @params) : Command<AddInterceptCommandParameters>(@params);

internal record AddInterceptCommandParameters(IEnumerable<InterceptPhase> Phases) : CommandParameters
{
    public IEnumerable<BrowsingContext.BrowsingContext>? Contexts { get; set; }

    public IEnumerable<UrlPattern>? UrlPatterns { get; set; }
}

public record AddInterceptOptions : CommandOptions
{
    public AddInterceptOptions() { }

    internal AddInterceptOptions(BrowsingContextAddInterceptOptions? options)
    {
        UrlPatterns = options?.UrlPatterns;
    }

    public IEnumerable<BrowsingContext.BrowsingContext>? Contexts { get; set; }

    public IEnumerable<UrlPattern>? UrlPatterns { get; set; }
}

public record BrowsingContextAddInterceptOptions
{
    public IEnumerable<UrlPattern>? UrlPatterns { get; set; }
}

public record AddInterceptResult(Intercept Intercept);

public enum InterceptPhase
{
    BeforeRequestSent,
    ResponseStarted,
    AuthRequired
}
