// <copyright file="DevToolsProfilerTest.cs" company="Selenium Committers">
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

using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenQA.Selenium.DevTools
{
    using CurrentCdpVersion = V132;

    [TestFixture]
    public class DevToolsProfilerTest : DevToolsTestFixture
    {
        [Test]
        [IgnoreBrowser(Selenium.Browser.IE, "IE does not support Chrome DevTools Protocol")]
        [IgnoreBrowser(Selenium.Browser.Firefox, "Firefox does not support Chrome DevTools Protocol")]
        [IgnoreBrowser(Selenium.Browser.Safari, "Safari does not support Chrome DevTools Protocol")]
        public async Task SimpleStartStopAndGetProfilerTest()
        {
            var domains = session.GetVersionSpecificDomains<CurrentCdpVersion.DevToolsSessionDomains>();
            await domains.Profiler.Enable();
            await domains.Profiler.Start();
            var response = await domains.Profiler.Stop();
            var profiler = response.Profile;
            ValidateProfile(profiler);
            await domains.Profiler.Disable();
        }

        [Test]
        [IgnoreBrowser(Selenium.Browser.IE, "IE does not support Chrome DevTools Protocol")]
        [IgnoreBrowser(Selenium.Browser.Firefox, "Firefox does not support Chrome DevTools Protocol")]
        [IgnoreBrowser(Selenium.Browser.Safari, "Safari does not support Chrome DevTools Protocol")]
        public async Task SampleGetBestEffortProfilerTest()
        {
            var domains = session.GetVersionSpecificDomains<CurrentCdpVersion.DevToolsSessionDomains>();
            await domains.Profiler.Enable();
            driver.Url = simpleTestPage;
            await domains.Profiler.SetSamplingInterval(new CurrentCdpVersion.Profiler.SetSamplingIntervalCommandSettings()
            {
                Interval = 30
            });

            var response = await domains.Profiler.GetBestEffortCoverage();
            var bestEffort = response.Result;
            Assert.That(bestEffort, Is.Not.Null);
            Assert.That(bestEffort.Length, Is.GreaterThan(0));
            await domains.Profiler.Disable();
        }

        [Test]
        [IgnoreBrowser(Selenium.Browser.IE, "IE does not support Chrome DevTools Protocol")]
        [IgnoreBrowser(Selenium.Browser.Firefox, "Firefox does not support Chrome DevTools Protocol")]
        [IgnoreBrowser(Selenium.Browser.Safari, "Safari does not support Chrome DevTools Protocol")]
        public async Task SampleSetStartPreciseCoverageTest()
        {
            var domains = session.GetVersionSpecificDomains<CurrentCdpVersion.DevToolsSessionDomains>();
            await domains.Profiler.Enable();
            driver.Url = simpleTestPage;
            await domains.Profiler.StartPreciseCoverage(new CurrentCdpVersion.Profiler.StartPreciseCoverageCommandSettings()
            {
                CallCount = true,
                Detailed = true
            });
            await domains.Profiler.Start();
            var coverageResponse = await domains.Profiler.TakePreciseCoverage();
            var pc = coverageResponse.Result;
            Assert.That(pc, Is.Not.Null);
            var response = await domains.Profiler.Stop();
            var profiler = response.Profile;
            ValidateProfile(profiler);
            await domains.Profiler.Disable();
        }


        [Test]
        [IgnoreBrowser(Selenium.Browser.IE, "IE does not support Chrome DevTools Protocol")]
        [IgnoreBrowser(Selenium.Browser.Firefox, "Firefox does not support Chrome DevTools Protocol")]
        [IgnoreBrowser(Selenium.Browser.Safari, "Safari does not support Chrome DevTools Protocol")]
        public async Task SampleProfileEvents()
        {
            var domains = session.GetVersionSpecificDomains<CurrentCdpVersion.DevToolsSessionDomains>();
            await domains.Profiler.Enable();
            driver.Url = simpleTestPage;
            ManualResetEventSlim startSync = new ManualResetEventSlim(false);
            EventHandler<CurrentCdpVersion.Profiler.ConsoleProfileStartedEventArgs> consoleProfileStartedHandler = (sender, e) =>
            {
                Assert.That(e, Is.Not.Null);
                startSync.Set();
            };
            domains.Profiler.ConsoleProfileStarted += consoleProfileStartedHandler;

            await domains.Profiler.Start();
            startSync.Wait(TimeSpan.FromSeconds(5));
            driver.Navigate().Refresh();

            ManualResetEventSlim finishSync = new ManualResetEventSlim(false);
            EventHandler<CurrentCdpVersion.Profiler.ConsoleProfileFinishedEventArgs> consoleProfileFinishedHandler = (sender, e) =>
            {
                Assert.That(e, Is.Not.Null);
                finishSync.Set();
            };
            domains.Profiler.ConsoleProfileFinished += consoleProfileFinishedHandler;

            var response = await domains.Profiler.Stop();
            finishSync.Wait(TimeSpan.FromSeconds(5));

            var profiler = response.Profile;
            ValidateProfile(profiler);
            await domains.Profiler.Disable();
        }

        private void ValidateProfile(CurrentCdpVersion.Profiler.Profile profiler)
        {
            Assert.That(profiler, Is.Not.Null);
            Assert.That(profiler.Nodes, Is.Not.Null);
            Assert.That(profiler.StartTime, Is.Not.Zero);
            Assert.That(profiler.EndTime, Is.Not.Zero);
            Assert.That(profiler.TimeDeltas, Is.Not.Null);
            foreach (var delta in profiler.TimeDeltas)
            {
                Assert.That(delta, Is.Not.Zero);
            }

            foreach (var node in profiler.Nodes)
            {
                Assert.That(node, Is.Not.Null);
                Assert.That(node.CallFrame, Is.Not.Null);
            }
        }
    }
}
