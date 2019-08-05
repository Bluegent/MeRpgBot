using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroExpressionParser.Core;

namespace ParserTest
{
    using MicroExpressionParser;

    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class TreeBuilderTest
    {
        public static readonly GameEngine Engine = new GameEngine();

        [ClassInitialize]
        public static void StartUp(TestContext context)
        {
            ParserConstants.Init(Engine);
        }

        [TestMethod]
        public void TreeBuilderTestSingleParamFunction()
        {
            string expression = $"{StringConstants.ABS_F}(STR)";
            SyntacticNode tree = TreeBuilder.ExprToTree(expression);
            string[] expected = { "STR" };
            Assert.AreEqual(StringConstants.ABS_F,tree.Token.Value);
            for(int i=0 ; i<tree.Parameters.Count;++i)
                Assert.AreEqual(expected[i],tree.Parameters[i].Token.Value);
        }
        [TestMethod]
        public void TreeBuilderTestMultipleParamFunction()
        {
            string expression = $"{StringConstants.MAX_F}(STR,10,INT)";
            SyntacticNode tree = TreeBuilder.ExprToTree(expression);
            string[] expected = { "STR","10","INT" };
            Assert.AreEqual(StringConstants.MAX_F, tree.Token.Value);
            Assert.AreEqual(expected.Length,tree.Parameters.Count);
            for (int i = 0; i < tree.Parameters.Count; ++i)
                Assert.AreEqual(expected[i], tree.Parameters[i].Token.Value);
        }

        [TestMethod]
        public void TreeBuilderTestOperator()
        {
            string expression = "STR+INT";
            SyntacticNode tree = TreeBuilder.ExprToTree(expression);
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
            SyntacticNode tree = TreeBuilder.ExprToTree(expression);
            string[] expected = { "STR", "*" };
            string[] nestedExpect = {"INT","10"};
            Assert.AreEqual("+", tree.Token.Value);
            Assert.AreEqual(expected.Length, tree.Parameters.Count);
            for (int i = 0; i < tree.Parameters.Count; ++i)
                Assert.AreEqual(expected[i], tree.Parameters[i].Token.Value);

            SyntacticNode subNode = tree.Parameters[0];
            for (int i=0;i< subNode.Parameters.Count;++i)
                Assert.AreEqual(nestedExpect[i], subNode.Parameters[i].Token.Value);
        }

        [TestMethod]
        public void TreeBuilderTestNestedOperatorsAndFunctions()
        {
            string expression = $"STR+{StringConstants.MAX_F}(10,11,12)";
            SyntacticNode tree = TreeBuilder.ExprToTree(expression);
            string[] expected = { "STR", StringConstants.MAX_F };
            string[] nestedExpect = { "10", "11","12" };
            Assert.AreEqual("+", tree.Token.Value);
            Assert.AreEqual(expected.Length, tree.Parameters.Count);
            for (int i = 0; i < tree.Parameters.Count; ++i)
                Assert.AreEqual(expected[i], tree.Parameters[i].Token.Value);

            SyntacticNode subNode = tree.Parameters[1];
            for (int i = 0; i < subNode.Parameters.Count; ++i)
                Assert.AreEqual(nestedExpect[i], subNode.Parameters[i].Token.Value);
        }

        [TestMethod]
        public void TreeBuilderTestNestedFunctions()
        {
            string expression = $"{StringConstants.ABS_F}({StringConstants.MAX_F}(STR,INT,AGI))";
            SyntacticNode tree = TreeBuilder.ExprToTree(expression);
            string[] expected = { StringConstants.MAX_F };
            string[] nestedExpect = { "STR", "INT", "AGI" };
            Assert.AreEqual(StringConstants.ABS_F, tree.Token.Value);
            Assert.AreEqual(expected.Length, tree.Parameters.Count);
          
            for (int i = 0; i < tree.Parameters.Count; ++i)
                Assert.AreEqual(expected[i], tree.Parameters[i].Token.Value);

            SyntacticNode subNode = tree.Parameters[0];
            for (int i = 0; i < subNode.Parameters.Count; ++i)
                Assert.AreEqual(nestedExpect[i], subNode.Parameters[i].Token.Value);
        }

        [TestMethod]
        public void TreeBuilderMixOfOperatorsAndFunctions()
        {
            string expression = $"{StringConstants.MAX_F}(STR,AGI) + {StringConstants.MIN_F}(10,INT)";
            SyntacticNode tree = TreeBuilder.ExprToTree(expression);
            string[] expected = {StringConstants.MAX_F, StringConstants.MIN_F };
            string[] nestedExpect = { "STR", "AGI" };
            string[] nestedExpect2 = { "10", "INT" };
            Assert.AreEqual("+", tree.Token.Value);
            Assert.AreEqual(expected.Length, tree.Parameters.Count);

            for (int i = 0; i < tree.Parameters.Count; ++i)
                Assert.AreEqual(expected[i], tree.Parameters[i].Token.Value);

            SyntacticNode subNode = tree.Parameters[0];
            for (int i = 0; i < subNode.Parameters.Count; ++i)
                Assert.AreEqual(nestedExpect[i], subNode.Parameters[i].Token.Value);


            SyntacticNode subNode2 = tree.Parameters[1];
            for (int i = 0; i < subNode2.Parameters.Count; ++i)
                Assert.AreEqual(nestedExpect2[i], subNode2.Parameters[i].Token.Value);
        }

        [TestMethod]
        public void TreeBuilderUnaryOperator()
        {
            string expression = "!(X>Y)";
            SyntacticNode tree = TreeBuilder.ExprToTree(expression);
            string[] expected = { ">"};
            string[] nestedExpect = { "X", "Y" };
            Assert.AreEqual("!", tree.Token.Value);
            Assert.AreEqual(expected.Length, tree.Parameters.Count);

            for (int i = 0; i < tree.Parameters.Count; ++i)
                Assert.AreEqual(expected[i], tree.Parameters[i].Token.Value);

            SyntacticNode subNode = tree.Parameters[0];
            for (int i = 0; i < subNode.Parameters.Count; ++i)
                Assert.AreEqual(nestedExpect[i], subNode.Parameters[i].Token.Value);
        }

        [TestMethod]
        public void TreeBuilderBooleanOperatorPrecedence()
        {
            string expression = "X>Y+Z";
            SyntacticNode tree = TreeBuilder.ExprToTree(expression);
            string[] expected = { "X","+" };
            string[] nestedExpect = { "Y", "Z" };
            Assert.AreEqual(">", tree.Token.Value);

            Assert.AreEqual(expected.Length, tree.Parameters.Count);
            for (int i = 0; i < tree.Parameters.Count; ++i)
                Assert.AreEqual(expected[i], tree.Parameters[i].Token.Value);

            SyntacticNode subNode = tree.Parameters[1];
            Assert.AreEqual(nestedExpect.Length, subNode.Parameters.Count);
            for (int i = 0; i < subNode.Parameters.Count; ++i)
                Assert.AreEqual(nestedExpect[i], subNode.Parameters[i].Token.Value);


        }
    }
}
