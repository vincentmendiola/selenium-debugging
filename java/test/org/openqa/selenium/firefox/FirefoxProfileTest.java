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

import static org.assertj.core.api.Assertions.assertThat;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileReader;
import java.io.IOException;
import java.io.StringReader;
import java.nio.file.Files;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Set;
import java.util.stream.Collectors;
import java.util.stream.Stream;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Tag;
import org.junit.jupiter.api.Test;
import org.openqa.selenium.build.InProject;
import org.openqa.selenium.io.FileHandler;
import org.openqa.selenium.io.Zip;

@Tag("UnitTests")
class FirefoxProfileTest {
  private static final String EXT_PATH = "common/extensions/webextensions-selenium-example.xpi";
  private static final String EXT_RESOURCE_PATH =
      "java/test/org/openqa/selenium/firefox/webextensions-selenium-example.xpi";

  private FirefoxProfile profile;

  @BeforeEach
  public void setUp() {
    profile = new FirefoxProfile();
  }

  @Test
  void shouldQuoteStringsWhenSettingStringProperties() throws Exception {
    profile.setPreference("cheese", "brie");

    assertPreferenceValueEquals("cheese", "\"brie\"");
  }

  @Test
  void getStringPreferenceShouldReturnUserSuppliedValueWhenSet() {
    String key = "cheese";
    String value = "brie";
    profile.setPreference(key, value);

    String defaultValue = "edam";
    assertThat(profile.getStringPreference(key, defaultValue)).isEqualTo(value);
  }

  @Test
  void getStringPreferenceShouldReturnDefaultValueWhenSet() {
    String key = "cheese";

    String defaultValue = "brie";
    assertThat(profile.getStringPreference(key, defaultValue)).isEqualTo(defaultValue);
  }

  @Test
  void shouldSetIntegerPreferences() throws Exception {
    profile.setPreference("cheese", 1234);

    assertPreferenceValueEquals("cheese", 1234);
  }

  @Test
  void getIntegerPreferenceShouldReturnUserSuppliedValueWhenSet() {
    String key = "cheese";
    int value = 1234;
    profile.setPreference(key, value);

    int defaultValue = -42;
    assertThat(profile.getIntegerPreference(key, defaultValue)).isEqualTo(1234);
  }

  @Test
  void getIntegerPreferenceShouldReturnDefaultValueWhenSet() {
    String key = "cheese";

    int defaultValue = 42;
    assertThat(profile.getIntegerPreference(key, defaultValue)).isEqualTo(defaultValue);
  }

  @Test
  void shouldSetBooleanPreferences() throws Exception {
    profile.setPreference("cheese", false);

    assertPreferenceValueEquals("cheese", false);
  }

  @Test
  void getBooleanPreferenceShouldReturnUserSuppliedValueWhenSet() {
    String key = "cheese";
    boolean value = true;
    profile.setPreference(key, value);

    boolean defaultValue = false;
    assertThat(profile.getBooleanPreference(key, defaultValue)).isEqualTo(value);
  }

  @Test
  void getBooleanPreferenceShouldReturnDefaultValueWhenSet() {
    String key = "cheese";

    boolean defaultValue = true;
    assertThat(profile.getBooleanPreference(key, defaultValue)).isEqualTo(defaultValue);
  }

  @Test
  void shouldAllowSettingFrozenPreferences() throws Exception {
    profile.setPreference("network.http.phishy-userpass-length", 1024);
    assertPreferenceValueEquals("network.http.phishy-userpass-length", 1024);
  }

  @Test
  void shouldInstallWebExtensionFromZip() {
    profile.addExtension(InProject.locate(EXT_PATH).toFile());
    File profileDir = profile.layoutOnDisk();
    File extensionFile = new File(profileDir, "extensions/webextensions-selenium-example@0.1.xpi");
    assertThat(extensionFile).exists().isFile();
  }

  @Test
  void shouldInstallWebExtensionFromDirectory() throws IOException {
    File extension = InProject.locate(EXT_PATH).toFile();
    File unzippedExtension = Zip.unzipToTempDir(new FileInputStream(extension), "unzip", "stream");
    profile.addExtension(unzippedExtension);
    File profileDir = profile.layoutOnDisk();
    File extensionDir = new File(profileDir, "extensions/webextensions-selenium-example@0.1");
    assertThat(extensionDir).exists();
  }

  @Test
  void shouldInstallExtensionUsingClasspath() {
    profile.addExtension(FirefoxProfileTest.class, EXT_RESOURCE_PATH);
    File profileDir = profile.layoutOnDisk();
    File extensionDir = new File(profileDir, "extensions/webextensions-selenium-example@0.1.xpi");
    assertThat(extensionDir).exists();
  }

  @Test
  void convertingToJsonShouldNotPolluteTempDir() throws IOException {
    File sysTemp = new File(System.getProperty("java.io.tmpdir"));
    Set<String> before =
        Arrays.stream(sysTemp.list())
            .filter(f -> f.endsWith("webdriver-profile"))
            .collect(Collectors.toSet());
    assertThat(profile.toJson()).isNotNull();
    Set<String> after =
        Arrays.stream(sysTemp.list())
            .filter(f -> f.endsWith("webdriver-profile"))
            .collect(Collectors.toSet());
    assertThat(after).isEqualTo(before);
  }

  @Test
  void shouldConvertItselfIntoAMeaningfulRepresentation() throws IOException {
    profile.setPreference("i.like.cheese", true);

    String json = profile.toJson();

    assertThat(json).isNotNull();

    File dir = Zip.unzipToTempDir(json, "webdriver", "duplicated");

    File prefs = new File(dir, "user.js");
    assertThat(prefs).exists();

    try (Stream<String> lines = Files.lines(prefs.toPath())) {
      assertThat(lines.anyMatch(s -> s.contains("i.like.cheese"))).isTrue();
    }

    FileHandler.delete(dir);
  }

  private List<String> readGeneratedProperties(FirefoxProfile profile) throws Exception {
    File generatedProfile = profile.layoutOnDisk();

    File prefs = new File(generatedProfile, "user.js");
    BufferedReader reader = new BufferedReader(new FileReader(prefs));

    List<String> prefLines = new ArrayList<>();
    for (String line = reader.readLine(); line != null; line = reader.readLine()) {
      prefLines.add(line);
    }

    reader.close();

    return prefLines;
  }

  @Test
  void layoutOnDiskSetsUserPreferences() throws IOException {
    profile.setPreference("browser.startup.homepage", "http://www.example.com");
    Preferences parsedPrefs = parseUserPrefs(profile);
    assertThat(parsedPrefs.getPreference("browser.startup.homepage"))
        .isEqualTo("http://www.example.com");
  }

  @Test
  void userPrefsArePreservedWhenConvertingToAndFromJson() throws IOException {
    profile.setPreference("browser.startup.homepage", "http://www.example.com");

    String json = profile.toJson();
    FirefoxProfile rebuilt = FirefoxProfile.fromJson(json);
    Preferences parsedPrefs = parseUserPrefs(rebuilt);

    assertThat(parsedPrefs.getPreference("browser.startup.homepage"))
        .isEqualTo("http://www.example.com");
  }

  @Test
  void backslashedCharsArePreservedWhenConvertingToAndFromJson() throws IOException {
    String dir =
        "c:\\aaa\\bbb\\ccc\\ddd\\eee\\fff\\ggg\\hhh\\iii\\jjj\\kkk\\lll\\mmm\\n"
            + "nn\\ooo\\ppp\\qqq\\r"
            + "rr\\sss\\ttt\\uuu\\vvv\\www\\xxx\\yyy\\zzz";
    profile.setPreference("browser.download.dir", dir);

    String json = profile.toJson();
    FirefoxProfile rebuilt = FirefoxProfile.fromJson(json);
    Preferences parsedPrefs = parseUserPrefs(rebuilt);

    assertThat(parsedPrefs.getPreference("browser.download.dir")).isEqualTo(dir);
  }

  private void assertPreferenceValueEquals(String key, Object value) throws Exception {
    List<String> props = readGeneratedProperties(profile);
    assertThat(
            props.stream()
                .anyMatch(line -> line.contains(key) && line.contains(", " + value + ")")))
        .isTrue();
  }

  private Preferences parseUserPrefs(FirefoxProfile profile) throws IOException {
    File directory = profile.layoutOnDisk();
    File userPrefs = new File(directory, "user.js");
    FileReader reader = new FileReader(userPrefs);
    return new Preferences(new StringReader("{\"mutable\": {}, \"frozen\": {}}"), reader);
  }
}
