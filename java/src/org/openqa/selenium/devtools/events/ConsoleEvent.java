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

package org.openqa.selenium.devtools.events;

import java.time.Instant;
import java.util.List;
import java.util.Objects;
import java.util.stream.Collectors;
import java.util.stream.Stream;
import org.openqa.selenium.devtools.idealized.runtime.model.RemoteObject;

public class ConsoleEvent {

  private final String type;
  private final Instant timestamp;
  private final List<Object> modifiedArgs;
  private final List<Object> args;

  public ConsoleEvent(String type, Instant timestamp, List<Object> modifiedArgs, Object... args) {
    this.type = type;
    this.timestamp = timestamp;
    this.modifiedArgs = modifiedArgs;
    this.args = List.of(args);
  }

  public String getType() {
    return type;
  }

  public Instant getTimestamp() {
    return timestamp;
  }

  public List<Object> getArgs() {
    return args;
  }

  public List<String> getMessages() {
    return modifiedArgs.stream()
        .map(RemoteObject.class::cast)
        .map(RemoteObject::getValue)
        .filter(Objects::nonNull)
        .map(Object::toString)
        .collect(Collectors.toList());
  }

  @Override
  public String toString() {
    return String.format(
        "%s [%s] %s",
        timestamp, type, Stream.of(args).map(String::valueOf).collect(Collectors.joining(", ")));
  }
}
