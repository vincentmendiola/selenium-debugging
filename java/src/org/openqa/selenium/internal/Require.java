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

package org.openqa.selenium.internal;

import java.io.File;
import java.nio.file.Files;
import java.nio.file.Path;
import java.time.Duration;
import java.util.Objects;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;

/**
 * A utility class to check arguments (preconditions) and state.
 *
 * <p>Examples of use:
 *
 * <pre>
 *   public void setActionWithTimeout(Action action delegate, int timeout) {
 *     this.action = Require.nonNull("Action", action);
 *     this.timeout = Require.positive("Timeout", timeout);
 *   }
 * </pre>
 */
@NullMarked
public final class Require {

  private static final String MUST_BE_SET = "%s must be set";
  private static final String MUST_EXIST = "%s must exist: %s";
  private static final String MUST_BE_DIR = "%s must be a directory: %s";
  private static final String MUST_BE_FILE = "%s must be a regular file: %s";
  private static final String MUST_BE_EQUAL = "%s must be equal to `%s`";
  private static final String MUST_BE_EXECUTABLE = "%s must be executable: %s";
  private static final String MUST_BE_NON_NEGATIVE = "%s must be 0 or greater";
  private static final String MUST_BE_POSITIVE = "%s must be greater than 0";

  private Require() {
    // An utility class
  }

  public static void precondition(boolean condition, String message, Object... args) {
    if (!condition) {
      throw new IllegalArgumentException(String.format(message, args));
    }
  }

  public static <T> T nonNull(String argName, @Nullable T arg) {
    if (arg == null) {
      throw new IllegalArgumentException(String.format(MUST_BE_SET, argName));
    }
    return arg;
  }

  public static <T> T nonNull(String argName, @Nullable T arg, String message, Object... args) {
    if (arg == null) {
      throw new IllegalArgumentException(String.join(" ", argName, String.format(message, args)));
    }
    return arg;
  }

  public static <T> ArgumentChecker<T> argument(String argName, @Nullable T arg) {
    return new ArgumentChecker<>(argName, arg);
  }

  public static Duration nonNegative(String argName, @Nullable Duration arg) {
    if (arg == null) {
      throw new IllegalArgumentException(String.format(MUST_BE_SET, argName));
    }
    if (arg.isNegative()) {
      throw new IllegalArgumentException(String.format(MUST_BE_NON_NEGATIVE, argName));
    }
    return arg;
  }

  public static Duration nonNegative(@Nullable Duration arg) {
    if (arg == null) {
      throw new IllegalArgumentException(String.format(MUST_BE_SET, "Duration"));
    }
    if (arg.isNegative()) {
      throw new IllegalArgumentException(String.format(MUST_BE_NON_NEGATIVE, "Duration"));
    }
    return arg;
  }

  public static Duration positive(String argName, @Nullable Duration arg) {
    if (arg == null) {
      throw new IllegalArgumentException(String.format(MUST_BE_SET, argName));
    }
    if (arg.isNegative() || arg.isZero()) {
      throw new IllegalArgumentException(String.format(MUST_BE_POSITIVE, argName));
    }
    return arg;
  }

  public static Duration positive(@Nullable Duration arg) {
    if (arg == null) {
      throw new IllegalArgumentException(String.format(MUST_BE_SET, "Duration"));
    }
    if (arg.isNegative() || arg.isZero()) {
      throw new IllegalArgumentException(String.format(MUST_BE_POSITIVE, "Duration"));
    }
    return arg;
  }

  public static int nonNegative(String argName, @Nullable Integer number) {
    if (number == null) {
      throw new IllegalArgumentException(String.format(MUST_BE_SET, argName));
    }
    if (number < 0) {
      throw new IllegalArgumentException(String.format(MUST_BE_NON_NEGATIVE, argName));
    }
    return number;
  }

  public static int positive(String argName, @Nullable Integer number, @Nullable String message) {
    if (number == null) {
      throw new IllegalArgumentException(String.format(MUST_BE_SET, argName));
    }
    if (number <= 0) {
      throw new IllegalArgumentException(
          Objects.requireNonNullElseGet(message, () -> String.format(MUST_BE_POSITIVE, argName)));
    }
    return number;
  }

  public static double positive(String argName, @Nullable Double number, @Nullable String message) {
    if (number == null) {
      throw new IllegalArgumentException(String.format(MUST_BE_SET, argName));
    }
    if (number <= 0) {
      throw new IllegalArgumentException(
          Objects.requireNonNullElseGet(message, () -> String.format(MUST_BE_POSITIVE, argName)));
    }
    return number;
  }

  public static double positive(String argName, @Nullable Double number) {
    return positive(argName, number, null);
  }

  public static int positive(String argName, @Nullable Integer number) {
    return positive(argName, number, null);
  }

  public static IntChecker argument(String argName, @Nullable Integer number) {
    return new IntChecker(argName, number);
  }

  @Deprecated(forRemoval = true)
  public static FileChecker argument(String argName, @Nullable File file) {
    return new FileChecker(argName, file);
  }

  public static PathChecker argument(String argName, @Nullable Path path) {
    return new PathChecker(argName, path);
  }

  public static void stateCondition(boolean state, String message, Object... args) {
    if (!state) {
      throw new IllegalStateException(String.format(message, args));
    }
  }

  public static <T> StateChecker<T> state(String name, @Nullable T state) {
    return new StateChecker<>(name, state);
  }

  @Deprecated(forRemoval = true)
  public static FileStateChecker state(String name, @Nullable File file) {
    return new FileStateChecker(name, file);
  }

  public static PathStateChecker state(String name, @Nullable Path path) {
    return new PathStateChecker(name, path);
  }

  public static class ArgumentChecker<T> {

    private final String argName;
    private final @Nullable T arg;

    ArgumentChecker(String argName, @Nullable T arg) {
      this.argName = argName;
      this.arg = arg;
    }

    public T nonNull() {
      if (arg == null) {
        throw new IllegalArgumentException(String.format(MUST_BE_SET, argName));
      }
      return arg;
    }

    public T nonNull(String message, Object... args) {
      if (arg == null) {
        throw new IllegalArgumentException(String.format(message, args));
      }
      return arg;
    }

    public T equalTo(Object other) {
      if (arg == null) {
        throw new IllegalArgumentException(String.format(MUST_BE_SET, argName));
      }
      if (!Objects.equals(arg, other)) {
        throw new IllegalArgumentException(String.format(MUST_BE_EQUAL, argName, other));
      }
      return arg;
    }

    public T instanceOf(Class<?> cls) {
      if (arg == null) {
        throw new IllegalArgumentException(String.format(MUST_BE_SET, argName));
      }
      if (!cls.isInstance(arg)) {
        throw new IllegalArgumentException(argName + " must be an instance of " + cls);
      }
      return arg;
    }
  }

  public static class IntChecker {

    private final String argName;
    private final @Nullable Integer number;

    IntChecker(String argName, @Nullable Integer number) {
      this.argName = argName;
      this.number = number;
    }

    public int greaterThan(int max, String message) {
      if (number == null) {
        throw new IllegalArgumentException(String.format(MUST_BE_SET, argName));
      }
      if (number <= max) {
        throw new IllegalArgumentException(message);
      }
      return number;
    }
  }

  @Deprecated(forRemoval = true)
  public static class FileChecker {

    private final String argName;
    private final @Nullable File file;

    FileChecker(String argName, @Nullable File file) {
      this.argName = argName;
      this.file = file;
    }

    public File isFile() {
      if (file == null) {
        throw new IllegalArgumentException(String.format(MUST_BE_SET, argName));
      }
      if (!file.exists()) {
        throw new IllegalArgumentException(
            String.format(MUST_EXIST, argName, file.getAbsolutePath()));
      }
      if (!file.isFile()) {
        throw new IllegalArgumentException(
            String.format(MUST_BE_FILE, argName, file.getAbsolutePath()));
      }
      return file;
    }

    public File isDirectory() {
      if (file == null) {
        throw new IllegalArgumentException(String.format(MUST_BE_SET, argName));
      }
      if (!file.exists()) {
        throw new IllegalArgumentException(
            String.format(MUST_EXIST, argName, file.getAbsolutePath()));
      }
      if (!file.isDirectory()) {
        throw new IllegalArgumentException(
            String.format(MUST_BE_DIR, argName, file.getAbsolutePath()));
      }
      return file;
    }
  }

  public static class PathChecker {

    private final String argName;
    private final @Nullable Path path;

    PathChecker(String argName, @Nullable Path path) {
      this.argName = argName;
      this.path = path;
    }

    public Path isFile() {
      if (path == null) {
        throw new IllegalArgumentException(String.format(MUST_BE_SET, argName));
      }
      if (!Files.exists(path)) {
        throw new IllegalArgumentException(
            String.format(MUST_EXIST, argName, path.toAbsolutePath()));
      }
      if (!Files.isRegularFile(path)) {
        throw new IllegalArgumentException(
            String.format(MUST_BE_FILE, argName, path.toAbsolutePath()));
      }
      return path;
    }

    public Path isDirectory() {
      if (path == null) {
        throw new IllegalArgumentException(String.format(MUST_BE_SET, argName));
      }
      if (!Files.exists(path)) {
        throw new IllegalArgumentException(
            String.format(MUST_EXIST, argName, path.toAbsolutePath()));
      }
      if (!Files.isDirectory(path)) {
        throw new IllegalArgumentException(
            String.format(MUST_BE_DIR, argName, path.toAbsolutePath()));
      }
      return path;
    }
  }

  public static class StateChecker<T> {

    private final String name;
    private final @Nullable T state;

    StateChecker(String name, @Nullable T state) {
      this.name = name;
      this.state = state;
    }

    public T nonNull() {
      if (state == null) {
        throw new IllegalStateException(name + " must not be null");
      }
      return state;
    }

    public T nonNull(String message, Object... args) {
      if (state == null) {
        throw new IllegalStateException(String.join(" ", name, String.format(message, args)));
      }
      return state;
    }

    public T instanceOf(Class<?> cls) {
      if (state == null) {
        throw new IllegalStateException(String.format(MUST_BE_SET, name));
      }
      if (!cls.isInstance(state)) {
        throw new IllegalStateException(name + " must be an instance of " + cls);
      }
      return state;
    }
  }

  @Deprecated(forRemoval = true)
  public static class FileStateChecker {

    private final String name;
    private final @Nullable File file;

    FileStateChecker(String name, @Nullable File file) {
      this.name = name;
      this.file = file;
    }

    public File isFile() {
      if (file == null) {
        throw new IllegalStateException(String.format(MUST_BE_SET, name));
      }
      if (!file.exists()) {
        throw new IllegalStateException(String.format(MUST_EXIST, name, file.getAbsolutePath()));
      }
      if (!file.isFile()) {
        throw new IllegalStateException(String.format(MUST_BE_FILE, name, file.getAbsolutePath()));
      }
      return file;
    }

    public File isDirectory() {
      if (file == null) {
        throw new IllegalStateException(String.format(MUST_BE_SET, name));
      }
      if (!file.exists()) {
        throw new IllegalStateException(String.format(MUST_EXIST, name, file.getAbsolutePath()));
      }
      if (!file.isDirectory()) {
        throw new IllegalStateException(String.format(MUST_BE_DIR, name, file.getAbsolutePath()));
      }
      return file;
    }

    public File isExecutable() {
      if (file == null) {
        throw new IllegalStateException(String.format(MUST_BE_SET, name));
      }
      if (!file.exists()) {
        throw new IllegalStateException(String.format(MUST_EXIST, name, file.getAbsolutePath()));
      }
      if (!file.canExecute()) {
        throw new IllegalStateException(
            String.format(MUST_BE_EXECUTABLE, name, file.getAbsolutePath()));
      }
      return file;
    }
  }

  public static class PathStateChecker {

    private final String name;
    private final @Nullable Path path;

    PathStateChecker(String name, @Nullable Path path) {
      this.name = name;
      this.path = path;
    }

    public Path isFile() {
      if (path == null) {
        throw new IllegalStateException(String.format(MUST_BE_SET, name));
      }
      // notExists returns false in case it is impossible to determinate the exact result of a link
      // target e.g. Windows app execution aliases
      if (Files.notExists(path)) {
        throw new IllegalStateException(String.format(MUST_EXIST, name, path));
      }
      if (!Files.isRegularFile(path)) {
        throw new IllegalStateException(String.format(MUST_BE_FILE, name, path));
      }
      return path;
    }

    public Path isDirectory() {
      if (path == null) {
        throw new IllegalStateException(String.format(MUST_BE_SET, name));
      }
      // notExists returns false in case it is impossible to determinate the exact result of a link
      // target e.g. Windows app execution aliases
      if (Files.notExists(path)) {
        throw new IllegalStateException(String.format(MUST_EXIST, name, path));
      }
      if (!Files.isDirectory(path)) {
        throw new IllegalStateException(String.format(MUST_BE_DIR, name, path));
      }
      return path;
    }

    public Path isExecutable() {
      if (path == null) {
        throw new IllegalStateException(String.format(MUST_BE_SET, name));
      }
      // notExists returns false in case it is impossible to determinate the exact result of a link
      // target e.g. Windows app execution aliases
      if (Files.notExists(path)) {
        throw new IllegalStateException(String.format(MUST_EXIST, name, path));
      }
      // do not check for isRegularFile here, there are executable none regular files e.g. Windows
      // app execution aliases
      if (!Files.isExecutable(path)) {
        throw new IllegalStateException(String.format(MUST_BE_EXECUTABLE, name, path));
      }
      return path;
    }
  }
}
