using System;
using CoreSample;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoreSampleTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var givenSomeLogic = new SomeLogic();

            var thenSomething = givenSomeLogic.GetSomething(true);

            Assert.AreEqual("something", thenSomething);
        }

        [TestMethod]
        [Ignore]
        public void TestMethod2()
        {
            var givenSomeLogic = new SomeLogic();

            var thenSomething = givenSomeLogic.GetSomething(false);

            Assert.AreEqual("something", thenSomething);
        }
    }
}
