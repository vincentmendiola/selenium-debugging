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

package org.openqa.selenium.remote;

import static org.mockito.ArgumentMatchers.anyString;
import static org.mockito.Mockito.times;
import static org.mockito.Mockito.verify;
import static org.mockito.Mockito.verifyNoMoreInteractions;
import static org.mockito.Mockito.when;

import java.io.IOException;
import java.util.HashMap;
import java.util.Map;
import java.util.UUID;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Tag;
import org.junit.jupiter.api.Test;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;
import org.openqa.selenium.remote.tracing.Span;
import org.openqa.selenium.remote.tracing.TraceContext;
import org.openqa.selenium.remote.tracing.Tracer;

@Tag("UnitTests")
class TracedCommandExecutorTest {
  @Mock private CommandExecutor commandExecutor;
  @Mock private Tracer tracer;
  @Mock private TraceContext traceContext;
  @Mock private Span span;

  private TracedCommandExecutor tracedCommandExecutor;

  @BeforeEach
  public void createMocksAndTracedCommandExecutor() {
    MockitoAnnotations.initMocks(this);
    when(tracer.getCurrentContext()).thenReturn(traceContext);
    when(traceContext.createSpan(anyString())).thenReturn(span);
    tracedCommandExecutor = new TracedCommandExecutor(commandExecutor, tracer);
  }

  @Test
  void canCreateSpanWithAllAttributes() throws IOException {
    SessionId sessionId = new SessionId(UUID.randomUUID());
    Map<String, Object> parameters = new HashMap<>();
    parameters.put("param1", "value1");
    parameters.put("param2", "value2");
    Command command = new Command(sessionId, "findElement", parameters);

    tracedCommandExecutor.execute(command);

    verify(span, times(1)).setAttribute("sessionId", sessionId.toString());
    verify(span, times(1)).setAttribute("command", "findElement");
    verify(span, times(1)).setAttribute("parameter.param1", "value1");
    verify(span, times(1)).setAttribute("parameter.param2", "value2");
    verify(span, times(1)).close();
    verifyNoMoreInteractions(span);
  }

  @Test
  void canCreateSpanFromNullParameter() throws IOException {
    SessionId sessionId = new SessionId(UUID.randomUUID());
    Map<String, Object> parameters = new HashMap<>();
    parameters.put("param1", null);
    Command command = new Command(sessionId, "findElement", parameters);

    tracedCommandExecutor.execute(command);

    verify(span, times(1)).setAttribute("sessionId", sessionId.toString());
    verify(span, times(1)).setAttribute("command", "findElement");
    verify(span, times(1)).setAttribute("parameter.param1", "null");
    verify(span, times(1)).close();
    verifyNoMoreInteractions(span);
  }

  @Test
  void canCreateSpanWithSessionIdAndCommandName() throws IOException {
    SessionId sessionId = new SessionId(UUID.randomUUID());
    Command command = new Command(sessionId, "findElement");

    tracedCommandExecutor.execute(command);

    verify(span, times(1)).setAttribute("sessionId", sessionId.toString());
    verify(span, times(1)).setAttribute("command", "findElement");
    verify(span, times(1)).close();
    verifyNoMoreInteractions(span);
  }

  @Test
  void canCreateSpanWithCommandName() throws IOException {
    Command command = new Command(null, "createSession");

    tracedCommandExecutor.execute(command);

    verify(span, times(1)).setAttribute("command", "createSession");
    verify(span, times(1)).close();
    verifyNoMoreInteractions(span);
  }

  @Test
  void canCreateSpanWithCommandNameAsSpanName() throws IOException {
    SessionId sessionId = new SessionId(UUID.randomUUID());
    Command command = new Command(sessionId, "findElement");

    tracedCommandExecutor.execute(command);

    verify(traceContext).createSpan("findElement");
    verify(span).setAttribute("sessionId", sessionId.toString());
    verify(span).setAttribute("command", "findElement");
    verify(span).close();
    verifyNoMoreInteractions(span);
  }
}
