// <copyright file="ExecuteJavaScriptTest.cs" company="Selenium Committers">
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

using Moq;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OpenQA.Selenium.Support.Extensions
{
    public interface IJavaScriptExecutingWebDriver : IWebDriver, IJavaScriptExecutor
    {
    }

    public class SubClassOfReadOnlyCollectionOfObject : ReadOnlyCollection<object>
    {
        public SubClassOfReadOnlyCollectionOfObject(IList<object> list) : base(list)
        {
        }
    }

    [TestFixture]
    public class ExecuteJavaScriptTest
    {
        private const string JavaScript = "Hello, World";
        private static readonly object[] JavaScriptParameters = new object[0];

        private Mock<IJavaScriptExecutingWebDriver> driver;

        [SetUp]
        public void TestSetUp()
        {
            driver = new Mock<IJavaScriptExecutingWebDriver>();
        }

        [Test]
        public void ShouldConvertToIEnumerable()
        {
            var expected = new ReadOnlyCollection<object>(new List<object>());

            driver.Setup(_ => _.ExecuteScript(It.IsAny<string>(), It.IsAny<object[]>())).Returns(expected);

            Assert.That(
                () => driver.Object.ExecuteJavaScript<IEnumerable>(JavaScript, JavaScriptParameters),
                Throws.Nothing);
        }

        [Test]
        public void ShouldConvertToIEnumerableOfObject()
        {
            var expected = new ReadOnlyCollection<object>(new List<object>());

            driver.Setup(_ => _.ExecuteScript(It.IsAny<string>(), It.IsAny<object[]>())).Returns(expected);

            Assert.That(
                () => driver.Object.ExecuteJavaScript<IEnumerable<object>>(JavaScript, JavaScriptParameters),
                Throws.Nothing);
        }

        [Test]
        public void ShouldNotConvertToIEnumerableOfInteger()
        {
            var expected = new ReadOnlyCollection<object>(new List<object>());

            driver.Setup(_ => _.ExecuteScript(It.IsAny<string>(), It.IsAny<object[]>())).Returns(expected);

            Assert.That(
                () => driver.Object.ExecuteJavaScript<IEnumerable<int>>(JavaScript, JavaScriptParameters),
                Throws.TypeOf<WebDriverException>());
        }

        [Test]
        public void ShouldConvertToReadOnlyCollectionOfObject()
        {
            var expected = new ReadOnlyCollection<object>(new List<object>());

            driver.Setup(_ => _.ExecuteScript(It.IsAny<string>(), It.IsAny<object[]>())).Returns(expected);

            Assert.That(
                () => driver.Object.ExecuteJavaScript<ReadOnlyCollection<object>>(JavaScript, JavaScriptParameters),
                Throws.Nothing);
        }

        [Test]
        public void ShouldNotConvertToSubClassOfReadOnlyCollectionOfObject()
        {
            var expected = new ReadOnlyCollection<object>(new List<object>());

            driver.Setup(_ => _.ExecuteScript(It.IsAny<string>(), It.IsAny<object[]>())).Returns(expected);

            Assert.That(
                () => driver.Object.ExecuteJavaScript<SubClassOfReadOnlyCollectionOfObject>(JavaScript, JavaScriptParameters),
                Throws.TypeOf<WebDriverException>());
        }

        [Test]
        public void ShouldNotThrowWhenNullIsReturned()
        {
            driver.Setup(_ => _.ExecuteScript(It.IsAny<string>(), It.IsAny<object[]>())).Returns(null);

            Assert.That(
                () => driver.Object.ExecuteJavaScript<string>(JavaScript, JavaScriptParameters),
                Throws.Nothing);
        }

        [Test]
        public void ShouldNotThrowWhenNullIsReturnedForNullableValueType()
        {
            driver.Setup(_ => _.ExecuteScript(It.IsAny<string>(), It.IsAny<object[]>())).Returns(null);

            Assert.That(
                () => driver.Object.ExecuteJavaScript<int?>(JavaScript, JavaScriptParameters),
                Throws.Nothing);
        }

        [Test]
        public void ShouldThrowWhenNullIsReturnedForValueType()
        {
            driver.Setup(_ => _.ExecuteScript(It.IsAny<string>(), It.IsAny<object[]>())).Returns(null);

            Assert.That(
                () => driver.Object.ExecuteJavaScript<int>(JavaScript, JavaScriptParameters),
                Throws.TypeOf<WebDriverException>());
        }

        [Test]
        public void ShouldAllowExecutingJavaScriptWithoutReturningResult()
        {
            driver.Setup(_ => _.ExecuteScript(It.IsAny<string>(), It.IsAny<object[]>())).Returns(null);

            Assert.That(
                () => driver.Object.ExecuteJavaScript(JavaScript, JavaScriptParameters),
                Throws.Nothing);
        }
    }
}
