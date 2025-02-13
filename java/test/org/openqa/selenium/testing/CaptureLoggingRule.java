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

package org.openqa.selenium.testing;

import static java.util.stream.Collectors.toList;
import static org.openqa.selenium.internal.Debug.isDebugging;

import java.io.PrintWriter;
import java.io.StringWriter;
import java.time.Instant;
import java.time.LocalDateTime;
import java.time.ZoneId;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.logging.ConsoleHandler;
import java.util.logging.Formatter;
import java.util.logging.Handler;
import java.util.logging.LogManager;
import java.util.logging.LogRecord;
import java.util.logging.Logger;

class CaptureLoggingRule {

  List<Handler> handlers;

  public CaptureLoggingRule() {
    if (isDebugging()) {
      return;
    }

    Logger logger = LogManager.getLogManager().getLogger("");

    // Capture the original log handlers
    handlers =
        Arrays.stream(logger.getHandlers())
            .filter(handler -> handler instanceof ConsoleHandler)
            .collect(toList());

    // Remove them from the logger
    handlers.forEach(logger::removeHandler);

    // Replace them with log handlers that record messages
    logger.addHandler(new RecordingHandler());
  }

  public void writeCapturedLogs() {
    if (isDebugging()) {
      return;
    }

    Logger logger = LogManager.getLogManager().getLogger("");
    Arrays.stream(logger.getHandlers())
        .filter(handler -> handler instanceof RecordingHandler)
        .map(handler -> (RecordingHandler) handler)
        .forEach(RecordingHandler::write);
  }

  public void endLogCapture() {
    if (isDebugging()) {
      return;
    }

    // Find our recording handler
    Logger logger = LogManager.getLogManager().getLogger("");
    List<RecordingHandler> recordingHandlers =
        Arrays.stream(logger.getHandlers())
            .filter(handler -> handler instanceof RecordingHandler)
            .map(handler -> (RecordingHandler) handler)
            .collect(toList());

    recordingHandlers.forEach(logger::removeHandler);
  }

  private static class RecordingHandler extends Handler {

    private final List<LogRecord> records = new ArrayList<>();

    @Override
    public void publish(LogRecord record) {
      records.add(record);
    }

    @Override
    public void flush() {
      // no-op
    }

    @Override
    public void close() throws SecurityException {
      // no-op
    }

    public void write() {
      Formatter formatter = new OurFormatter();
      records.forEach(record -> System.out.print(formatter.format(record)));
    }
  }

  private static class OurFormatter extends Formatter {

    private final DateTimeFormatter dateFormat = DateTimeFormatter.ofPattern("HH:mm:ss.SSS");

    @Override
    public String format(LogRecord record) {
      StringBuilder buffer = new StringBuilder();
      LocalDateTime dateTime =
          LocalDateTime.ofInstant(Instant.ofEpochMilli(record.getMillis()), ZoneId.systemDefault());
      buffer.append(dateFormat.format(dateTime));
      buffer.append(' ');
      buffer.append(record.getLevel());
      if (record.getSourceClassName() != null) {
        String[] parts = record.getSourceClassName().split("\\.");
        buffer
            .append(" [")
            .append(parts[parts.length - 1])
            .append(".")
            .append(record.getSourceMethodName())
            .append("]");
      }
      buffer.append(" - ");
      buffer.append(formatMessage(record)).append(System.lineSeparator());
      if (record.getThrown() != null) {
        final StringWriter trace = new StringWriter();
        record.getThrown().printStackTrace(new PrintWriter(trace));
        buffer.append(trace);
      }

      return buffer.toString();
    }
  }
}
