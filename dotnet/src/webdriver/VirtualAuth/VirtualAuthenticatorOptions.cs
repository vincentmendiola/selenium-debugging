// <copyright file="VirtualAuthenticatorOptions.cs" company="Selenium Committers">
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
// </copyright>

using System;
using System.Collections.Generic;

#nullable enable

namespace OpenQA.Selenium.VirtualAuth
{
    /// <summary>
    /// Options for the creation of virtual authenticators.
    /// Refer https://w3c.github.io/webauthn/#sctn-automation
    /// </summary>
    public class VirtualAuthenticatorOptions
    {
        /// <summary>
        /// The protocol to use for the virtual authenticator.
        /// </summary>
        public static class Protocol
        {
            /// <summary>
            /// Value representing the CTAP2 protocol.
            /// </summary>
            public static readonly string CTAP2 = "ctap2";

            /// <summary>
            /// Value representing the U2F protocol.
            /// </summary>
            public static readonly string U2F = "ctap1/u2f";
        }

        /// <summary>
        /// The transport to use for the virtual authenticator.
        /// </summary>
        public static class Transport
        {
            /// <summary>
            /// Value representing the BLE transport.
            /// </summary>
            public static readonly string BLE = "ble";

            /// <summary>
            /// Value representing the "internal" transport.
            /// </summary>
            public static readonly string INTERNAL = "internal";

            /// <summary>
            /// Value representing the near-field communications transport.
            /// </summary>
            public static readonly string NFC = "nfc";

            /// <summary>
            /// Value representing the USB transport.
            /// </summary>
            public static readonly string USB = "usb";
        }

        private string protocol = Protocol.CTAP2;
        private string transport = Transport.USB;
        private bool hasResidentKey = false;
        private bool hasUserVerification = false;
        private bool isUserConsenting = true;
        private bool isUserVerified = false;

        /// <summary>
        /// Sets the Client to Authenticator Protocol (CTAP) this <see href="https://www.w3.org/TR/webauthn-2/#sctn-automation-virtual-authenticators">Virtual Authenticator</see> speaks.
        /// </summary>
        /// <param name="protocol">The CTAP protocol identifier.</param>
        /// <returns>This options instance for chaining.</returns>
        /// <remarks>Valid protocols are available on the <see cref="Protocol"/> type.</remarks>
        /// <exception cref="ArgumentException">If <paramref name="protocol"/> is not a supported protocol value.</exception>
        /// <completionlist cref="Protocol"/>
        public VirtualAuthenticatorOptions SetProtocol(string protocol)
        {
            if (string.Equals(Protocol.CTAP2, protocol) || string.Equals(Protocol.U2F, protocol))
            {
                this.protocol = protocol;
                return this;
            }
            else
            {
                throw new ArgumentException("Enter a valid protocol value." +
                    "Refer to https://www.w3.org/TR/webauthn-2/#sctn-automation-virtual-authenticators for supported protocols.");
            }
        }

        /// <summary>
        /// Sets the <see href="https://www.w3.org/TR/webauthn-2/#enum-transport">Authenticator Transport</see> this <see href="https://www.w3.org/TR/webauthn-2/#sctn-automation-virtual-authenticators">Virtual Authenticator</see> needs to implement, to communicate with clients.
        /// </summary>
        /// <param name="transport">Valid transport value.
        /// </param>
        /// <returns>This options instance for chaining.</returns>
        /// <remarks>Valid protocols are available on the <see cref="Transport"/> type.</remarks>
        /// <exception cref="ArgumentException">If <paramref name="transport"/> is not a supported transport value.</exception>
        /// <completionlist cref="Transport"/>
        public VirtualAuthenticatorOptions SetTransport(string transport)
        {
            if (Transport.BLE == transport || Transport.INTERNAL == transport || Transport.NFC == transport || Transport.USB == transport)
            {
                this.transport = transport;
                return this;
            }
            else
            {
                throw new ArgumentException("Enter a valid transport value." +
                    "Refer to https://www.w3.org/TR/webauthn-2/#enum-transport for supported transport values.");
            }
        }

        /// <summary>
        /// If set to <see langword="true"/>, the authenticator will support <see href="https://w3c.github.io/webauthn/#client-side-discoverable-credential">Client-side discoverable Credentials</see>.
        /// </summary>
        /// <param name="hasResidentKey">Whether authenticator will support client-side discoverable credentials.</param>
        /// <returns>This options instance for chaining.</returns>
        public VirtualAuthenticatorOptions SetHasResidentKey(bool hasResidentKey)
        {
            this.hasResidentKey = hasResidentKey;
            return this;
        }

        /// <summary>
        /// If set to <see langword="true"/>, the authenticator will support <see href="https://w3c.github.io/webauthn/#user-verification">User Verification</see>.
        /// </summary>
        /// <param name="hasUserVerification">Whether the authenticator supports user verification.</param>
        /// <returns>This options instance for chaining.</returns>
        public VirtualAuthenticatorOptions SetHasUserVerification(bool hasUserVerification)
        {
            this.hasUserVerification = hasUserVerification;
            return this;
        }

        /// <summary>
        /// If set to <see langword="true"/>, a <see href="https://w3c.github.io/webauthn/#user-consent">User Consent</see> will always be granted.
        /// </summary>
        /// <param name="isUserConsenting">Whether a user consent will always be granted.</param>
        /// <returns>This options instance for chaining.</returns>
        public VirtualAuthenticatorOptions SetIsUserConsenting(bool isUserConsenting)
        {
            this.isUserConsenting = isUserConsenting;
            return this;
        }

        /// <summary>
        /// If set to <see langword="true"/>, <see href="https://w3c.github.io/webauthn/#user-verification">User Verification</see> will always succeed.
        /// </summary>
        /// <param name="isUserVerified">Whether User Verification will always succeed.</param>
        /// <returns>This options instance for chaining.</returns>
        public VirtualAuthenticatorOptions SetIsUserVerified(bool isUserVerified)
        {
            this.isUserVerified = isUserVerified;
            return this;
        }

        /// <summary>
        /// Serializes this set of options into a dictionary of key-value pairs.
        /// </summary>
        /// <returns>The dictionary containing the values of this set of options.</returns>
        public Dictionary<string, object> ToDictionary()
        {
            Dictionary<string, object> toReturn = new Dictionary<string, object>();

            toReturn["protocol"] = this.protocol;
            toReturn["transport"] = this.transport;
            toReturn["hasResidentKey"] = this.hasResidentKey;
            toReturn["hasUserVerification"] = this.hasUserVerification;
            toReturn["isUserConsenting"] = this.isUserConsenting;
            toReturn["isUserVerified"] = this.isUserVerified;

            return toReturn;
        }
    }
}
