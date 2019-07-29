using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroExpressionParser;

namespace ParserTest
{
    using System.Collections.Generic;

    [TestClass]
    public class ParserTest
    {
        public static readonly Dictionary<string, double> VARIABLE_MAP = new Dictionary<string, double>();

        [TestInitialize]
        public void StartUp()
        {
            VARIABLE_MAP.Add("STR",30);
            VARIABLE_MAP.Add("INT",10);
            VARIABLE_MAP.Add("AGI",5);
        }

        [TestMethod]
        public void ParserTestSimpleFunction()
        {
            string expression = "MAX(10,20)";
            Assert.AreEqual(20,ValueResolver.Resolve(expression,VARIABLE_MAP));
        }
    }
}

