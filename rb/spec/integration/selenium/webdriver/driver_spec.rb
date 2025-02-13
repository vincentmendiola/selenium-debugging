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
    describe Driver, exclusive: {bidi: false, reason: 'Not yet implemented with BiDi'} do
      it_behaves_like 'driver that can be started concurrently', exclude: [
        {browser: %i[safari safari_preview]},
        {driver: :remote, rbe: true, reason: 'Cannot start 2+ drivers at once.'}
      ]

      it 'creates default capabilities', exclude: {browser: %i[safari safari_preview]} do
        reset_driver! do |driver|
          caps = driver.capabilities
          expect(caps.proxy).to be_nil
          expect(caps.browser_version).to match(/^\d\d\d?\./)
          expect(caps.platform_name).not_to be_nil

          expect(caps.accept_insecure_certs).to eq(caps.browser_name == 'firefox')
          expect(caps.page_load_strategy).to eq 'normal'
          expect(caps.implicit_timeout).to be_zero
          expect(caps.page_load_timeout).to eq 300000
          expect(caps.script_timeout).to eq 30000
        end
      end

      it 'gets driver status' do
        status = driver.status
        expect(status).to include('ready', 'message')
      end

      it 'gets the page title' do
        driver.navigate.to url_for('xhtmlTest.html')
        expect(driver.title).to eq('XHTML Test Page')
      end

      it 'gets the page source' do
        driver.navigate.to url_for('xhtmlTest.html')
        expect(driver.page_source).to match(%r{<title>XHTML Test Page</title>}i)
      end

      it 'refreshes the page' do
        driver.navigate.to url_for('javascriptPage.html')
        sleep 1 # javascript takes too long to load
        driver.find_element(id: 'updatediv').click
        expect(driver.find_element(id: 'dynamo').text).to eq('Fish and chips!')
        driver.navigate.refresh
        wait_for_element(id: 'dynamo')
        expect(driver.find_element(id: 'dynamo').text).to eq("What's for dinner?")
      end

      describe 'one element' do
        it 'finds by id' do
          driver.navigate.to url_for('xhtmlTest.html')
          element = driver.find_element(id: 'id1')
          expect(element).to be_a(WebDriver::Element)
          expect(element.text).to eq('Foo')
        end

        it 'finds by field name' do
          driver.navigate.to url_for('formPage.html')
          expect(driver.find_element(name: 'x').attribute('value')).to eq('name')
        end

        it 'finds by class name' do # rubocop:disable RSpec/RepeatedExample
          driver.navigate.to url_for('xhtmlTest.html')
          expect(driver.find_element(class: 'header').text).to eq('XHTML Might Be The Future')
        end

        # TODO: Rewrite this test so it's not a duplicate of above or remove
        it 'finds elements with a hash selector' do # rubocop:disable RSpec/RepeatedExample
          driver.navigate.to url_for('xhtmlTest.html')
          expect(driver.find_element(class: 'header').text).to eq('XHTML Might Be The Future')
        end

        it 'finds by link text' do
          driver.navigate.to url_for('xhtmlTest.html')
          expect(driver.find_element(link: 'Foo').text).to eq('Foo')
        end

        it 'finds by xpath' do
          driver.navigate.to url_for('xhtmlTest.html')
          expect(driver.find_element(xpath: '//h1').text).to eq('XHTML Might Be The Future')
        end

        it 'finds by css selector' do
          driver.navigate.to url_for('xhtmlTest.html')
          expect(driver.find_element(css: 'div.content').attribute('class')).to eq('content')
        end

        it 'finds by tag name' do
          driver.navigate.to url_for('xhtmlTest.html')
          expect(driver.find_element(tag_name: 'div').attribute('class')).to eq('navigation')
        end

        it 'finds above another' do
          driver.navigate.to url_for('relative_locators.html')

          above = driver.find_element(relative: {tag_name: 'td', above: {id: 'center'}})
          expect(above.attribute('id')).to eq('top')
        end

        it 'finds child element' do
          driver.navigate.to url_for('nestedElements.html')

          element = driver.find_element(name: 'form2')
          child = element.find_element(name: 'selectomatic')

          expect(child.attribute('id')).to eq('2')
        end

        it 'finds child element by tag name' do
          driver.navigate.to url_for('nestedElements.html')

          element = driver.find_element(name: 'form2')
          child = element.find_element(tag_name: 'select')

          expect(child.attribute('id')).to eq('2')
        end

        it 'finds elements with the shortcut syntax' do
          driver.navigate.to url_for('xhtmlTest.html')

          expect(driver[:id1]).to be_a(WebDriver::Element)
          expect(driver[xpath: '//h1']).to be_a(WebDriver::Element)
        end

        it 'raises if element not found' do
          driver.navigate.to url_for('xhtmlTest.html')
          expect {
            driver.find_element(id: 'not-there')
          }.to raise_error(Error::NoSuchElementError, /errors#no-such-element-exception/)
        end

        it 'raises if invalid locator',
           exclude: {browser: %i[safari safari_preview], reason: 'Safari TimeoutError'} do
          driver.navigate.to url_for('xhtmlTest.html')
          expect {
            driver.find_element(xpath: '*?//-')
          }.to raise_error(Error::InvalidSelectorError, /errors#invalid-selector-exception/)
        end
      end

      describe 'many elements' do
        it 'finds by class name' do
          driver.navigate.to url_for('xhtmlTest.html')
          expect(driver.find_elements(class: 'nameC').size).to eq(2)
        end

        it 'finds by css selector' do
          driver.navigate.to url_for('xhtmlTest.html')
          expect(driver.find_elements(css: 'p').size).to be_positive
        end

        it 'finds above element' do
          driver.navigate.to url_for('relative_locators.html')

          lowest = driver.find_element(id: 'below')
          above = driver.find_elements(relative: {tag_name: 'p', above: lowest})
          expect(above.map { |e| e.attribute('id') }).to eq(%w[mid above])
        end

        it 'finds above another' do
          driver.navigate.to url_for('relative_locators.html')

          above = driver.find_elements(relative: {css: 'td', above: {id: 'center'}})
          expect(above.map { |e| e.attribute('id') }).to eq(%w[top topLeft topRight])
        end

        it 'finds below element' do
          driver.navigate.to url_for('relative_locators.html')

          midpoint = driver.find_element(id: 'mid')
          above = driver.find_elements(relative: {id: 'below', below: midpoint})
          expect(above.map { |e| e.attribute('id') }).to eq(['below'])
        end

        it 'finds near another within default distance' do
          driver.navigate.to url_for('relative_locators.html')

          near = driver.find_elements(relative: {tag_name: 'td', near: {id: 'right'}})
          expect(near.map { |e| e.attribute('id') }).to eq(%w[topRight bottomRight center top bottom])
        end

        it 'finds near another within custom distance', except: {browser: %i[safari safari_preview]} do
          driver.navigate.to url_for('relative_locators.html')

          near = driver.find_elements(relative: {tag_name: 'td', near: {id: 'right', distance: 100}})
          expect(near.map { |e| e.attribute('id') }).to eq(%w[topRight bottomRight center top bottom])
        end

        it 'finds to the left of another' do
          driver.navigate.to url_for('relative_locators.html')

          left = driver.find_elements(relative: {tag_name: 'td', left: {id: 'center'}})
          expect(left.map { |e| e.attribute('id') }).to eq(%w[left topLeft bottomLeft])
        end

        it 'finds to the right of another' do
          driver.navigate.to url_for('relative_locators.html')

          right = driver.find_elements(relative: {tag_name: 'td', right: {id: 'center'}})
          expect(right.map { |e| e.attribute('id') }).to eq(%w[right topRight bottomRight])
        end

        it 'finds by combined relative locators' do
          driver.navigate.to url_for('relative_locators.html')

          found = driver.find_elements(relative: {tag_name: 'td', right: {id: 'top'}, above: {id: 'center'}})
          expect(found.map { |e| e.attribute('id') }).to eq(['topRight'])
        end

        it 'finds all by empty relative locator' do
          driver.navigate.to url_for('relative_locators.html')

          expected = driver.find_elements(tag_name: 'p')
          actual = driver.find_elements(relative: {tag_name: 'p'})
          expect(actual).to eq(expected)
        end

        it 'finds children by field name' do
          driver.navigate.to url_for('nestedElements.html')
          element = driver.find_element(name: 'form2')
          children = element.find_elements(name: 'selectomatic')
          expect(children.size).to eq(2)
        end
      end

      describe '#script' do
        it 'executes script with deprecation warning' do
          driver.navigate.to url_for('xhtmlTest.html')
          expect {
            expect(driver.script('return document.title;')).to eq('XHTML Test Page')
          }.to have_deprecated(:driver_script)
        end
      end

      describe '#execute_script' do
        it 'returns strings' do
          driver.navigate.to url_for('xhtmlTest.html')
          expect(driver.execute_script('return document.title;')).to eq('XHTML Test Page')
        end

        it 'returns numbers' do
          driver.navigate.to url_for('xhtmlTest.html')
          expect(driver.execute_script('return document.title.length;')).to eq(15)
        end

        it 'returns elements' do
          driver.navigate.to url_for('xhtmlTest.html')
          element = driver.execute_script("return document.getElementById('id1');")
          expect(element).to be_a(WebDriver::Element)
          expect(element.text).to eq('Foo')
        end

        it 'unwraps elements in deep objects' do
          driver.navigate.to url_for('xhtmlTest.html')
          result = driver.execute_script(<<~SCRIPT)
            var e1 = document.getElementById('id1');
            var body = document.body;

            return {
              elements: {'body' : body, other: [e1] }
            };
          SCRIPT

          expect(result).to be_a(Hash)
          expect(result['elements']['body']).to be_a(WebDriver::Element)
          expect(result['elements']['other'].first).to be_a(WebDriver::Element)
        end

        it 'returns booleans' do
          driver.navigate.to url_for('xhtmlTest.html')
          expect(driver.execute_script('return true;')).to be(true)
        end

        it 'raises if the script is bad' do
          driver.navigate.to url_for('xhtmlTest.html')
          expect {
            driver.execute_script('return squiggle();')
          }.to raise_error(Selenium::WebDriver::Error::JavascriptError)
        end

        it 'returns arrays' do
          driver.navigate.to url_for('xhtmlTest.html')
          expect(driver.execute_script('return ["zero", "one", "two"];')).to eq(%w[zero one two])
        end

        it 'is able to call functions on the page' do
          driver.navigate.to url_for('javascriptPage.html')
          driver.execute_script("displayMessage('I like cheese');")
          expect(driver.find_element(id: 'result').text.strip).to eq('I like cheese')
        end

        it 'is able to pass string arguments' do
          driver.navigate.to url_for('javascriptPage.html')
          expect(driver.execute_script("return arguments[0] == 'fish' ? 'fish' : 'not fish';", 'fish')).to eq('fish')
        end

        it 'is able to pass boolean arguments' do
          driver.navigate.to url_for('javascriptPage.html')
          expect(driver.execute_script('return arguments[0] == true;', true)).to be(true)
        end

        it 'is able to pass numeric arguments' do
          driver.navigate.to url_for('javascriptPage.html')
          expect(driver.execute_script('return arguments[0] == 1 ? 1 : 0;', 1)).to eq(1)
        end

        it 'is able to pass null arguments' do
          driver.navigate.to url_for('javascriptPage.html')
          expect(driver.execute_script('return arguments[0];', nil)).to be_nil
        end

        it 'is able to pass array arguments' do
          driver.navigate.to url_for('javascriptPage.html')
          expect(driver.execute_script('return arguments[0];', [1, '2', 3])).to eq([1, '2', 3])
        end

        it 'is able to pass element arguments' do
          driver.navigate.to url_for('javascriptPage.html')
          button = driver.find_element(id: 'plainButton')
          js = "arguments[0]['flibble'] = arguments[0].getAttribute('id'); return arguments[0]['flibble'];"
          expect(driver.execute_script(js, button))
            .to eq('plainButton')
        end

        it 'is able to pass in multiple arguments' do
          driver.navigate.to url_for('javascriptPage.html')
          expect(driver.execute_script('return arguments[0] + arguments[1];', 'one', 'two')).to eq('onetwo')
        end
      end

      describe 'execute async script' do
        before do
          driver.manage.timeouts.script = 1
          driver.navigate.to url_for('ajaxy_page.html')
        end

        it 'is able to return arrays of primitives from async scripts' do
          result = driver.execute_async_script "arguments[arguments.length - 1]([null, 123, 'abc', true, false]);"
          expect(result).to eq([nil, 123, 'abc', true, false])
        end

        it 'is able to pass multiple arguments to async scripts' do
          result = driver.execute_async_script 'arguments[arguments.length - 1](arguments[0] + arguments[1]);', 1, 2
          expect(result).to eq(3)
        end

        # Safari raises TimeoutError instead
        it 'times out if the callback is not invoked', except: {browser: %i[safari safari_preview]} do
          expect {
            # Script is expected to be async and explicitly callback, so this should timeout.
            driver.execute_async_script 'return 1 + 2;'
          }.to raise_error(Selenium::WebDriver::Error::ScriptTimeoutError)
        end
      end
    end
  end # WebDriver
end # Selenium
