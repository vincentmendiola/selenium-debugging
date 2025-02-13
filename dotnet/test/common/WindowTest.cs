// <copyright file="WindowTest.cs" company="Selenium Committers">
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
using System.Drawing;

namespace OpenQA.Selenium
{
    [TestFixture]
    public class WindowTest : DriverTestFixture
    {
        private Size originalWindowSize;

        [SetUp]
        public void GetBrowserWindowSize()
        {
            driver.Manage().Window.Position = new Point(50, 50);
            this.originalWindowSize = driver.Manage().Window.Size;
        }

        [TearDown]
        public void RestoreBrowserWindow()
        {
            driver.Manage().Window.Size = originalWindowSize;
        }

        [Test]
        public void ShouldBeAbleToGetTheSizeOfTheCurrentWindow()
        {
            Size size = driver.Manage().Window.Size;
            Assert.That(size.Width, Is.GreaterThan(0));
            Assert.That(size.Height, Is.GreaterThan(0));
        }

        [Test]
        public void ShouldBeAbleToSetTheSizeOfTheCurrentWindow()
        {
            IWindow window = driver.Manage().Window;
            Size size = window.Size;

            // resize relative to the initial size, since we don't know what it is
            Size targetSize = new Size(size.Width - 20, size.Height - 20);
            ChangeSizeBy(-20, -20);

            Size newSize = window.Size;
            Assert.That(newSize.Width, Is.EqualTo(targetSize.Width));
            Assert.That(newSize.Height, Is.EqualTo(targetSize.Height));
        }

        [Test]
        public void ShouldBeAbleToSetTheSizeOfTheCurrentWindowFromFrame()
        {
            IWindow window = driver.Manage().Window;
            Size size = window.Size;
            driver.Url = framesetPage;
            driver.SwitchTo().Frame("fourth");

            try
            {
                // resize relative to the initial size, since we don't know what it is
                Size targetSize = new Size(size.Width - 20, size.Height - 20);
                window.Size = targetSize;


                Size newSize = window.Size;
                Assert.That(newSize.Width, Is.EqualTo(targetSize.Width));
                Assert.That(newSize.Height, Is.EqualTo(targetSize.Height));
            }
            finally
            {
                driver.SwitchTo().DefaultContent();
            }
        }

        [Test]
        public void ShouldBeAbleToSetTheSizeOfTheCurrentWindowFromIFrame()
        {
            IWindow window = driver.Manage().Window;
            Size size = window.Size;
            driver.Url = iframePage;
            driver.SwitchTo().Frame("iframe1-name");

            try
            {
                // resize relative to the initial size, since we don't know what it is
                Size targetSize = new Size(size.Width - 20, size.Height - 20);
                window.Size = targetSize;


                Size newSize = window.Size;
                Assert.That(newSize.Width, Is.EqualTo(targetSize.Width));
                Assert.That(newSize.Height, Is.EqualTo(targetSize.Height));
            }
            finally
            {
                driver.SwitchTo().DefaultContent();
            }
        }

        [Test]
        public void ShouldBeAbleToGetThePositionOfTheCurrentWindow()
        {
            Point position = driver.Manage().Window.Position;
            Assert.That(position.X, Is.GreaterThan(0));
            Assert.That(position.Y, Is.GreaterThan(0));
        }

        [Test]
        public void ShouldBeAbleToSetThePositionOfTheCurrentWindow()
        {
            IWindow window = driver.Manage().Window;
            window.Size = new Size(200, 200);
            Point position = window.Position;

            Point targetPosition = new Point(position.X + 10, position.Y + 10);
            window.Position = targetPosition;

            Point newLocation = window.Position;

            Assert.That(newLocation.X, Is.EqualTo(targetPosition.X));
            Assert.That(newLocation.Y, Is.EqualTo(targetPosition.Y));
        }

        [Test]
        public void ShouldBeAbleToMaximizeTheCurrentWindow()
        {
            Size targetSize = new Size(640, 400);

            ChangeSizeTo(targetSize);

            Maximize();

            IWindow window = driver.Manage().Window;
            Assert.That(window.Size.Height, Is.GreaterThan(targetSize.Height));
            Assert.That(window.Size.Width, Is.GreaterThan(targetSize.Width));
        }

        [Test]
        public void ShouldBeAbleToMaximizeTheWindowFromFrame()
        {
            driver.Url = framesetPage;
            ChangeSizeTo(new Size(640, 400));

            driver.SwitchTo().Frame("fourth");
            try
            {
                Maximize();
            }
            finally
            {
                driver.SwitchTo().DefaultContent();
            }
        }

        [Test]
        public void ShouldBeAbleToMaximizeTheWindowFromIframe()
        {
            driver.Url = iframePage;
            ChangeSizeTo(new Size(640, 400));

            driver.SwitchTo().Frame("iframe1-name");
            try
            {
                Maximize();
            }
            finally
            {
                driver.SwitchTo().DefaultContent();
            }
        }

        //------------------------------------------------------------------
        // Tests below here are not included in the Java test suite
        //------------------------------------------------------------------

        [Test]
        [IgnoreBrowser(Browser.IE, "Edge in IE Mode does not support full screen")]
        public void ShouldBeAbleToFullScreenTheCurrentWindow()
        {
            IWindow window = driver.Manage().Window;
            Size origSize = window.Size;

            Size targetSize = new Size(640, 400);

            ChangeSizeTo(targetSize);

            FullScreen();

            Size windowSize = window.Size;

            Assert.That(windowSize.Height, Is.GreaterThan(targetSize.Height));
            Assert.That(windowSize.Width, Is.GreaterThan(targetSize.Width));
        }

        [Test]
        public void ShouldBeAbleToMinimizeTheCurrentWindow()
        {
            Size targetSize = new Size(640, 400);

            ChangeSizeTo(targetSize);

            driver.Manage().Window.Minimize();

            Assert.That(((IJavaScriptExecutor)driver).ExecuteScript("return document.hidden;"), Is.True);
        }

        private void FullScreen()
        {
            IWindow window = driver.Manage().Window;
            Size currentSize = window.Size;
            window.FullScreen();
        }

        private void Maximize()
        {
            IWindow window = driver.Manage().Window;
            Size currentSize = window.Size;
            window.Maximize();
            WaitFor(WindowHeightToBeGreaterThan(currentSize.Height), "Window height was not greater than " + currentSize.Height.ToString());
            WaitFor(WindowWidthToBeGreaterThan(currentSize.Width), "Window width was not greater than " + currentSize.Width.ToString());
        }

        private void ChangeSizeTo(Size targetSize)
        {
            IWindow window = driver.Manage().Window;
            window.Size = targetSize;
            WaitFor(WindowHeightToBeEqualTo(targetSize.Height), "Window height was " + window.Size.Height + " not " + targetSize.Height.ToString());
            WaitFor(WindowWidthToBeEqualTo(targetSize.Width), "Window width was not " + targetSize.Width.ToString());
        }

        private void ChangeSizeBy(int deltaX, int deltaY)
        {
            IWindow window = driver.Manage().Window;
            Size size = window.Size;
            ChangeSizeTo(new Size(size.Width + deltaX, size.Height + deltaY));
        }

        private Func<bool> WindowHeightToBeEqualTo(int height)
        {
            return () => { return driver.Manage().Window.Size.Height == height; };
        }

        private Func<bool> WindowWidthToBeEqualTo(int width)
        {
            return () => { return driver.Manage().Window.Size.Width == width; };
        }

        private Func<bool> WindowHeightToBeGreaterThan(int height)
        {
            return () => { return driver.Manage().Window.Size.Height > height; };
        }

        private Func<bool> WindowWidthToBeGreaterThan(int width)
        {
            return () => { return driver.Manage().Window.Size.Width > width; };
        }

        private Func<bool> WindowHeightToBeLessThan(int height)
        {
            return () => { return driver.Manage().Window.Size.Height < height; };
        }

        private Func<bool> WindowWidthToBeLessThan(int width)
        {
            return () => { return driver.Manage().Window.Size.Width < width; };
        }
    }
}
