// <copyright file="IHasVirtualAuthenticator.cs" company="Selenium Committers">
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
    /// Interface indicating that an object supports using a virtual authenticator.
    /// </summary>
    public interface IHasVirtualAuthenticator
    {
        /// <summary>
        /// Adds a virtual authenticator.
        /// </summary>
        /// <param name="options">The VirtualAuthenticatorOptions to use in creating the authenticator.</param>
        /// <returns>The ID of the added virtual authenticator.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="options"/> is <see langword="null"/>.</exception>
        string AddVirtualAuthenticator(VirtualAuthenticatorOptions options);

        /// <summary>
        /// Removes a virtual authenticator.
        /// </summary>
        /// <param name="id">The ID of the virtual authenticator to remove.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="id"/> is <see langword="null"/>.</exception>
        /// <exception cref="WebDriverArgumentException">If the specified virtual authenticator does not exist.</exception>
        void RemoveVirtualAuthenticator(string id);

        /// <summary>
        /// Adds a credential to the virtual authenticator.
        /// </summary>
        /// <param name="credential">The credential to add to the authenticator.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="credential"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If a Virtual Authenticator has not been added yet.</exception>
        void AddCredential(Credential credential);

        /// <summary>
        /// Gets a list of the credentials registered to the virtual authenticator.
        /// </summary>
        /// <returns>The list of credentials registered to the virtual authenticator.</returns>
        List<Credential> GetCredentials();

        /// <summary>
        /// Removes a credential from the virtual authenticator.
        /// </summary>
        /// <param name="credentialId">A byte array representing the ID of the credential to remove.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="credentialId"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If a Virtual Authenticator has not been added yet.</exception>
        void RemoveCredential(byte[] credentialId);

        /// <summary>
        /// Removes a credential from the virtual authenticator.
        /// </summary>
        /// <param name="credentialId">A string representing the ID of the credential to remove.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="credentialId"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If a Virtual Authenticator has not been added yet.</exception>
        void RemoveCredential(string credentialId);

        /// <summary>
        /// Removes all credentials registered to this virtual authenticator.
        /// </summary>
        /// <exception cref="InvalidOperationException">If a Virtual Authenticator has not been added yet.</exception>
        void RemoveAllCredentials();

        /// <summary>
        /// Sets whether or not a user is verified in this virtual authenticator.
        /// </summary>
        /// <param name="verified"><see langword="true"/> if the user is verified; otherwise <see langword="false"/>.</param>
        /// <exception cref="InvalidOperationException">If a Virtual Authenticator has not been added yet.</exception>
        void SetUserVerified(bool verified);
    }
}
