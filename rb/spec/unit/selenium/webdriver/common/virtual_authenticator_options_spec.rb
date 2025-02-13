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
    describe VirtualAuthenticatorOptions do
      let(:options) do
        described_class.new(transport: :nfc,
                            protocol: :u2f,
                            resident_key: true,
                            user_verification: true,
                            user_consenting: false,
                            user_verified: true)
      end

      describe '#initialize' do
        it 'sets parameters' do
          expect(options.transport).to eq(:nfc)
          expect(options.protocol).to eq(:u2f)
          expect(options.resident_key?).to be(true)
          expect(options.user_verification?).to be(true)
          expect(options.user_consenting?).to be(false)
          expect(options.user_verified?).to be(true)
        end
      end

      describe '#as_json' do
        it 'converts default options to JSON' do
          json = options.as_json
          expect(json['transport']).to eq('nfc')
          expect(json['protocol']).to eq('ctap1/u2f')
          expect(json['hasResidentKey']).to be(true)
          expect(json['hasUserVerification']).to be(true)
          expect(json['isUserConsenting']).to be(false)
          expect(json['isUserVerified']).to be(true)
        end
      end
    end # VirtualAuthenticatorOptions
  end # WebDriver
end # Selenium
