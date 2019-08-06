using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroExpressionParser;
using MicroExpressionParser.Core;

namespace ParserTest
{
    [TestClass]
    public class ConverterTest
    {

        public static readonly GameEngine Engine = new GameEngine();
        [ClassInitialize]
        public static void StartUp(TestContext context)
        {
        }
        [TestMethod]
        public void ConverterTestSimpleFunction()
        {
            string expression = $"{StringConstants.ABS_F}(STR)";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = SYConverter.ToPostfix(infix);
            string[] expected = { "(", "STR", StringConstants.ABS_F };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }

        [TestMethod]
        public void ConverterTestFunctionAndOperator()
        {
            string expression = $"{StringConstants.ABS_F}(10-100)";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = SYConverter.ToPostfix(infix);
            string[] expected = { "(", "10", "100", "-", StringConstants.ABS_F };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }

        [TestMethod]
        public void ConverterTestMultipleParams()
        {
            string expression = $"{StringConstants.MAX_F}(STR,INT)";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = SYConverter.ToPostfix(infix);
            string[] expected = { "(", "STR", "INT", StringConstants.MAX_F };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }


        [TestMethod]
        public void ConverterTestNestedFunctions()
        {
            string expression = $"{StringConstants.ABS_F}({StringConstants.MAX_F}(STR,INT))";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = SYConverter.ToPostfix(infix);
            string[] expected = { "(", "(", "STR", "INT", StringConstants.MAX_F, StringConstants.ABS_F };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }

        [TestMethod]
        public void ConverterTestFunctionAndOperatorMix()
        {
            string expression = $"{StringConstants.ABS_F}({StringConstants.MAX_F}(STR,INT)+{StringConstants.MIN_F}(10,-20))";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = SYConverter.ToPostfix(infix);
            string[] expected = { "(", "(", "STR", "INT", StringConstants.MAX_F, "(", "10", "-20", StringConstants.MIN_F, "+", StringConstants.ABS_F };
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
            string[] expected = { "10", "2", "3", "+", "4", "+", "*" };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }

        [TestMethod]
        public void ConverterTestUnaryOperator()
        {
            string expression = "!(X+Y)";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = SYConverter.ToPostfix(infix);
            string[] expected = { "X", "Y", "+", "!" };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }

    }
}