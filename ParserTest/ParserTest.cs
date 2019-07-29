using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroExpressionParser;

namespace ParserTest
{
    [TestClass]
    public class ParserTest
    {
        [TestMethod]
        public void ParserTestSimpleFunction()
        {
            string expression = "MAX(10,20)";
            Assert.AreEqual(20,MEParser.Parse(expression));
        }
    }
}

