# frozen_string_literal: true

# Licensed to the Software Freedom Conservancy (SFC) under one
# or more contributor license agreements.  See the NOTICE file
# distributed with this work for additional information
# regarding copyright ownership.  The SFC licenses this file
# to you under the Apache License, Version 2.0 (the
# "License"); you may not use this file except in compliance
# with the License.  You may obtain a copy of the License at
#
#   http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing,
# software distributed under the License is distributed on an
# "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
# KIND, either express or implied.  See the License for the
# specific language governing permissions and limitations
# under the License.

require_relative 'spec_helper'

module Selenium
  module WebDriver
    describe Manager, exclusive: {bidi: false, reason: 'Not yet implemented with BiDi'} do
      describe 'cookie management' do
        before { driver.navigate.to url_for('xhtmlTest.html') }

        after { driver.manage.delete_all_cookies }

        it 'sets correct defaults' do
          driver.manage.add_cookie name: 'default',
                                   value: 'value'

          cookie = driver.manage.cookie_named('default')
          expect(cookie[:value]).to eq('value')
          expect(cookie[:path]).to eq('/')
          expect(cookie[:domain]).to eq('localhost')
          expect(cookie[:http_only]).to be(false)
          expect(cookie[:secure]).to be(false)
        end

        it 'sets samesite property of Lax by default',
           except: {browser: :firefox,
                    reason: 'https://github.com/mozilla/geckodriver/issues/1841'},
           only: {browser: %i[chrome edge firefox]} do
          driver.manage.add_cookie name: 'samesite',
                                   value: 'default'

          expect(driver.manage.cookie_named('samesite')[:same_site]).to eq('Lax')
        end

        it 'respects path' do
          driver.manage.add_cookie name: 'path',
                                   value: 'specified',
                                   path: '/child'

          expect(driver.manage.all_cookies.size).to eq(0)

          driver.navigate.to url_for('child/childPage.html')

          expect(driver.manage.cookie_named('path')[:path]).to eq '/child'
        end

        it 'respects setting on domain from a subdomain',
           exclusive: {driver: :none,
                       reason: 'Can only be tested on site with subdomains'} do
          driver.get('https://opensource.saucelabs.com')

          driver.manage.add_cookie name: 'domain',
                                   value: 'specified',
                                   domain: 'saucelabs.com'

          expect(driver.manage.cookie_named('domain')[:domain]).to eq('.saucelabs.com')

          driver.get('https://accounts.saucelabs.com')
          expect(driver.manage.cookie_named('domain')[:domain]).to eq('.saucelabs.com')

          driver.get('https://saucelabs.com')
          expect(driver.manage.cookie_named('domain')[:domain]).to eq('.saucelabs.com')
        end

        it 'does not allow setting on a different domain', except: {browser: %i[safari safari_preview]} do
          expect {
            driver.manage.add_cookie name: 'domain',
                                     value: 'different',
                                     domain: 'selenium.dev'
          }.to raise_error(Error::InvalidCookieDomainError)
        end

        it 'does not allow setting on a subdomain from parent domain',
           exclusive: {driver: :none,
                       reason: 'Can not run on our test server; needs subdomains'} do
          driver.get('https://saucelabs.com')

          expect {
            driver.manage.add_cookie name: 'domain',
                                     value: 'subdomain',
                                     domain: 'opensource.saucelabs.com'
          }.to raise_exception(Error::InvalidCookieDomainError)
        end

        it 'is not visible to javascript when http_only is true' do
          driver.manage.add_cookie name: 'httponly',
                                   value: 'true',
                                   http_only: true

          expect(driver.execute_script('return document.cookie')).to be_empty
          expect(driver.manage.cookie_named('httponly')[:http_only]).to be true
        end

        it 'does not add secure cookie when http',
           except: {browser: :firefox,
                    reason: 'https://github.com/mozilla/geckodriver/issues/1840'},
           exclusive: {driver: :none,
                       reason: 'Cannot be tested on localhost'} do
          driver.get 'http://watir.com'
          driver.manage.add_cookie name: 'secure',
                                   value: 'http',
                                   secure: true

          expect(driver.manage.all_cookies.size).to eq(0)
        end

        it 'adds secure cookie when https',
           exclusive: {driver: :none,
                       reason: 'Can only be tested on https site'} do
          driver.get 'https://www.selenium.dev'

          driver.manage.add_cookie name: 'secure',
                                   value: 'https',
                                   secure: true

          expect(driver.manage.cookie_named('secure')[:secure]).to be(true)
        end

        describe 'sameSite' do
          it 'allows adding with value Strict' do
            driver.manage.add_cookie name: 'samesite',
                                     value: 'strict',
                                     same_site: 'Strict'

            expect(driver.manage.cookie_named('samesite')[:same_site]).to eq('Strict')
          end

          it 'allows adding with value Lax' do
            driver.manage.add_cookie name: 'samesite',
                                     value: 'lax',
                                     same_site: 'Lax'
            expect(driver.manage.cookie_named('samesite')[:same_site]).to eq('Lax')
          end

          it 'allows adding with value None',
             exclusive: {driver: :none,
                         reason: 'Can only be tested on https site'} do
            driver.get 'https://selenium.dev'

            driver.manage.add_cookie name: 'samesite',
                                     value: 'none-secure',
                                     same_site: 'None',
                                     secure: true

            expect(driver.manage.cookie_named('samesite')[:same_site]).to eq('None')
          end

          it 'does not allow adding with value None when secure is false',
             except: [{browser: :firefox,
                       reason: 'https://github.com/mozilla/geckodriver/issues/1842'},
                      {browser: %i[safari safari_preview]}] do
            expect {
              driver.manage.add_cookie name: 'samesite',
                                       value: 'none-insecure',
                                       same_site: 'None',
                                       secure: false
            }.to raise_exception(Error::UnableToSetCookieError)
          end
        end

        describe 'expiration' do
          it 'allows adding with DateTime value' do
            expected = (Date.today + 2).to_datetime
            driver.manage.add_cookie name: 'expiration',
                                     value: 'datetime',
                                     expires: expected

            actual = driver.manage.cookie_named('expiration')[:expires]
            expect(actual).to be_a(DateTime)
            expect(actual).to eq(expected)
          end

          it 'allows adding with Time value' do
            expected = (Date.today + 2).to_datetime
            driver.manage.add_cookie name: 'expiration',
                                     value: 'time',
                                     expires: expected.to_time

            actual = driver.manage.cookie_named('expiration')[:expires]
            expect(actual).to be_a(DateTime)
            expect(actual).to eq(expected)
          end

          it 'allows adding with Number value' do
            expected = (Date.today + 2).to_datetime
            driver.manage.add_cookie name: 'expiration',
                                     value: 'number',
                                     expires: expected.to_time.to_f

            actual = driver.manage.cookie_named('expiration')[:expires]
            expect(actual).to be_a(DateTime)
            expect(actual).to eq(expected)
          end

          it 'does not allow adding when value is in the past' do
            expected = (Date.today - 2).to_datetime
            driver.manage.add_cookie name: 'expiration',
                                     value: 'datetime',
                                     expires: expected

            expect(driver.manage.all_cookies.size).to eq(0)
          end
        end

        it 'gets one' do
          driver.manage.add_cookie name: 'foo', value: 'bar'

          expect(driver.manage.cookie_named('foo')[:value]).to eq('bar')
        end

        it 'gets all' do
          driver.manage.add_cookie name: 'foo', value: 'bar'

          cookies = driver.manage.all_cookies

          expect(cookies.size).to eq(1)
          expect(cookies.first[:name]).to eq('foo')
          expect(cookies.first[:value]).to eq('bar')
        end

        it 'deletes one' do
          driver.manage.add_cookie name: 'foo', value: 'bar'

          driver.manage.delete_cookie('foo')
          expect(driver.manage.all_cookies.find { |c| c[:name] == 'foo' }).to be_nil
        end

        it 'deletes all' do
          driver.manage.add_cookie name: 'foo', value: 'bar'
          driver.manage.add_cookie name: 'bar', value: 'foo'
          driver.manage.delete_all_cookies
          expect(driver.manage.all_cookies).to be_empty
        end

        it 'throws error when fetching non-existent cookie' do
          expect { driver.manage.cookie_named('non-existent') }
            .to raise_exception(Error::NoSuchCookieError)
        end
      end
    end # Options
  end # WebDriver
end # Selenium
