using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroExpressionParser;

namespace ParserTest
{
    using System.Collections.Generic;

    [TestClass]
    public class ResolverTest
    {
        public static readonly Dictionary<string, double> VariableMap = new Dictionary<string, double>();

        [ClassInitialize]
        public static void StartUp(TestContext context)
        {
            VariableMap.Add("STR",30);
            VariableMap.Add("INT",10);
            VariableMap.Add("AGI",5);
        }
        [TestMethod]
        public void ResolverTestComplexFunctionWithAttributes()
        {
            string expression = "STR*100+INT/2+MAX(AGI,3)";
            Assert.AreEqual(3010, ValueResolver.Resolve(expression, VariableMap));
        }


        [TestMethod]
        public void ResolverTestSimpleFunction()
        {
            string expression = "MAX(10,20)";
            Assert.AreEqual(20,ValueResolver.Resolve(expression,VariableMap));
        }

        [TestMethod]
        public void ResolverTestSimpleOperation()
        {
            string expression = "10*13";
            Assert.AreEqual(130, ValueResolver.Resolve(expression, VariableMap));
        }

        [TestMethod]
        public void ResolverTestSimpleOperationNegativeResult()
        {
            string expression = "10-13";
            Assert.AreEqual(-3, ValueResolver.Resolve(expression, VariableMap));
        }

        [TestMethod]
        public void ResolverTestNestedOperation()
        {
            string expression = "10*13+10";
            Assert.AreEqual(140, ValueResolver.Resolve(expression, VariableMap));
        }

        [TestMethod]
        public void ResolverTestNestedFunctions()
        {
            string expression = "MAX(10,MIN(3,4))";
            Assert.AreEqual(10, ValueResolver.Resolve(expression, VariableMap));
        }

        [TestMethod]
        public void ResolverTestOperatorAndFunction()
        {
            string expression = "ABS(10-100)";
            Assert.AreEqual(90, ValueResolver.Resolve(expression, VariableMap));
        }
    }
}

