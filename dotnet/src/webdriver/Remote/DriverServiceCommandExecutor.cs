// <copyright file="DriverServiceCommandExecutor.cs" company="Selenium Committers">
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
using System.Threading.Tasks;

#nullable enable

namespace OpenQA.Selenium.Remote
{
    /// <summary>
    /// Provides a mechanism to execute commands on the browser
    /// </summary>
    public class DriverServiceCommandExecutor : ICommandExecutor
    {
        private readonly DriverService service;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverServiceCommandExecutor"/> class.
        /// </summary>
        /// <param name="driverService">The <see cref="DriverService"/> that drives the browser.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="driverService"/> is <see langword="null"/>.</exception>
        public DriverServiceCommandExecutor(DriverService driverService, TimeSpan commandTimeout)
            : this(driverService, commandTimeout, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverServiceCommandExecutor"/> class.
        /// </summary>
        /// <param name="driverService">The <see cref="DriverService"/> that drives the browser.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        /// <param name="enableKeepAlive"><see langword="true"/> if the KeepAlive header should be sent
        /// with HTTP requests; otherwise, <see langword="false"/>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="driverService"/> is <see langword="null"/>.</exception>
        public DriverServiceCommandExecutor(DriverService driverService, TimeSpan commandTimeout, bool enableKeepAlive)
        {
            this.service = driverService ?? throw new ArgumentNullException(nameof(driverService));
            this.HttpExecutor = new HttpCommandExecutor(driverService.ServiceUrl, commandTimeout, enableKeepAlive);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverServiceCommandExecutor"/> class.
        /// </summary>
        /// <param name="service">The <see cref="DriverService"/> that drives the browser.</param>
        /// <param name="commandExecutor">The <see cref="HttpCommandExecutor"/> object used to execute commands,
        /// communicating with the service via HTTP.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="service"/> or <paramref name="commandExecutor"/> are <see langword="null"/>.</exception>
        public DriverServiceCommandExecutor(DriverService service, HttpCommandExecutor commandExecutor)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.HttpExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
        }

        /// <summary>
        /// Gets the <see cref="CommandInfoRepository"/> object associated with this executor.
        /// </summary>
        //public CommandInfoRepository CommandInfoRepository
        //{
        //    get { return this.HttpExecutor.CommandInfoRepository; }
        //}

        public bool TryAddCommand(string commandName, CommandInfo info)
        {
            return this.HttpExecutor.TryAddCommand(commandName, info);
        }

        /// <summary>
        /// Gets the <see cref="HttpCommandExecutor"/> that sends commands to the remote
        /// end WebDriver implementation.
        /// </summary>
        public HttpCommandExecutor HttpExecutor { get; }

        /// <summary>
        /// Executes a command
        /// </summary>
        /// <param name="commandToExecute">The command you wish to execute</param>
        /// <returns>A response from the browser</returns>
        public Response Execute(Command commandToExecute)
        {
            return Task.Run(() => this.ExecuteAsync(commandToExecute)).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Executes a command as an asynchronous task.
        /// </summary>
        /// <param name="commandToExecute">The command you wish to execute</param>
        /// <returns>A task object representing the asynchronous operation</returns>
        public async Task<Response> ExecuteAsync(Command commandToExecute)
        {
            if (commandToExecute == null)
            {
                throw new ArgumentNullException(nameof(commandToExecute), "Command to execute cannot be null");
            }

            Response toReturn;
            if (commandToExecute.Name == DriverCommand.NewSession)
            {
                this.service.Start();
            }

            // Use a try-catch block to catch exceptions for the Quit
            // command, so that we can get the finally block.
            try
            {
                toReturn = await this.HttpExecutor.ExecuteAsync(commandToExecute).ConfigureAwait(false);
            }
            finally
            {
                if (commandToExecute.Name == DriverCommand.Quit)
                {
                    this.Dispose();
                }
            }

            return toReturn;
        }

        /// <summary>
        /// Releases all resources used by the <see cref="DriverServiceCommandExecutor"/>.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="HttpCommandExecutor"/> and
        /// optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release managed and resources;
        /// <see langword="false"/> to only release unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.HttpExecutor.Dispose();
                    this.service.Dispose();
                }

                this.isDisposed = true;
            }
        }
    }
}
