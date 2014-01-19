using System;
using NUnit.Framework;

namespace Weblog.DynamicDomainObject.Test
{
    [TestFixture]
    public class DynamicDomainObjectTest
    {
        [Test]
        public void TestGetNonDynamicProperties()
        {
            dynamic testFixture = new TestFixture();

            Assert.AreEqual(0, testFixture.NonDynamicPropertyStruct);
            Assert.IsNull(testFixture.NonDynamicPropertyStruct2);
            Assert.AreEqual("NonDynamicPropertyOne", testFixture.NonDynamicPropertyOne);
            Assert.IsNull(testFixture.NonDynamicPropertyTwo);
            Assert.IsNotNull(testFixture.NonDynamicPropertyThree);
            Assert.That(testFixture.NonDynamicPropertyThree is TestFixtureType);
        }

        [Test]
        public void TestSetNonDynamicProperties()
        {
            dynamic testFixture = new TestFixture();

            testFixture.NonDynamicPropertyOne = "Value changed";
            testFixture.NonDynamicPropertyStruct = 1;
            testFixture.NonDynamicPropertyStruct2 = 1;

            Assert.AreEqual("Value changed", testFixture.NonDynamicPropertyOne);
            Assert.AreEqual(1, testFixture.NonDynamicPropertyStruct);
            Assert.AreEqual(1, testFixture.NonDynamicPropertyStruct2);
        }


        [Test]
        public void TestGetDynamicProperities()
        {
            dynamic testFixture = new TestFixture();

            Assert.IsNull(testFixture.NonExistsDynamicProperty);
        }


        [Test]
        public void TestSetDynamicProperties()
        {
            dynamic testFixture = new TestFixture();

            testFixture.DynamicPropertyOne = "test";
            testFixture.DynamicPropertyTwo = 1;

            Assert.AreEqual("test", testFixture.DynamicPropertyOne);
            Assert.AreEqual(1, testFixture.DynamicPropertyTwo);
        }

        [Test]
        public void TestConvertType()
        {
            dynamic testFixture = new TypeTestFixture();

            var fixtureInterface = (TestFixtureInterface)testFixture;
            var fixtureInterface2 = (TestFixtureInterface2)testFixture;

            Assert.NotNull(fixtureInterface);
            Assert.IsNull(fixtureInterface2);
        }

        private interface TestFixtureInterface
        {
            
        }

        private interface TestFixtureInterface2
        {
            
        }

        private class TypeTestFixture : DynamicDomainObject, TestFixtureInterface
        {
            
        }

        private class TestFixtureType
        {
        }


        private class TestFixture : DynamicDomainObject
        {
            public String NonDynamicPropertyOne { get; set; }
            public TestFixtureType NonDynamicPropertyTwo { get; set; }
            public TestFixtureType NonDynamicPropertyThree { get; set; }
            public Int32 NonDynamicPropertyStruct { get; set; }
            public Int32? NonDynamicPropertyStruct2 { get; set; }

            public TestFixture()
            {
                NonDynamicPropertyOne = "NonDynamicPropertyOne";
                NonDynamicPropertyThree = new TestFixtureType();
            }
        }
    }
}
