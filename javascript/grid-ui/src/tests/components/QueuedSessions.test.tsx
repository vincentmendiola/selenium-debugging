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

import * as React from 'react'
import QueuedSessions from '../../components/QueuedSessions/QueuedSessions'
import { render, screen } from '@testing-library/react'

const sessionQueueRequests: string[] = [
  '{"acceptInsecureCerts":true,"browserName":"chrome","goog:chromeOptions":{"args":["--start-maximized"],"extensions":[]}}'
]

it('renders basic session information', () => {
  render(<QueuedSessions sessionQueueRequests={sessionQueueRequests} />)
  const browserLogo = screen.getByAltText('Browser Logo')
  expect(browserLogo).toBeInTheDocument()
  expect(browserLogo).toHaveAttribute('src', 'chrome.svg')
})
