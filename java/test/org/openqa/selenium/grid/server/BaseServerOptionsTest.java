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

package org.openqa.selenium.grid.server;

import static org.assertj.core.api.Assertions.assertThat;
import static org.junit.jupiter.api.Assertions.assertThrows;

import java.net.URI;
import java.net.URISyntaxException;
import java.util.Map;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.grid.config.MapConfig;

class BaseServerOptionsTest {

  @Test
  void readingThePortTwiceShouldGiveTheSameResult() {
    BaseServerOptions options =
        new BaseServerOptions(new MapConfig(Map.of("server", Map.of("port", -1))));

    int first = options.getPort();
    int second = options.getPort();

    assertThat(first).isEqualTo(second);
  }

  @Test
  void serverConfigBindsToHostByDefault() {
    BaseServerOptions options =
        new BaseServerOptions(new MapConfig(Map.of("server", Map.of("port", 4444))));

    assertThat(options.getBindHost()).isEqualTo(true);
  }

  @Test
  void externalUriFailsForNonUriStrings() {
    BaseServerOptions options =
        new BaseServerOptions(new MapConfig(Map.of("server", Map.of("external-url", "not a URL"))));

    Exception exception = assertThrows(RuntimeException.class, options::getExternalUri);

    assertThat(exception.getMessage())
        .as("External URI must be parseable as URI.")
        .isEqualTo(
            "Supplied external URI is invalid: Illegal character in path at index 3: not a URL");
  }

  @Test
  void externalUriTakesPriorityOverDefaults() throws URISyntaxException {
    URI expected = new URI("http://10.0.1.1:33333");

    BaseServerOptions options =
        new BaseServerOptions(
            new MapConfig(
                Map.of(
                    "server",
                    Map.of(
                        "external-url", expected.toString(), "host", "localhost", "port", 5555))));

    assertThat(options.getExternalUri()).isEqualTo(expected);
  }

  @Test
  void externalUriDefaultsToValueDerivedFromHostnameAndPortWhenNotDefined()
      throws URISyntaxException {
    URI expected = new URI("http://localhost:5555");

    BaseServerOptions options =
        new BaseServerOptions(
            new MapConfig(
                Map.of(
                    "server",
                    Map.of(
                        "host", expected.getHost(),
                        "port", expected.getPort()))));

    assertThat(options.getExternalUri()).isEqualTo(expected);
  }
}
