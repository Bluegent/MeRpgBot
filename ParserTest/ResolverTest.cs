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
        public static readonly GameEngine Engine = new GameEngine();

        public static readonly Entity MockPlayer = new MockEntity() { Name = "MOCK_PLAYER" ,Key = "MOCK_KEY"};

        [ClassInitialize]
        public static void StartUp(TestContext context)
        {

            Engine.AddPlayer(MockPlayer);
            ParserConstants.Init(Engine);
        }

        [TestMethod]
        public void ResolverTestGetPropertyTest()
        {
            string expression = "GET_PROP(MOCK_KEY,STR)";
            Assert.AreEqual(5, FunctionalTreeConverter.BuildTree(expression, Engine).Value.ToDouble());
        }

        [TestMethod]
        public void ResolverTestComplexFunctionWithAttributes()
        {
            string expression = "GET_PROP(MOCK_KEY,STR)*100+MAX(GET_PROP(MOCK_KEY,AGI),3)";
            Assert.AreEqual(505, FunctionalTreeConverter.BuildTree(expression, Engine).Value.ToDouble());
        }


        [TestMethod]
        public void ResolverTestSimpleFunction()
        {
            string expression = "MAX(10,20)";
            Assert.AreEqual(20,FunctionalTreeConverter.BuildTree(expression,Engine).Value.ToDouble());
        }

        [TestMethod]
        public void ResolverTestSimpleOperation()
        {
            string expression = "10*13";
            Assert.AreEqual(130, FunctionalTreeConverter.BuildTree(expression, Engine).Value.ToDouble());
        }

        [TestMethod]
        public void ResolverTestSimpleOperationNegativeResult()
        {
            string expression = "10-13";
            Assert.AreEqual(-3, FunctionalTreeConverter.BuildTree(expression, Engine).Value.ToDouble());
        }

        [TestMethod]
        public void ResolverTestNestedOperation()
        {
            string expression = "10*13+10";
            Assert.AreEqual(140, FunctionalTreeConverter.BuildTree(expression, Engine).Value.ToDouble());
        }

        [TestMethod]
        public void ResolverTestNestedFunctions()
        {
            string expression = "MAX(10,MIN(3,4))";
            Assert.AreEqual(10, FunctionalTreeConverter.BuildTree(expression, Engine).Value.ToDouble());
        }

        [TestMethod]
        public void ResolverTestOperatorAndFunction()
        {
            string expression = "ABS(10-100)";
            Assert.AreEqual(90, FunctionalTreeConverter.BuildTree(expression, Engine).Value.ToDouble());
        }


        [TestMethod]
        public void ResolverTestArray()
        {
            string expression = "ARRAY(10,10,10)";
            double[] expected = { 10, 10, 10 };
            double[] actual = MeVariable.ToDoubleArray(FunctionalTreeConverter.BuildTree(expression, Engine).Value.ToArray());
            Assert.AreEqual(expected.Length, actual.Length);
            CollectionAssert.AreEqual(expected,actual);
        }

        [TestMethod]
        public void ResolverTestArrayAndFunction()
        {
            string expression = "ARRAY(10,MAX(10,20),10)";
            double[] expected = { 10, 20, 10 };
            double[] actual = MeVariable.ToDoubleArray(FunctionalTreeConverter.BuildTree(expression, Engine).Value.ToArray());
            Assert.AreEqual(expected.Length, actual.Length);
            CollectionAssert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void ResolverTestFunctionWithNoParameters()
        {
            string expression = "GET_PLAYERS()";
            string expected = "MOCK_PLAYER";
            MeVariable player  = FunctionalTreeConverter.BuildTree(expression, Engine).Value.ToArray()[0];
            Assert.AreEqual(expected,player.ToEntity().Name);
        }

        [TestMethod]
        public void ResolverTestHarmEntity()
        {
            string expression = "HARM(MOCK_KEY,P,20)";
            double expected = MockPlayer.GetProperty("CHP").Value - 20;
            FunctionalTreeConverter.BuildTree(expression, Engine);

            Assert.AreEqual(expected,MockPlayer.GetProperty("CHP").Value);
        }


    }
}

