using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroExpressionParser;

namespace ParserTest
{
    [TestClass]
    public class ConverterTest
    {

        public static readonly GameEngine Engine = new GameEngine();
        [ClassInitialize]
        public static void StartUp(TestContext context)
        {
            ParserConstants.Init(Engine);
        }
        [TestMethod]
        public void ConverterTestSimpleFunction()
        {
            string expression = "ABS(STR)";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = SYConverter.ToPostfix(infix);
            String[] expected = { "(", "STR", "ABS" };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }

        [TestMethod]
        public void ConverterTestFunctionAndOperator()
        {
            string expression = "ABS(10-100)";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = SYConverter.ToPostfix(infix);
            String[] expected = { "(", "10", "100","-","ABS" };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }

        [TestMethod]
        public void ConverterTestMultipleParams()
        {
            string expression = "MAX(STR,INT)";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = SYConverter.ToPostfix(infix);
            String[] expected = { "(", "STR", "INT", "MAX" };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }


        [TestMethod]
        public void ConverterTestNestedFunctions()
        {
            string expression = "ABS(MAX(STR,INT))";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = SYConverter.ToPostfix(infix);
            String[] expected = { "(","(","STR","INT","MAX","ABS"};
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }

        [TestMethod]
        public void ConverterTestFunctionAndOperatorMix()
        {
            string expression = "ABS(MAX(STR,INT)+MIN(10,-20))";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = SYConverter.ToPostfix(infix);
            String[] expected = { "(", "(", "STR", "INT", "MAX","(","10","-20","MIN","+", "ABS" };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }


        [TestMethod]
        public void ConverterTestOperatorWithParenthesis()
        {
            string expression = "10*(2+3+4)";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = SYConverter.ToPostfix(infix);
            String[] expected = { "10","2","3","+","4","+","*"};
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }

    }
}