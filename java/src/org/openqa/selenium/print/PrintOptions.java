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
import java.util.List;
import java.util.Map;
import org.jspecify.annotations.NullMarked;
import org.jspecify.annotations.Nullable;
import org.openqa.selenium.internal.Require;

@NullMarked
public class PrintOptions {

  public enum Orientation {
    PORTRAIT("portrait"),
    LANDSCAPE("landscape");

    private final String serialFormat;

    Orientation(String serialFormat) {
      this.serialFormat = serialFormat;
    }

    @Override
    public String toString() {
      return serialFormat;
    }
  }

  private Orientation orientation = Orientation.PORTRAIT;
  private double scale = 1.0;
  private boolean background = false;
  private boolean shrinkToFit = true;
  private PageSize pageSize = new PageSize();
  private PageMargin pageMargin = new PageMargin();
  private String @Nullable [] pageRanges;

  public Orientation getOrientation() {
    return this.orientation;
  }

  public void setOrientation(Orientation orientation) {
    this.orientation = Require.nonNull("orientation", orientation);
  }

  public String @Nullable [] getPageRanges() {
    return this.pageRanges;
  }

  public void setPageRanges(String firstRange, String... ranges) {
    Require.nonNull("pageRanges", firstRange);
    this.pageRanges =
        new String[ranges.length + 1]; // Need to add all ranges and the initial range too.

    this.pageRanges[0] = firstRange;

    if (ranges.length > 0) System.arraycopy(ranges, 0, this.pageRanges, 1, ranges.length - 1);
  }

  public void setPageRanges(List<String> ranges) {
    this.pageRanges = new String[ranges.size()];
    this.pageRanges = ranges.toArray(this.pageRanges);
  }

  public void setBackground(boolean background) {
    this.background = Require.nonNull("background", background);
  }

  public boolean getBackground() {
    return this.background;
  }

  public void setScale(double scale) {
    if (scale < 0.1 || scale > 2) {
      throw new IllegalArgumentException("Scale value should be between 0.1 and 2");
    }
    this.scale = scale;
  }

  public double getScale() {
    return this.scale;
  }

  public boolean getShrinkToFit() {
    return this.shrinkToFit;
  }

  public void setShrinkToFit(boolean value) {
    this.shrinkToFit = Require.nonNull("value", value);
  }

  public void setPageSize(PageSize pageSize) {
    this.pageSize = Require.nonNull("pageSize", pageSize);
  }

  public void setPageMargin(PageMargin margin) {
    this.pageMargin = Require.nonNull("margin", margin);
  }

  public PageSize getPageSize() {
    return this.pageSize;
  }

  public PageMargin getPageMargin() {
    return this.pageMargin;
  }

  public Map<String, Object> toMap() {
    final Map<String, Object> options = new HashMap<>(7);
    options.put("page", getPageSize().toMap());
    options.put("orientation", getOrientation().toString());
    options.put("scale", getScale());
    options.put("shrinkToFit", getShrinkToFit());
    options.put("background", getBackground());
    final String[] effectivePageRanges = getPageRanges();
    if (effectivePageRanges != null) {
      options.put("pageRanges", effectivePageRanges);
    }
    options.put("margin", getPageMargin().toMap());

    return options;
  }
}
