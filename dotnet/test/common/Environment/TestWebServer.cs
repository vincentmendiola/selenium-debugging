// <copyright file="TestWebServer.cs" company="Selenium Committers">
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

using Bazel;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenQA.Selenium.Environment
{
    public class TestWebServer
    {
        private Process webserverProcess;

        private string standaloneTestJar = @"_main/java/test/org/openqa/selenium/environment/appserver";
        private string projectRootPath;
        private bool captureWebServerOutput;
        private bool hideCommandPrompt;
        private string javaHomeDirectory;
        private string port;

        private StringBuilder outputData = new StringBuilder();

        public TestWebServer(string projectRoot, TestWebServerConfig config)
        {
            this.projectRootPath = projectRoot;
            this.captureWebServerOutput = config.CaptureConsoleOutput;
            this.hideCommandPrompt = config.HideCommandPromptWindow;
            this.javaHomeDirectory = config.JavaHomeDirectory;
            this.port = config.Port;
        }

        public async Task StartAsync()
        {
            if (webserverProcess == null || webserverProcess.HasExited)
            {
                try
                {
                    var runfiles = Runfiles.Create();
                    standaloneTestJar = runfiles.Rlocation(standaloneTestJar);
                }
                catch (FileNotFoundException)
                {
                    var baseDirectory = AppContext.BaseDirectory;
                    standaloneTestJar = Path.Combine(baseDirectory, "../../../../../../bazel-bin/java/test/org/openqa/selenium/environment/appserver");
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    standaloneTestJar += ".exe";
                }

                Console.Write("Standalone jar is " + standaloneTestJar);

                if (!File.Exists(standaloneTestJar))
                {
                    throw new FileNotFoundException(
                        string.Format(
                            "Test webserver jar at {0} didn't exist. Project root is {2}. Please build it using something like {1}.",
                            standaloneTestJar,
                            "bazel build //java/test/org/openqa/selenium/environment:appserver_deploy.jar",
                            projectRootPath));
                }

                //List<string> javaSystemProperties = new List<string>();

                StringBuilder processArgsBuilder = new StringBuilder();
                // foreach (string systemProperty in javaSystemProperties)
                // {
                //     if (processArgsBuilder.Length > 0)
                //     {
                //         processArgsBuilder.Append(" ");
                //     }
                //
                //     processArgsBuilder.AppendFormat("-D{0}", systemProperty);
                // }
                //
                // if (processArgsBuilder.Length > 0)
                // {
                //     processArgsBuilder.Append(" ");
                // }
                //
                // processArgsBuilder.AppendFormat("-jar {0}", standaloneTestJar);
                processArgsBuilder.AppendFormat(" {0}", this.port);

                Console.Write(processArgsBuilder.ToString());

                webserverProcess = new Process();
                webserverProcess.StartInfo.FileName = standaloneTestJar;
                // if (!string.IsNullOrEmpty(javaExecutablePath))
                // {
                //     webserverProcess.StartInfo.FileName = Path.Combine(javaExecutablePath, javaExecutableName);
                // }
                // else
                // {
                //     webserverProcess.StartInfo.FileName = javaExecutableName;
                // }

                webserverProcess.StartInfo.Arguments = processArgsBuilder.ToString();
                webserverProcess.StartInfo.WorkingDirectory = projectRootPath;
                webserverProcess.StartInfo.UseShellExecute = !(hideCommandPrompt || captureWebServerOutput);
                webserverProcess.StartInfo.CreateNoWindow = hideCommandPrompt;
                if (!string.IsNullOrEmpty(this.javaHomeDirectory))
                {
                    webserverProcess.StartInfo.EnvironmentVariables["JAVA_HOME"] = this.javaHomeDirectory;
                }

                captureWebServerOutput = true;

                if (captureWebServerOutput)
                {
                    webserverProcess.StartInfo.RedirectStandardOutput = true;
                    webserverProcess.StartInfo.RedirectStandardError = true;
                }

                webserverProcess.Start();

                TimeSpan timeout = TimeSpan.FromSeconds(30);
                DateTime endTime = DateTime.Now.Add(TimeSpan.FromSeconds(30));
                bool isRunning = false;

                // Poll until the webserver is correctly serving pages.
                using var httpClient = new HttpClient();

                while (!isRunning && DateTime.Now < endTime)
                {
                    try
                    {
                        using var response = await httpClient.GetAsync(EnvironmentManager.Instance.UrlBuilder.LocalWhereIs("simpleTest.html"));

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
                    string output = "'CaptureWebServerOutput' parameter is false. Web server output not captured";
                    string error = "'CaptureWebServerOutput' parameter is false. Web server output not being captured.";
                    if (captureWebServerOutput)
                    {
                        error = webserverProcess.StandardError.ReadToEnd();
                        output = webserverProcess.StandardOutput.ReadToEnd();
                    }

                    string errorMessage = string.Format("Could not start the test web server in {0} seconds.\nWorking directory: {1}\nProcess Args: {2}\nstdout: {3}\nstderr: {4}", timeout.TotalSeconds, projectRootPath, processArgsBuilder, output, error);
                    throw new TimeoutException(errorMessage);
                }
            }
        }

        public async Task StopAsync()
        {
            if (webserverProcess != null)
            {
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        using (await httpClient.GetAsync(EnvironmentManager.Instance.UrlBuilder.LocalWhereIs("quitquitquit")))
                        {

                        }
                    }
                    catch (HttpRequestException)
                    {

                    }
                }

                try
                {
                    webserverProcess.WaitForExit(10000);
                    if (!webserverProcess.HasExited)
                    {
                        webserverProcess.Kill(entireProcessTree: true);
                    }
                }
                finally
                {
                    webserverProcess.Dispose();
                    webserverProcess = null;
                }
            }
        }
    }
}
