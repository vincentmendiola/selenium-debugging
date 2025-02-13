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

package org.openqa.selenium.chrome;

import static org.openqa.selenium.remote.Browser.CHROME;

import com.google.auto.service.AutoService;
import java.util.Map;
import java.util.function.Predicate;
import org.openqa.selenium.Capabilities;
import org.openqa.selenium.remote.AdditionalHttpCommands;
import org.openqa.selenium.remote.AugmenterProvider;
import org.openqa.selenium.remote.CommandInfo;
import org.openqa.selenium.remote.http.HttpMethod;

@SuppressWarnings({"rawtypes", "RedundantSuppression"})
@AutoService({AdditionalHttpCommands.class, AugmenterProvider.class})
public class AddHasCasting extends org.openqa.selenium.chromium.AddHasCasting {

  @Override
  public Map<String, CommandInfo> getAdditionalCommands() {
    return Map.of(
        GET_CAST_SINKS, new CommandInfo("session/:sessionId/goog/cast/get_sinks", HttpMethod.GET),
        SET_CAST_SINK_TO_USE,
            new CommandInfo("session/:sessionId/goog/cast/set_sink_to_use", HttpMethod.POST),
        START_CAST_DESKTOP_MIRRORING,
            new CommandInfo(
                "session/:sessionId/goog/cast/start_desktop_mirroring", HttpMethod.POST),
        START_CAST_TAB_MIRRORING,
            new CommandInfo("session/:sessionId/goog/cast/start_tab_mirroring", HttpMethod.POST),
        GET_CAST_ISSUE_MESSAGE,
            new CommandInfo("session/:sessionId/goog/cast/get_issue_message", HttpMethod.GET),
        STOP_CASTING,
            new CommandInfo("session/:sessionId/goog/cast/stop_casting", HttpMethod.POST));
  }

  @Override
  public Predicate<Capabilities> isApplicable() {
    return CHROME::is;
  }
}
