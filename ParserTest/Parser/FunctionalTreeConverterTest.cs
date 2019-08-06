using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroExpressionParser;

namespace ParserTest
{
    using System.Collections.Generic;
    using System.Linq;

    using RPGEngine.Language;

    [TestClass]
    public class FunctionalTreeConverterTest
    {
        public static readonly GameEngine Engine = new GameEngine();

        public static readonly Entity MockPlayer = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };

        [ClassInitialize]
        public static void StartUp(TestContext context)
        {

            Engine.AddPlayer(MockPlayer);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestGetPropertyTest()
        {
            string expression = $"{Constants.GET_PROP_F}({MockPlayer.Key},STR)";
            Assert.AreEqual(5, FunctionalTreeConverter.ResolveTree(expression, Engine).Value.ToDouble());
        }

        [TestMethod]
        public void FunctionalTreeConverterTestComplexFunctionWithAttributes()
        {
            string expression = $"{Constants.GET_PROP_F}({MockPlayer.Key},STR)*100+{Constants.MAX_F}({Constants.GET_PROP_F}({MockPlayer.Key},AGI),3)";
            Assert.AreEqual(505, FunctionalTreeConverter.ResolveTree(expression, Engine).Value.ToDouble());
        }


        [TestMethod]
        public void FunctionalTreeConverterTestSimpleFunction()
        {
            string expression = $"{Constants.MAX_F}(10,20)";
            Assert.AreEqual(20, FunctionalTreeConverter.ResolveTree(expression, Engine).Value.ToDouble());
        }

        [TestMethod]
        public void FunctionalTreeConverterTestSimpleOperation()
        {
            string expression = "10*13";
            Assert.AreEqual(130, FunctionalTreeConverter.ResolveTree(expression, Engine).Value.ToDouble());
        }

        [TestMethod]
        public void FunctionalTreeConverterTestSimpleOperationNegativeResult()
        {
            string expression = "10-13";
            Assert.AreEqual(-3, FunctionalTreeConverter.ResolveTree(expression, Engine).Value.ToDouble());
        }

        [TestMethod]
        public void FunctionalTreeConverterTestNestedOperation()
        {
            string expression = "10*13+10";
            Assert.AreEqual(140, FunctionalTreeConverter.ResolveTree(expression, Engine).Value.ToDouble());
        }

        [TestMethod]
        public void FunctionalTreeConverterTestNestedFunctions()
        {
            string expression = $"{Constants.MAX_F}(10,{Constants.MAX_F}(3,4))";
            Assert.AreEqual(10, FunctionalTreeConverter.ResolveTree(expression, Engine).Value.ToDouble());
        }

        [TestMethod]
        public void FunctionalTreeConverterTestOperatorAndFunction()
        {
            string expression = $"{Constants.ABS_F}(10-100)";
            Assert.AreEqual(90, FunctionalTreeConverter.ResolveTree(expression, Engine).Value.ToDouble());
        }


        [TestMethod]
        public void FunctionalTreeConverterTestArray()
        {
            string expression = $"{Constants.ARRAY_F}(10,10,10)";
            double[] expected = { 10, 10, 10 };
            double[] actual = MeVariable.ToDoubleArray(FunctionalTreeConverter.ResolveTree(expression, Engine).Value.ToArray());
            Assert.AreEqual(expected.Length, actual.Length);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestArrayAndFunction()
        {
            string expression = $"{Constants.ARRAY_F}(10,{Constants.MAX_F}(10,20),10)";
            double[] expected = { 10, 20, 10 };
            double[] actual = MeVariable.ToDoubleArray(FunctionalTreeConverter.ResolveTree(expression, Engine).Value.ToArray());
            Assert.AreEqual(expected.Length, actual.Length);
            CollectionAssert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void FunctionalTreeConverterTestFunctionWithNoParameters()
        {
            string expression = $"{Constants.GET_PLAYERS_F}()";
            string expected = "MOCK_PLAYER";
            MeVariable player = FunctionalTreeConverter.ResolveTree(expression, Engine).Value.ToArray()[0];
            Assert.AreEqual(expected, player.ToEntity().Name);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestHarmEntity()
        {
            string expression = $"{Constants.HARM_F}({MockPlayer.Key},{MockPlayer.Key},P,20)";
            double expected = MockPlayer.GetProperty("CHP").Value - 20;
            FunctionalTreeConverter.ResolveTree(expression, Engine);

            Assert.AreEqual(expected, MockPlayer.GetProperty("CHP").Value);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestBooleanOperator()
        {
            string expression = "10>3";
            bool actual = FunctionalTreeConverter.ResolveTree(expression, Engine).Value.ToBoolean();
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestBooleanAndUnaryOperator()
        {
            string expression = "!(10>3+8)";
            bool actual = FunctionalTreeConverter.ResolveTree(expression, Engine).Value.ToBoolean();
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestExecuteLater()
        {
            string expression = $"{Constants.IF_F}(10>3,10,11)";
            double actual = FunctionalTreeConverter.ResolveTree(expression, Engine).Value.ToDouble();
            Assert.AreEqual(10, actual);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestExecuteLaterFunctionThatDoesntChangeThings()
        {
            string expression = $"{Constants.IF_F}(10>3,10,{Constants.HARM_F}({MockPlayer.Key},{MockPlayer.Key},P,10))";
            double exepectedHp = MockPlayer.GetProperty("CHP").Value;
            FunctionalTreeConverter.ResolveTree(expression, Engine);
            Assert.AreEqual(exepectedHp, MockPlayer.GetProperty("CHP").Value);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestExecuteLaterFunctionThatChangesThings()
        {
            string expression = $"{Constants.IF_F}(1>3,10,{Constants.HARM_F}({MockPlayer.Key},{MockPlayer.Key},P,10))";
            double exepectedHp = MockPlayer.GetProperty("CHP").Value-10;
            FunctionalTreeConverter.ResolveTree(expression, Engine);
            Assert.AreEqual(exepectedHp, MockPlayer.GetProperty("CHP").Value);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestNestedIf()
        {
            string expression = $"{Constants.IF_F}({Constants.MAX_F}(10,3)>3,{Constants.IF_F}(10>3,10,20),30)";
            double exepected = 10;
            double actual = FunctionalTreeConverter.ResolveTree(expression, Engine).Value.ToDouble();
            Assert.AreEqual(exepected, actual);
        }


        [TestMethod]
        public void FunctionalTreeConverterTestPowerOperator()
        {
            string expression = "10^3";
            double exepected = 1000;
            double actual = FunctionalTreeConverter.ResolveTree(expression, Engine).Value.ToDouble();
            Assert.AreEqual(exepected, actual);
        }


        [TestMethod]
        public void FunctionalTreeConverterTestPowerOperatorCombination()
        {
            string expression = "10+10^3+10";
            double exepected = 1020;
            double actual = FunctionalTreeConverter.ResolveTree(expression, Engine).Value.ToDouble();
            Assert.AreEqual(exepected, actual);
        }


        [TestMethod]
        public void FunctionalTreeConverterTestCopy()
        {
            string expression = "11+10";
            double[] expected = { 11, 10 };
            FunctionalNode tree = FunctionalTreeConverter.BuildTree(expression, Engine);
            FunctionalNode copy = FunctionalTreeConverter.ResolveNode(tree, 0);
            Assert.AreEqual(tree.Value.ToOperator().Character, "+");
            Assert.AreEqual(2, tree.Leaves.Count);
            for (int i = 0; i < expected.Length; ++i)
                Assert.AreEqual(expected[i], tree.Leaves[i].Value.ToDouble());
        }

    }
}

