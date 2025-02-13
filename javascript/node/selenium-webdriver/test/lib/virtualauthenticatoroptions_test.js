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

'use strict'

const assert = require('node:assert')
const virtualAuthenticatorOptions = require('selenium-webdriver/lib/virtual_authenticator').VirtualAuthenticatorOptions
const Transport = require('selenium-webdriver/lib/virtual_authenticator').Transport
const Protocol = require('selenium-webdriver/lib/virtual_authenticator').Protocol

let options

describe('VirtualAuthenticatorOptions', function () {
  beforeEach(function () {
    options = new virtualAuthenticatorOptions()
  })

  it('can testSetTransport', function () {
    options.setTransport(Transport['USB'])
    assert.equal(options.getTransport(), Transport['USB'])
  })

  it('can testGetTransport', function () {
    options._transport = Transport['NFC']
    assert.equal(options.getTransport(), Transport['NFC'])
  })

  it('can testSetProtocol', function () {
    options.setProtocol(Protocol['U2F'])
    assert.equal(options.getProtocol(), Protocol['U2F'])
  })

  it('can testGetProtocol', function () {
    options._protocol = Protocol['CTAP2']
    assert.equal(options.getProtocol(), Protocol['CTAP2'])
  })

  it('can testSetHasResidentKey', function () {
    options.setHasResidentKey(true)
    assert.equal(options.getHasResidentKey(), true)
  })

  it('can testGetHasResidentKey', function () {
    options._hasResidentKey = false
    assert.equal(options.getHasResidentKey(), false)
  })

  it('can testSetHasUserVerification', function () {
    options.setHasUserVerification(true)
    assert.equal(options.getHasUserVerification(), true)
  })

  it('can testGetHasUserVerification', function () {
    options._hasUserVerification = false
    assert.equal(options.getHasUserVerification(), false)
  })

  it('can testSetIsUserConsenting', function () {
    options.setIsUserConsenting(true)
    assert.equal(options.getIsUserConsenting(), true)
  })

  it('can testGetIsUserConsenting', function () {
    options._isUserConsenting = false
    assert.equal(options.getIsUserConsenting(), false)
  })

  it('can testSetIsUserVerified', function () {
    options.setIsUserVerified(true)
    assert.equal(options.getIsUserVerified(), true)
  })

  it('can testGetIsUserVerified', function () {
    options._isUserVerified = false
    assert.equal(options.getIsUserVerified(), false)
  })

  it('can testToDictWithDefaults', function () {
    let default_options = options.toDict()
    assert.equal(default_options['transport'], Transport['USB'])
    assert.equal(default_options['protocol'], Protocol['CTAP2'])
    assert.equal(default_options['hasResidentKey'], false)
    assert.equal(default_options['hasUserVerification'], false)
    assert.equal(default_options['isUserConsenting'], true)
    assert.equal(default_options['isUserVerified'], false)
  })
})
