// <copyright file="UrlPattern.cs" company="Selenium Committers">
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

using System.Text.Json.Serialization;

#nullable enable

namespace OpenQA.Selenium.BiDi.Modules.Network;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(Pattern), "pattern")]
[JsonDerivedType(typeof(String), "string")]
public abstract record UrlPattern
{
    public static implicit operator UrlPattern(string value) => new String(value);

    public record Pattern : UrlPattern
    {
        public string? Protocol { get; set; }

        public string? Hostname { get; set; }

        public string? Port { get; set; }

        public string? Pathname { get; set; }

        public string? Search { get; set; }
    }

    public record String(string Pattern) : UrlPattern
    {
        public new string Pattern { get; } = Pattern;
    }
}
