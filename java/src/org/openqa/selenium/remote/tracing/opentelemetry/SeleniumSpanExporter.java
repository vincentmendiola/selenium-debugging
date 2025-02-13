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

package org.openqa.selenium.remote.tracing.opentelemetry;

import io.opentelemetry.api.common.AttributeKey;
import io.opentelemetry.api.common.Attributes;
import io.opentelemetry.api.trace.StatusCode;
import io.opentelemetry.sdk.common.CompletableResultCode;
import io.opentelemetry.sdk.trace.SpanProcessor;
import io.opentelemetry.sdk.trace.data.EventData;
import io.opentelemetry.sdk.trace.data.SpanData;
import io.opentelemetry.sdk.trace.export.SimpleSpanProcessor;
import io.opentelemetry.sdk.trace.export.SpanExporter;
import java.util.Collection;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Optional;
import java.util.Set;
import java.util.logging.Level;
import java.util.logging.Logger;
import org.openqa.selenium.json.Json;
import org.openqa.selenium.json.JsonOutput;
import org.openqa.selenium.remote.tracing.Span;

public class SeleniumSpanExporter {

  private static final Logger LOG = Logger.getLogger(SeleniumSpanExporter.class.getName());
  private static final Set<String> EXCEPTION_ATTRIBUTES =
      Set.of("exception.message", "exception.stacktrace");
  private static final boolean httpLogs = OpenTelemetryTracer.getHttpLogs();
  private static final AttributeKey<String> KEY_SPAN_KIND =
      AttributeKey.stringKey(org.openqa.selenium.remote.tracing.AttributeKey.SPAN_KIND.getKey());

  private static String getJsonString(Map<String, Object> map) {
    StringBuilder text = new StringBuilder();
    try (JsonOutput json = new Json().newOutput(text).setPrettyPrint(false)) {
      json.write(map);
      text.append('\n');
    }
    return text.toString();
  }

  public static SpanProcessor getSpanProcessor() {
    return SimpleSpanProcessor.create(
        new SpanExporter() {
          @Override
          public CompletableResultCode export(Collection<SpanData> spans) {
            spans.forEach(
                span -> {
                  if (LOG.isLoggable(Level.FINE)) {
                    LOG.fine(String.valueOf(span));
                  }

                  Level logLevel = getLogLevel(span);

                  if (!LOG.isLoggable(logLevel)) {
                    return;
                  }

                  String traceId = span.getTraceId();
                  List<EventData> eventList = span.getEvents();
                  eventList.forEach(
                      event -> {
                        Map<String, Object> map = new HashMap<>();
                        map.put("eventTime", event.getEpochNanos());
                        map.put("traceId", traceId);
                        map.put("eventName", event.getName());

                        Attributes attributes = event.getAttributes();
                        map.put("attributes", attributes.asMap());

                        EXCEPTION_ATTRIBUTES.forEach(
                            exceptionAttribute ->
                                attributes.asMap().keySet().stream()
                                    .filter(
                                        key -> exceptionAttribute.equalsIgnoreCase(key.getKey()))
                                    .findFirst()
                                    .ifPresent(
                                        key ->
                                            LOG.log(
                                                logLevel, attributes.asMap().get(key).toString())));
                        String jsonString = getJsonString(map);
                        LOG.log(logLevel, jsonString);
                      });
                });
            return CompletableResultCode.ofSuccess();
          }

          @Override
          public CompletableResultCode flush() {
            return CompletableResultCode.ofSuccess();
          }

          @Override
          public CompletableResultCode shutdown() {
            // no-op
            return CompletableResultCode.ofSuccess();
          }
        });
  }

  private static Level getLogLevel(SpanData span) {
    Level level = Level.FINE;

    if (span.getStatus().getStatusCode() == StatusCode.ERROR) {
      level = Level.WARNING;
    } else if (httpLogs) {
      Optional<String> kind = Optional.ofNullable(span.getAttributes().get(KEY_SPAN_KIND));

      if (kind.isPresent()) {
        String kindValue = kind.get();
        if (Span.Kind.SERVER.name().equalsIgnoreCase(kindValue)
            || Span.Kind.CLIENT.name().equalsIgnoreCase(kindValue)) {
          level = Level.INFO;
        }
      }
    }
    return level;
  }
}
