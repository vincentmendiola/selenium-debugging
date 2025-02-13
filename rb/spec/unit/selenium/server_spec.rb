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

require File.expand_path('webdriver/spec_helper', __dir__)
require 'selenium/server'

module Selenium
  describe Server do
    let(:mock_process) { instance_double(WebDriver::ChildProcess).as_null_object }
    let(:mock_poller) { instance_double(WebDriver::SocketPoller, connected?: true, closed?: true) }
    let(:repo) { 'https://api.github.com/repos/seleniumhq/selenium/releases' }
    let(:port) { WebDriver::PortProber.above(4444) }
    let(:example_json) do
      [{url: "#{repo}/41272273",
        assets: {
          name: 'selenium-server-3.141.59.jar',
          browser_download_url: "#{repo}/selenium-3.141.59/selenium-server-standalone-3.141.59.jar"
        }},
       {url: "#{repo}/51272273",
        assets: {
          name: 'selenium-server-10.0.1.jar',
          browser_download_url: "#{repo}/selenium-10.0.1/selenium-server-10.0.1.jar"
        }}].to_json
    end

    it 'raises an error if the jar file does not exist' do
      expect {
        described_class.new('doesnt-exist.jar')
      }.to raise_error(Errno::ENOENT)
    end

    it 'uses the given jar file and port' do
      allow(File).to receive(:exist?).with('selenium_server_deploy.jar').and_return(true)
      allow(WebDriver::ChildProcess).to receive(:build)
        .with('java', '-jar', 'selenium_server_deploy.jar', 'standalone', '--port', '1234')
        .and_return(mock_process)

      server = described_class.new('selenium_server_deploy.jar', port: 1234, background: true)
      allow(server).to receive(:socket).and_return(mock_poller)

      server.start
      expect(File).to have_received(:exist?).with('selenium_server_deploy.jar')
      expect(WebDriver::ChildProcess).to have_received(:build)
        .with('java', '-jar', 'selenium_server_deploy.jar', 'standalone', '--port', '1234')
    end

    it 'waits for the server process by default' do
      allow(File).to receive(:exist?).with('selenium_server_deploy.jar').and_return(true)
      allow(WebDriver::ChildProcess).to receive(:build)
        .with('java', '-jar', 'selenium_server_deploy.jar', 'standalone', '--port', port.to_s)
        .and_return(mock_process)

      server = described_class.new('selenium_server_deploy.jar', port: port)
      allow(server).to receive(:socket).and_return(mock_poller)
      allow(mock_process).to receive(:wait)

      server.start

      expect(File).to have_received(:exist?).with('selenium_server_deploy.jar')
      expect(WebDriver::ChildProcess).to have_received(:build)
        .with('java', '-jar', 'selenium_server_deploy.jar', 'standalone', '--port', port.to_s)
      expect(mock_process).to have_received(:wait)
    end

    it 'adds additional args' do
      allow(File).to receive(:exist?).with('selenium_server_deploy.jar').and_return(true)
      allow(WebDriver::ChildProcess).to receive(:build)
        .with('java', '-jar', 'selenium_server_deploy.jar', 'standalone', '--port', port.to_s, 'foo', 'bar')
        .and_return(mock_process)

      server = described_class.new('selenium_server_deploy.jar', port: port, background: true)
      allow(server).to receive(:socket).and_return(mock_poller)

      server << %w[foo bar]

      server.start
      expect(File).to have_received(:exist?).with('selenium_server_deploy.jar')
      expect(WebDriver::ChildProcess).to have_received(:build)
        .with('java', '-jar', 'selenium_server_deploy.jar', 'standalone',
              '--port', port.to_s, 'foo', 'bar')
    end

    it 'adds additional JAVA options args' do
      allow(File).to receive(:exist?).with('selenium_server_deploy.jar').and_return(true)
      allow(WebDriver::ChildProcess).to receive(:build)
        .with('java',
              '-Dwebdriver.chrome.driver=/bin/chromedriver',
              '-jar', 'selenium_server_deploy.jar',
              'standalone',
              '--port', port.to_s,
              'foo',
              'bar')
        .and_return(mock_process)

      server = described_class.new('selenium_server_deploy.jar', background: true)
      allow(server).to receive(:socket).and_return(mock_poller)

      server << %w[foo bar]
      server << '-Dwebdriver.chrome.driver=/bin/chromedriver'

      server.start
      expect(File).to have_received(:exist?).with('selenium_server_deploy.jar')
      expect(WebDriver::ChildProcess).to have_received(:build)
        .with('java',
              '-Dwebdriver.chrome.driver=/bin/chromedriver',
              '-jar', 'selenium_server_deploy.jar',
              'standalone',
              '--port', port.to_s,
              'foo',
              'bar')
    end

    it 'downloads the specified version from the selenium site' do
      required_version = '10.0.1'
      expected_download_file_name = "selenium-server-#{required_version}.jar"

      stub_request(:get, repo).to_return(body: example_json)

      stub_request(:get, "#{repo}/selenium-10.0.1/#{expected_download_file_name}")
        .to_return(headers: {location: 'https://github-releases.githubusercontent.com/something'})

      stub_request(:get, 'https://github-releases.githubusercontent.com/something')
        .to_return(body: 'this is pretending to be a jar file for testing purposes')

      begin
        actual_download_file_name = described_class.download(required_version)
        expect(actual_download_file_name).to eq(expected_download_file_name)
        expect(File).to exist(expected_download_file_name)
      ensure
        FileUtils.rm_rf expected_download_file_name
      end
    end

    it 'gets a server instance and downloads the specified version' do
      required_version = '10.4.0'
      expected_download_file_name = "selenium-server-standalone-#{required_version}.jar"
      expected_options = {port: 5555}
      fake_server = Object.new

      allow(described_class).to receive(:download).with(required_version).and_return(expected_download_file_name)
      allow(described_class).to receive(:new).with(expected_download_file_name,
                                                   expected_options).and_return(fake_server)

      server = described_class.get required_version, expected_options
      expect(server).to eq(fake_server)
      expect(described_class).to have_received(:download).with(required_version)
      expect(described_class).to have_received(:new).with(expected_download_file_name, expected_options)
    end

    it 'automatically repairs http_proxy settings that do not start with http://' do
      with_env('http_proxy' => 'proxy.com') do
        expect(described_class.net_http_start('example.com', &:proxy_address)).to eq('proxy.com')
      end

      with_env('HTTP_PROXY' => 'proxy.com') do
        expect(described_class.net_http_start('example.com', &:proxy_address)).to eq('proxy.com')
      end
    end

    it 'only downloads a jar if it is not present in the current directory' do
      required_version = '10.2.0'
      expected_download_file_name = "selenium-server-#{required_version}.jar"

      allow(File).to receive(:exist?).with(expected_download_file_name).and_return true

      described_class.download required_version
      expect(File).to have_received(:exist?).with(expected_download_file_name)
    end

    it 'knows what the latest version available is' do
      expected_latest = '10.0.1'

      stub_request(:get, repo).to_return(body: example_json)

      expect(described_class.latest).to eq(expected_latest)
    end

    it 'raises Selenium::Server::Error if the server is not launched within the timeout' do
      allow(File).to receive(:exist?).with('selenium_server_deploy.jar').and_return(true)
      allow(WebDriver::ChildProcess).to receive(:build)
        .with('java', '-jar', 'selenium_server_deploy.jar', 'standalone', '--port', port.to_s)
        .and_return(mock_process)

      poller = instance_double(WebDriver::SocketPoller)
      allow(poller).to receive(:connected?).and_return(false)

      server = described_class.new('selenium_server_deploy.jar', background: true)
      allow(server).to receive(:socket).and_return(poller)

      expect { server.start }.to raise_error(Selenium::Server::Error)
      expect(File).to have_received(:exist?).with('selenium_server_deploy.jar')
    end

    it 'sets options after instantiation' do
      allow(File).to receive(:exist?).with('selenium_server_deploy.jar').and_return(true)
      server = described_class.new('selenium_server_deploy.jar', port: port)
      expect(server.port).to eq(port)
      expect(server.timeout).to eq(30)
      expect(server.background).to be false
      expect(server.log).to be_nil

      server.port = 1234
      server.timeout = 5
      server.background = true
      server.log = '/tmp/server.log'

      aggregate_failures do
        expect(server.port).to eq(1234)
        expect(server.timeout).to eq(5)
        expect(server.background).to be_truthy
        expect(server.log).to eq('/tmp/server.log')
      end
      expect(File).to have_received(:exist?).with('selenium_server_deploy.jar')
    end
  end
end # Selenium
