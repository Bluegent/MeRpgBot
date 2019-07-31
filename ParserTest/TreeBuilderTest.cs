using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            String expression = "ABS(STR)";
            SyntacticNode tree = TreeBuilder.ExprToTree(expression);
            String[] expected = { "STR" };
            Assert.AreEqual("ABS",tree.Token.Value);
            for(int i=0 ; i<tree.Parameters.Count;++i)
                Assert.AreEqual(expected[i],tree.Parameters[i].Token.Value);
        }
        [TestMethod]
        public void TreeBuilderTestMultipleParamFunction()
        {
            String expression = "MAX(STR,10,INT)";
            SyntacticNode tree = TreeBuilder.ExprToTree(expression);
            String[] expected = { "STR","10","INT" };
            Assert.AreEqual("MAX", tree.Token.Value);
            Assert.AreEqual(expected.Length,tree.Parameters.Count);
            for (int i = 0; i < tree.Parameters.Count; ++i)
                Assert.AreEqual(expected[i], tree.Parameters[i].Token.Value);
        }

        [TestMethod]
        public void TreeBuilderTestOperator()
        {
            String expression = "STR+INT";
            SyntacticNode tree = TreeBuilder.ExprToTree(expression);
            String[] expected = { "STR","INT" };
            Assert.AreEqual("+", tree.Token.Value);
            Assert.AreEqual(expected.Length, tree.Parameters.Count);
            for (int i = 0; i < tree.Parameters.Count; ++i)
                Assert.AreEqual(expected[i], tree.Parameters[i].Token.Value);
        }

        [TestMethod]
        public void TreeBuilderTestNestedOperators()
        {
            String expression = "STR+INT*10";
            SyntacticNode tree = TreeBuilder.ExprToTree(expression);
            String[] expected = { "STR", "*" };
            String[] nestedExpect = {"INT","10"};
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
            String expression = "STR+MAX(10,11,12)";
            SyntacticNode tree = TreeBuilder.ExprToTree(expression);
            String[] expected = { "STR", "MAX" };
            String[] nestedExpect = { "10", "11","12" };
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
            String expression = "ABS(MAX(STR,INT,AGI))";
            SyntacticNode tree = TreeBuilder.ExprToTree(expression);
            String[] expected = { "MAX" };
            String[] nestedExpect = { "STR", "INT", "AGI" };
            Assert.AreEqual("ABS", tree.Token.Value);
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
            String expression = "MAX(STR,AGI) + MIN(10,INT)";
            SyntacticNode tree = TreeBuilder.ExprToTree(expression);
            String[] expected = {"MAX", "MIN" };
            String[] nestedExpect = { "STR", "AGI" };
            String[] nestedExpect2 = { "10", "INT" };
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
            String expression = "!(X>Y)";
            SyntacticNode tree = TreeBuilder.ExprToTree(expression);
            String[] expected = { ">"};
            String[] nestedExpect = { "X", "Y" };
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
            String expression = "X>Y+Z";
            SyntacticNode tree = TreeBuilder.ExprToTree(expression);
            String[] expected = { "X","+" };
            String[] nestedExpect = { "Y", "Z" };
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
