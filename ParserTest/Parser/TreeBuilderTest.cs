using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPGEngine.Logging;

namespace ParserTest
{
    using MicroExpressionParser;

    using RPGEngine.Core;
    using RPGEngine.Language;
    using RPGEngine.Parser;

    /// <summary>
    /// Summary description for BaseObjectTest
    /// </summary>
    [TestClass]
    public class TreeBuilderTest
    {
        public static readonly GameEngine Engine = new GameEngine(new DiscordLogHelper(new ConsoleLogger()));

        [ClassInitialize]
        public static void StartUp(TestContext context)
        {
        }

        [TestMethod]
        public void TreeBuilderTestSingleParamFunction()
        {
            string expression = $"{Constants.ABS_F}(STR)";
            TokenNode tree = TreeBuilder.ExprToTree(expression);
            string[] expected = { "STR" };
            Assert.AreEqual(Constants.ABS_F,tree.Token.Value);
            for(int i=0 ; i<tree.Parameters.Count;++i)
                Assert.AreEqual(expected[i],tree.Parameters[i].Token.Value);
        }
        [TestMethod]
        public void TreeBuilderTestMultipleParamFunction()
        {
            string expression = $"{Constants.MAX_F}(STR,10,INT)";
            TokenNode tree = TreeBuilder.ExprToTree(expression);
            string[] expected = { "STR","10","INT" };
            Assert.AreEqual(Constants.MAX_F, tree.Token.Value);
            Assert.AreEqual(expected.Length,tree.Parameters.Count);
            for (int i = 0; i < tree.Parameters.Count; ++i)
                Assert.AreEqual(expected[i], tree.Parameters[i].Token.Value);
        }

        [TestMethod]
        public void TreeBuilderTestOperator()
        {
            string expression = "STR+INT";
            TokenNode tree = TreeBuilder.ExprToTree(expression);
            string[] expected = { "STR","INT" };
            Assert.AreEqual("+", tree.Token.Value);
            Assert.AreEqual(expected.Length, tree.Parameters.Count);
            for (int i = 0; i < tree.Parameters.Count; ++i)
                Assert.AreEqual(expected[i], tree.Parameters[i].Token.Value);
        }

        [TestMethod]
        public void TreeBuilderTestNestedOperators()
        {
            string expression = "STR+INT*10";
            TokenNode tree = TreeBuilder.ExprToTree(expression);
            string[] expected = { "STR", "*" };
            string[] nestedExpect = {"INT","10"};
            Assert.AreEqual("+", tree.Token.Value);
            Assert.AreEqual(expected.Length, tree.Parameters.Count);
            for (int i = 0; i < tree.Parameters.Count; ++i)
                Assert.AreEqual(expected[i], tree.Parameters[i].Token.Value);

            TokenNode subNode = tree.Parameters[0];
            for (int i=0;i< subNode.Parameters.Count;++i)
                Assert.AreEqual(nestedExpect[i], subNode.Parameters[i].Token.Value);
        }

        [TestMethod]
        public void TreeBuilderTestNestedOperatorsAndFunctions()
        {
            string expression = $"STR+{Constants.MAX_F}(10,11,12)";
            TokenNode tree = TreeBuilder.ExprToTree(expression);
            string[] expected = { "STR", Constants.MAX_F };
            string[] nestedExpect = { "10", "11","12" };
            Assert.AreEqual("+", tree.Token.Value);
            Assert.AreEqual(expected.Length, tree.Parameters.Count);
            for (int i = 0; i < tree.Parameters.Count; ++i)
                Assert.AreEqual(expected[i], tree.Parameters[i].Token.Value);

            TokenNode subNode = tree.Parameters[1];
            for (int i = 0; i < subNode.Parameters.Count; ++i)
                Assert.AreEqual(nestedExpect[i], subNode.Parameters[i].Token.Value);
        }

        [TestMethod]
        public void TreeBuilderTestNestedFunctions()
        {
            string expression = $"{Constants.ABS_F}({Constants.MAX_F}(STR,INT,AGI))";
            TokenNode tree = TreeBuilder.ExprToTree(expression);
            string[] expected = { Constants.MAX_F };
            string[] nestedExpect = { "STR", "INT", "AGI" };
            Assert.AreEqual(Constants.ABS_F, tree.Token.Value);
            Assert.AreEqual(expected.Length, tree.Parameters.Count);
          
            for (int i = 0; i < tree.Parameters.Count; ++i)
                Assert.AreEqual(expected[i], tree.Parameters[i].Token.Value);

            TokenNode subNode = tree.Parameters[0];
            for (int i = 0; i < subNode.Parameters.Count; ++i)
                Assert.AreEqual(nestedExpect[i], subNode.Parameters[i].Token.Value);
        }

        [TestMethod]
        public void TreeBuilderMixOfOperatorsAndFunctions()
        {
            string expression = $"{Constants.MAX_F}(STR,AGI) + {Constants.MIN_F}(10,INT)";
            TokenNode tree = TreeBuilder.ExprToTree(expression);
            string[] expected = {Constants.MAX_F, Constants.MIN_F };
            string[] nestedExpect = { "STR", "AGI" };
            string[] nestedExpect2 = { "10", "INT" };
            Assert.AreEqual("+", tree.Token.Value);
            Assert.AreEqual(expected.Length, tree.Parameters.Count);

            for (int i = 0; i < tree.Parameters.Count; ++i)
                Assert.AreEqual(expected[i], tree.Parameters[i].Token.Value);

            TokenNode subNode = tree.Parameters[0];
            for (int i = 0; i < subNode.Parameters.Count; ++i)
                Assert.AreEqual(nestedExpect[i], subNode.Parameters[i].Token.Value);


            TokenNode subNode2 = tree.Parameters[1];
            for (int i = 0; i < subNode2.Parameters.Count; ++i)
                Assert.AreEqual(nestedExpect2[i], subNode2.Parameters[i].Token.Value);
        }

        [TestMethod]
        public void TreeBuilderUnaryOperator()
        {
            string expression = "!(X>Y)";
            TokenNode tree = TreeBuilder.ExprToTree(expression);
            string[] expected = { ">"};
            string[] nestedExpect = { "X", "Y" };
            Assert.AreEqual("!", tree.Token.Value);
            Assert.AreEqual(expected.Length, tree.Parameters.Count);

            for (int i = 0; i < tree.Parameters.Count; ++i)
                Assert.AreEqual(expected[i], tree.Parameters[i].Token.Value);

            TokenNode subNode = tree.Parameters[0];
            for (int i = 0; i < subNode.Parameters.Count; ++i)
                Assert.AreEqual(nestedExpect[i], subNode.Parameters[i].Token.Value);
        }

        [TestMethod]
        public void TreeBuilderBooleanOperatorPrecedence()
        {
            string expression = "X>Y+Z";
            TokenNode tree = TreeBuilder.ExprToTree(expression);
            string[] expected = { "X","+" };
            string[] nestedExpect = { "Y", "Z" };
            Assert.AreEqual(">", tree.Token.Value);

            Assert.AreEqual(expected.Length, tree.Parameters.Count);
            for (int i = 0; i < tree.Parameters.Count; ++i)
                Assert.AreEqual(expected[i], tree.Parameters[i].Token.Value);

            TokenNode subNode = tree.Parameters[1];
            Assert.AreEqual(nestedExpect.Length, subNode.Parameters.Count);
            for (int i = 0; i < subNode.Parameters.Count; ++i)
                Assert.AreEqual(nestedExpect[i], subNode.Parameters[i].Token.Value);


        }
    }
}
