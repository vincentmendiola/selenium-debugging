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

package org.openqa.selenium.firefox;

import java.util.Collections;
import java.util.Locale;
import java.util.Map;
import java.util.logging.Level;

/**
 * <a href="https://github.com/mozilla/geckodriver#log-object">Log levels</a> defined by GeckoDriver
 */
public enum FirefoxDriverLogLevel {
  TRACE,
  DEBUG,
  CONFIG,
  INFO,
  WARN,
  ERROR,
  FATAL;

  private static final Map<Level, FirefoxDriverLogLevel> logLevelToGeckoLevelMap =
      Map.of(
          Level.ALL, TRACE,
          Level.FINEST, TRACE,
          Level.FINER, TRACE,
          Level.FINE, DEBUG,
          Level.CONFIG, CONFIG,
          Level.INFO, INFO,
          Level.WARNING, WARN,
          Level.SEVERE, ERROR,
          Level.OFF, FATAL);

  @Override
  public String toString() {
    return super.toString().toLowerCase(Locale.ENGLISH);
  }

  public static FirefoxDriverLogLevel fromString(String text) {
    if (text != null) {
      for (FirefoxDriverLogLevel b : FirefoxDriverLogLevel.values()) {
        if (text.equalsIgnoreCase(b.toString())) {
          return b;
        }
      }
    }
    return null;
  }

  public static FirefoxDriverLogLevel fromLevel(Level level) {
    return logLevelToGeckoLevelMap.getOrDefault(level, DEBUG);
  }

  Map<String, String> toJson() {
    return Collections.singletonMap("level", toString());
  }

  static FirefoxDriverLogLevel fromJson(Map<String, String> json) {
    return fromString(json.get("level"));
  }
}
