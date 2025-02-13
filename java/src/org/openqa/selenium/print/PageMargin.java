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

package org.openqa.selenium.print;

import java.util.HashMap;
import java.util.Map;
import org.jspecify.annotations.NullMarked;

@NullMarked
public class PageMargin {
  private final double top;
  private final double bottom;
  private final double left;
  private final double right;

  public PageMargin() {
    this.top = 1.0;
    this.bottom = 1.0;
    this.left = 1.0;
    this.right = 1.0;
  }

  public PageMargin(double top, double bottom, double left, double right) {
    this.top = top;
    this.bottom = bottom;
    this.left = left;
    this.right = right;
  }

  public double getTop() {
    return top;
  }

  public double getBottom() {
    return bottom;
  }

  public double getLeft() {
    return left;
  }

  public double getRight() {
    return right;
  }

  public Map<String, Object> toMap() {
    final Map<String, Object> options = new HashMap<>(7);
    options.put("top", getTop());
    options.put("bottom", getBottom());
    options.put("left", getLeft());
    options.put("right", getRight());

    return options;
  }
}
