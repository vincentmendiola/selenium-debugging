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

import io.opentelemetry.api.trace.SpanContext;
import io.opentelemetry.api.trace.StatusCode;
import io.opentelemetry.api.trace.Tracer;
import io.opentelemetry.context.Context;
import io.opentelemetry.context.Scope;
import java.util.Map;
import java.util.Objects;
import org.openqa.selenium.internal.Require;
import org.openqa.selenium.remote.tracing.AttributeMap;
import org.openqa.selenium.remote.tracing.Span;
import org.openqa.selenium.remote.tracing.Status;

class OpenTelemetrySpan extends OpenTelemetryContext implements AutoCloseable, Span {

  private final io.opentelemetry.api.trace.Span span;
  private final Scope scope;

  public OpenTelemetrySpan(
      Tracer tracer, Context context, io.opentelemetry.api.trace.Span span, Scope scope) {
    super(tracer, context);
    this.span = Require.nonNull("Span", span);
    this.scope = Require.nonNull("Scope", scope);
  }

  @Override
  public Span setName(String name) {
    span.updateName(Require.nonNull("Name to update to", name));
    return this;
  }

  @Override
  public Span setAttribute(String key, boolean value) {
    span.setAttribute(Require.nonNull("Key", key), value);
    return this;
  }

  @Override
  public Span setAttribute(String key, Number value) {
    Require.nonNull("Key", key);
    Require.nonNull("Value", value);

    if (value instanceof Double || value instanceof Float) {
      span.setAttribute(key, value.doubleValue());
    } else {
      span.setAttribute(key, value.longValue());
    }

    return this;
  }

  @Override
  public Span setAttribute(String key, String value) {
    Require.nonNull("Key", key);
    Require.nonNull("Value", value);
    span.setAttribute(key, value);
    return this;
  }

  @Override
  public Span addEvent(String name) {
    Require.nonNull("Name", name);
    span.addEvent(name);
    return this;
  }

  @Override
  public Span addEvent(String name, AttributeMap attributeMap) {
    Require.nonNull("Name", name);
    Require.nonNull("Event Attribute Map", attributeMap);

    span.addEvent(name, ((OpenTelemetryAttributeMap) attributeMap).build());
    return this;
  }

  private static final Map<Status.Kind, StatusCode> statuses =
      Map.ofEntries(
          Map.entry(Status.Kind.ABORTED, StatusCode.ERROR),
          Map.entry(Status.Kind.CANCELLED, StatusCode.ERROR),
          Map.entry(Status.Kind.NOT_FOUND, StatusCode.ERROR),
          Map.entry(Status.Kind.OK, StatusCode.OK),
          Map.entry(Status.Kind.RESOURCE_EXHAUSTED, StatusCode.ERROR),
          Map.entry(Status.Kind.UNKNOWN, StatusCode.ERROR),
          Map.entry(Status.Kind.INVALID_ARGUMENT, StatusCode.ERROR),
          Map.entry(Status.Kind.DEADLINE_EXCEEDED, StatusCode.ERROR),
          Map.entry(Status.Kind.ALREADY_EXISTS, StatusCode.ERROR),
          Map.entry(Status.Kind.PERMISSION_DENIED, StatusCode.ERROR),
          Map.entry(Status.Kind.OUT_OF_RANGE, StatusCode.ERROR),
          Map.entry(Status.Kind.UNIMPLEMENTED, StatusCode.ERROR),
          Map.entry(Status.Kind.INTERNAL, StatusCode.ERROR),
          Map.entry(Status.Kind.UNAVAILABLE, StatusCode.ERROR),
          Map.entry(Status.Kind.UNAUTHENTICATED, StatusCode.ERROR));

  @Override
  public Span setStatus(Status status) {
    Require.nonNull("Status", status);

    StatusCode statusCode = statuses.get(status.getKind());
    if (statusCode == null) {
      throw new IllegalArgumentException("Unrecognized status kind: " + status.getKind());
    }

    span.setStatus(
        statusCode,
        "Kind: " + status.getKind().toString() + " Description:" + status.getDescription());

    return this;
  }

  @Override
  public void close() {
    scope.close();
    span.end();
  }

  @Override
  public String toString() {
    SpanContext context = span.getSpanContext();

    return "OpenTelemetrySpan{traceId="
        + context.getTraceId()
        + ",spanId="
        + context.getSpanId()
        + "}";
  }

  @Override
  public boolean equals(Object o) {
    if (o instanceof OpenTelemetryContext && (!(o instanceof OpenTelemetrySpan))) {
      return false;
    }

    if (!(o instanceof OpenTelemetrySpan)) {
      return false;
    }

    OpenTelemetrySpan that = (OpenTelemetrySpan) o;
    SpanContext thisContext = this.span.getSpanContext();
    SpanContext thatContext = that.span.getSpanContext();

    return Objects.equals(thisContext.getSpanId(), thatContext.getSpanId())
        && Objects.equals(thisContext.getTraceId(), thatContext.getTraceId());
  }

  @Override
  public int hashCode() {
    return Objects.hash(span);
  }
}
