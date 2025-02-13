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

package org.openqa.selenium.grid.config;

import com.google.common.collect.ImmutableList;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.Optional;
import java.util.Set;
import java.util.logging.Logger;
import java.util.stream.Collectors;
import org.openqa.selenium.json.Json;

public interface Config {

  Set<String> getSectionNames();

  Set<String> getOptions(String section);

  Optional<List<String>> getAll(String section, String option);

  default Optional<String> get(String section, String option) {
    return getAll(section, option).map(items -> items.isEmpty() ? null : items.get(0));
  }

  default Optional<Integer> getInt(String section, String option) {
    return get(section, option).map(Integer::parseInt);
  }

  default Optional<Boolean> getBool(String section, String option) {
    return get(section, option).map(Boolean::parseBoolean);
  }

  default Optional<List<List<String>>> getArray(String section, String option) {
    Optional<List<String>> flatConfigs = getAll(section, option);
    if (!flatConfigs.isPresent()) {
      return Optional.empty();
    }

    List<String> configItem = new ArrayList<>();
    List<List<String>> configList = new ArrayList<>();
    for (String next : flatConfigs.get()) {
      if (Config.DELIMITER.equals(next)) {
        configList.add(configItem);
        configItem = new ArrayList<>();
      } else {
        configItem.add(next);
      }
    }
    return Optional.of(configList);
  }

  default <X> X getClass(String section, String option, Class<X> typeOfClass, String defaultClazz) {
    String clazz = get(section, option).orElse(defaultClazz);

    // We don't declare a constant on the interface, natch.
    Logger.getLogger(Config.class.getName())
        .fine(String.format("Creating %s as instance of %s", clazz, typeOfClass));

    try {
      return ClassCreation.callCreateMethod(clazz, typeOfClass, this);
    } catch (ReflectiveOperationException e) {
      throw new IllegalArgumentException("Unable to find class: " + clazz, e);
    }
  }

  String DELIM_KEY = "\u001E";
  String DELIMITER = DELIM_KEY + "=\"record-separator\"";

  default List<String> toEntryList(Map<String, Object> mapItem) {
    // transform config settings map into list of key/value strings
    List<String> entryList =
        mapItem.entrySet().stream()
            .map(
                entry -> {
                  return String.format("%s=%s", entry.getKey(), toJson(entry.getValue()));
                })
            .collect(Collectors.toList());
    // add record separator
    entryList.add(DELIMITER);
    // return immutable config settings list
    return ImmutableList.<String>builder().addAll(entryList).build();
  }

  default String toJson(Object value) {
    StringBuilder jsonStr = new StringBuilder();
    new Json().newOutput(jsonStr).setPrettyPrint(false).write(value);
    return jsonStr.toString();
  }
}
