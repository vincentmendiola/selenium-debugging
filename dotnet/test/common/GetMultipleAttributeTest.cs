// <copyright file="GetMultipleAttributeTest.cs" company="Selenium Committers">
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

namespace OpenQA.Selenium
{
    [TestFixture]
    public class GetMultipleAttributeTest : DriverTestFixture
    {
        [Test]
        public void MultipleAttributeShouldBeNullWhenNotSet()
        {
            driver.Url = selectPage;
            IWebElement element = driver.FindElement(By.Id("selectWithoutMultiple"));
            Assert.That(element.GetAttribute("multiple"), Is.Null);
        }

        [Test]
        public void MultipleAttributeShouldBeTrueWhenSet()
        {
            driver.Url = selectPage;
            IWebElement element = driver.FindElement(By.Id("selectWithMultipleEqualsMultiple"));
            Assert.That(element.GetAttribute("multiple"), Is.EqualTo("true"));
        }

        [Test]
        public void MultipleAttributeShouldBeTrueWhenSelectHasMutilpeWithValueAsBlank()
        {
            driver.Url = selectPage;
            IWebElement element = driver.FindElement(By.Id("selectWithEmptyStringMultiple"));
            Assert.That(element.GetAttribute("multiple"), Is.EqualTo("true"));
        }

        [Test]
        public void MultipleAttributeShouldBeTrueWhenSelectHasMutilpeWithoutAValue()
        {
            driver.Url = selectPage;
            IWebElement element = driver.FindElement(By.Id("selectWithMultipleWithoutValue"));
            Assert.That(element.GetAttribute("multiple"), Is.EqualTo("true"));
        }

        [Test]
        public void MultipleAttributeShouldBeTrueWhenSelectHasMutilpeWithValueAsSomethingElse()
        {
            driver.Url = selectPage;
            IWebElement element = driver.FindElement(By.Id("selectWithRandomMultipleValue"));
            Assert.That(element.GetAttribute("multiple"), Is.EqualTo("true"));
        }
    }
}
