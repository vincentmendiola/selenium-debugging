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

require File.expand_path('../spec_helper', __dir__)

module Selenium
  module WebDriver
    module IE
      describe Options do
        subject(:options) { described_class.new }

        describe '#initialize' do
          it 'accepts all defined parameters' do
            allow(File).to receive(:directory?).and_return(true)

            opts = described_class.new(browser_version: '11',
                                       platform_name: 'win10',
                                       accept_insecure_certs: false,
                                       page_load_strategy: 'eager',
                                       unhandled_prompt_behavior: 'accept',
                                       strict_file_interactability: true,
                                       timeouts: {script: 40000,
                                                  page_load: 400000,
                                                  implicit: 1},
                                       set_window_rect: false,
                                       args: %w[foo bar],
                                       browser_attach_timeout: 30000,
                                       element_scroll_behavior: Options::SCROLL_BOTTOM,
                                       full_page_screenshot: true,
                                       ensure_clean_session: true,
                                       file_upload_dialog_timeout: 30000,
                                       force_create_process_api: true,
                                       force_shell_windows_api: true,
                                       ignore_protected_mode_settings: true,
                                       ignore_zoom_level: true,
                                       initial_browser_url: 'http://google.com',
                                       native_events: false,
                                       persistent_hover: true,
                                       require_window_focus: true,
                                       use_per_process_proxy: true,
                                       use_legacy_file_upload_dialog_handling: true,
                                       attach_to_edge_chrome: true,
                                       edge_executable_path: '/path/to/edge',
                                       silent: true,
                                       'custom:options': {foo: 'bar'})

            expect(opts.args.to_a).to eq(%w[foo bar])
            expect(opts.browser_attach_timeout).to eq(30000)
            expect(opts.element_scroll_behavior).to eq(1)
            expect(opts.full_page_screenshot).to be(true)
            expect(opts.ensure_clean_session).to be(true)
            expect(opts.file_upload_dialog_timeout).to eq(30000)
            expect(opts.force_create_process_api).to be(true)
            expect(opts.force_shell_windows_api).to be(true)
            expect(opts.ignore_protected_mode_settings).to be(true)
            expect(opts.ignore_zoom_level).to be(true)
            expect(opts.initial_browser_url).to eq('http://google.com')
            expect(opts.native_events).to be(false)
            expect(opts.persistent_hover).to be(true)
            expect(opts.require_window_focus).to be(true)
            expect(opts.use_per_process_proxy).to be(true)
            expect(opts.use_legacy_file_upload_dialog_handling).to be(true)
            expect(opts.attach_to_edge_chrome).to be(true)
            expect(opts.edge_executable_path).to eq('/path/to/edge')
            expect(opts.browser_name).to eq('internet explorer')
            expect(opts.browser_version).to eq('11')
            expect(opts.platform_name).to eq('win10')
            expect(opts.accept_insecure_certs).to be(false)
            expect(opts.page_load_strategy).to eq('eager')
            expect(opts.unhandled_prompt_behavior).to eq('accept')
            expect(opts.strict_file_interactability).to be(true)
            expect(opts.timeouts).to eq(script: 40000, page_load: 400000, implicit: 1)
            expect(opts.set_window_rect).to be(false)
            expect(opts.options[:'custom:options']).to eq(foo: 'bar')
            expect(opts.silent).to be_truthy
          end

          it 'has native events on by default' do
            expect(options.native_events).to be(true)
          end
        end

        describe '#add_argument' do
          it 'adds a command-line argument' do
            options.add_argument('foo')
            expect(options.args.to_a).to eq(['foo'])
          end
        end

        describe '#add_option' do
          it 'adds vendor namespaced options with ordered pairs' do
            options.add_option('foo:bar', {bar: 'foo'})
            expect(options.instance_variable_get(:@options)['foo:bar']).to eq({bar: 'foo'})
          end

          it 'adds vendor namespaced options with Hash' do
            options.add_option('foo:bar' => {bar: 'foo'})
            expect(options.instance_variable_get(:@options)['foo:bar']).to eq({bar: 'foo'})
          end
        end

        describe '#as_json' do
          it 'returns empty options by default' do
            expect(options.as_json).to eq('browserName' => 'internet explorer',
                                          'se:ieOptions' => {'nativeEvents' => true})
          end

          it 'returns added options' do
            options.add_option('foo:bar', {foo: 'bar'})
            expect(options.as_json).to eq('browserName' => 'internet explorer',
                                          'foo:bar' => {'foo' => 'bar'},
                                          'se:ieOptions' => {'nativeEvents' => true})
          end

          it 'returns a JSON hash' do
            opts = described_class.new(browser_version: '11',
                                       platform_name: 'win10',
                                       accept_insecure_certs: false,
                                       page_load_strategy: 'eager',
                                       silent: true,
                                       unhandled_prompt_behavior: 'accept',
                                       strict_file_interactability: true,
                                       timeouts: {script: 40000,
                                                  page_load: 400000,
                                                  implicit: 1},
                                       set_window_rect: false,
                                       args: %w[foo bar],
                                       browser_attach_timeout: 30000,
                                       element_scroll_behavior: Options::SCROLL_BOTTOM,
                                       full_page_screenshot: true,
                                       ensure_clean_session: true,
                                       file_upload_dialog_timeout: 30000,
                                       force_create_process_api: true,
                                       force_shell_windows_api: true,
                                       ignore_protected_mode_settings: true,
                                       ignore_zoom_level: true,
                                       initial_browser_url: 'http://google.com',
                                       native_events: false,
                                       persistent_hover: true,
                                       require_window_focus: true,
                                       use_per_process_proxy: true,
                                       use_legacy_file_upload_dialog_handling: true,
                                       attach_to_edge_chrome: true,
                                       edge_executable_path: '/path/to/edge',
                                       'custom:options': {foo: 'bar'})

            key = 'se:ieOptions'
            expect(opts.as_json).to eq('browserName' => 'internet explorer',
                                       'browserVersion' => '11',
                                       'platformName' => 'win10',
                                       'acceptInsecureCerts' => false,
                                       'pageLoadStrategy' => 'eager',
                                       'unhandledPromptBehavior' => 'accept',
                                       'strictFileInteractability' => true,
                                       'timeouts' => {'script' => 40000,
                                                      'pageLoad' => 400000,
                                                      'implicit' => 1},
                                       'setWindowRect' => false,
                                       'custom:options' => {'foo' => 'bar'},
                                       key => {'ie.browserCommandLineSwitches' => 'foo bar',
                                               'browserAttachTimeout' => 30000,
                                               'elementScrollBehavior' => 1,
                                               'ie.enableFullPageScreenshot' => true,
                                               'ie.ensureCleanSession' => true,
                                               'ie.fileUploadDialogTimeout' => 30000,
                                               'ie.forceCreateProcessApi' => true,
                                               'ie.forceShellWindowsApi' => true,
                                               'ignoreProtectedModeSettings' => true,
                                               'ignoreZoomSetting' => true,
                                               'initialBrowserUrl' => 'http://google.com',
                                               'nativeEvents' => false,
                                               'enablePersistentHover' => true,
                                               'requireWindowFocus' => true,
                                               'ie.usePerProcessProxy' => true,
                                               'ie.useLegacyFileUploadDialogHandling' => true,
                                               'ie.edgechromium' => true,
                                               'ie.edgepath' => '/path/to/edge',
                                               'silent' => true})
          end
        end
      end # Options
    end # IE
  end # WebDriver
end # Selenium
