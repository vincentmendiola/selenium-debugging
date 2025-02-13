// <copyright file="RemoteSeleniumServer.cs" company="Selenium Committers">
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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenQA.Selenium.Environment
{
    public class RemoteSeleniumServer
    {
        private Process webserverProcess;
        private string serverJarName = @"java/src/org/openqa/selenium/grid/selenium_server_deploy.jar";
        private string projectRootPath;
        private bool autoStart;

        public RemoteSeleniumServer(string projectRoot, bool autoStartServer)
        {
            projectRootPath = projectRoot;
            autoStart = autoStartServer;
        }

        public async Task StartAsync()
        {
            if (autoStart && (webserverProcess == null || webserverProcess.HasExited))
            {
                serverJarName = serverJarName.Replace('/', Path.DirectorySeparatorChar);
                if (!File.Exists(Path.Combine(projectRootPath, serverJarName)))
                {
                    throw new FileNotFoundException(
                        string.Format(
                            "Selenium server jar at {0} didn't exist - please build it using something like {1}",
                            serverJarName,
                            "go //java/src/org/openqa/grid/selenium:selenium"));
                }

                webserverProcess = new Process();
                webserverProcess.StartInfo.FileName = "java.exe";
                webserverProcess.StartInfo.Arguments = " -jar " + serverJarName + " standalone --port 6000 --selenium-manager true --enable-managed-downloads true";
                webserverProcess.StartInfo.WorkingDirectory = projectRootPath;
                webserverProcess.Start();
                DateTime timeout = DateTime.Now.Add(TimeSpan.FromSeconds(30));
                bool isRunning = false;

                // Poll until the webserver is correctly serving pages.
                using var httpClient = new HttpClient();

                while (!isRunning && DateTime.Now < timeout)
                {
                    try
                    {
                        using var response = await httpClient.GetAsync("http://localhost:6000/wd/hub/status");

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            isRunning = true;
                        }
                    }
                    catch (Exception ex) when (ex is HttpRequestException || ex is TimeoutException)
                    {
                    }
                }

                if (!isRunning)
                {
                    throw new TimeoutException("Could not start the remote selenium server in 30 seconds");
                }
            }
        }

        public async Task StopAsync()
        {
            if (autoStart && webserverProcess != null && !webserverProcess.HasExited)
            {
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        using var response = await httpClient.GetAsync("http://localhost:6000/selenium-server/driver?cmd=shutDownSeleniumServer");
                    }
                    catch (Exception ex) when (ex is HttpRequestException || ex is TimeoutException)
                    {
                    }
                }

                webserverProcess.WaitForExit(10000);
                if (!webserverProcess.HasExited)
                {
                    webserverProcess.Kill();
                }

                webserverProcess.Dispose();
                webserverProcess = null;
            }
        }
    }
}
