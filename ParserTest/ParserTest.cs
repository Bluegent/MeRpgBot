using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroExpressionParser;

namespace ParserTest
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod]
        public void testSimpleFunction()
        {
            string expression = "MAX(10,20)";
            Assert.AreEqual(20,MEParser.parse(expression));
        }
    }
}

