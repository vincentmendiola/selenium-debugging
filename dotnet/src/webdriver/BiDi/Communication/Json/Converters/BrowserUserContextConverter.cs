// <copyright file="BrowserUserContextConverter.cs" company="Selenium Committers">
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

using OpenQA.Selenium.BiDi.Modules.Browser;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable enable

namespace OpenQA.Selenium.BiDi.Communication.Json.Converters;

internal class BrowserUserContextConverter : JsonConverter<UserContext>
{
    private readonly BiDi _bidi;

    public BrowserUserContextConverter(BiDi bidi)
    {
        _bidi = bidi;
    }

    public override UserContext? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var id = reader.GetString();

        return new UserContext(_bidi, id!);
    }

    public override void Write(Utf8JsonWriter writer, UserContext value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Id);
    }
}
