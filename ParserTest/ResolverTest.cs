using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroExpressionParser;

namespace ParserTest
{
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public class ResolverTest
    {
        public static readonly Dictionary<string, double> VariableMap = new Dictionary<string, double>();
        public static readonly GameEngine Engine = new GameEngine();

        [ClassInitialize]
        public static void StartUp(TestContext context)
        {
            VariableMap.Add("STR",30);
            VariableMap.Add("INT",10);
            VariableMap.Add("AGI", 5);
            Engine.AddPlayer(new MockEntity(){Name = "MOCK_PLAYER"});
            ParserConstants.Init(Engine);
        }

        [TestMethod]
        public void ResolverTestComplexFunctionWithAttributes()
        {
            string expression = "STR*100+INT/2+MAX(AGI,3)";
            Assert.AreEqual(3010, FunctionalTreeConverter.BuildTree(expression, VariableMap).Value.ToDouble());
        }


        [TestMethod]
        public void ResolverTestSimpleFunction()
        {
            string expression = "MAX(10,20)";
            Assert.AreEqual(20,FunctionalTreeConverter.BuildTree(expression,VariableMap).Value.ToDouble());
        }

        [TestMethod]
        public void ResolverTestSimpleOperation()
        {
            string expression = "10*13";
            Assert.AreEqual(130, FunctionalTreeConverter.BuildTree(expression, VariableMap).Value.ToDouble());
        }

        [TestMethod]
        public void ResolverTestSimpleOperationNegativeResult()
        {
            string expression = "10-13";
            Assert.AreEqual(-3, FunctionalTreeConverter.BuildTree(expression, VariableMap).Value.ToDouble());
        }

        [TestMethod]
        public void ResolverTestNestedOperation()
        {
            string expression = "10*13+10";
            Assert.AreEqual(140, FunctionalTreeConverter.BuildTree(expression, VariableMap).Value.ToDouble());
        }

        [TestMethod]
        public void ResolverTestNestedFunctions()
        {
            string expression = "MAX(10,MIN(3,4))";
            Assert.AreEqual(10, FunctionalTreeConverter.BuildTree(expression, VariableMap).Value.ToDouble());
        }

        [TestMethod]
        public void ResolverTestOperatorAndFunction()
        {
            string expression = "ABS(10-100)";
            Assert.AreEqual(90, FunctionalTreeConverter.BuildTree(expression, VariableMap).Value.ToDouble());
        }


        [TestMethod]
        public void ResolverTestArray()
        {
            string expression = "ARRAY(10,10,10)";
            double[] expected = { 10, 10, 10 };
            double[] actual = MeVariable.ToDoubleArray(FunctionalTreeConverter.BuildTree(expression, VariableMap).Value.ToArray());
            Assert.AreEqual(expected.Length, actual.Length);
            CollectionAssert.AreEqual(expected,actual);
        }

        [TestMethod]
        public void ResolverTestArrayAndFunction()
        {
            string expression = "ARRAY(10,MAX(10,20),10)";
            double[] expected = { 10, 20, 10 };
            double[] actual = MeVariable.ToDoubleArray(FunctionalTreeConverter.BuildTree(expression, VariableMap).Value.ToArray());
            Assert.AreEqual(expected.Length, actual.Length);
            CollectionAssert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void ResolverTestFunctionWithNoParameters()
        {
            string expression = "GET_PLAYERS()";
            string expected = "MOCK_PLAYER";
            MeVariable player  = FunctionalTreeConverter.BuildTree(expression, VariableMap).Value.ToArray()[0];
            Assert.AreEqual(expected,player.ToEntity().Name);
        }
    
}
}

