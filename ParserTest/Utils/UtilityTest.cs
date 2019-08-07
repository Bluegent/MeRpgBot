using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.Utils
{
    using RPGEngine.Utils;

    [TestClass]
    public class UtilityTest
    {
        [TestMethod]
        public void UtilityTestAlwaysTrue()
        {
            for(int i=0;i<100;++i)
                Assert.AreEqual(true,Utility.Chance(100));
        }

        [TestMethod]
        public void UtilityTestAlwaysFalse()
        {
            for (int i = 0; i < 100; ++i)
                Assert.AreEqual(false, Utility.Chance(0));
        }

        [TestMethod]
        public void UtilityTestTenFiftyPercent()
        {
            //statistically ,it should be impossible to fail, but this test still might, don't worry, it's more of a formality
            bool atLeastOnceTrue = false;
            for (int i = 0; i < 100; ++i)
                if (Utility.Chance(50))
                    atLeastOnceTrue = true;
            Assert.AreEqual(true,atLeastOnceTrue);
        }
        [TestMethod]
        public void UtilityTestTenTenPercent()
        {
            //statistically ,it should be impossible to fail, but this test still might, don't worry, it's more of a formality
            bool atLeastOnceTrue = false;
            for (int i = 0; i < 1000; ++i)
                if (Utility.Chance(10))
                    atLeastOnceTrue = true;
            Assert.AreEqual(true, atLeastOnceTrue);
        }
    }
}
