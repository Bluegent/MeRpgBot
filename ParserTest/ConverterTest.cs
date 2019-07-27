using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroExpressionParser;

namespace ParserTest
{
    [TestClass]
    public class ConverterTest
    {

        [TestMethod]
        public void converter_testSimpleFunction()
        {
            string expression = "ABS(STR)";
            Token[] infix = Tokenizer.tokenize(expression);
            Token[] postfix = SYConverter.toPostfix(infix);
            String[] expected = { "(", "STR", "ABS" };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].value);
        }

        [TestMethod]
        public void converter_testMultipleParams()
        {
            string expression = "MAX(STR,INT)";
            Token[] infix = Tokenizer.tokenize(expression);
            Token[] postfix = SYConverter.toPostfix(infix);
            String[] expected = { "(", "STR", "INT", "MAX" };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].value);
        }


        [TestMethod]
        public void converter_testNestedFunctions()
        {
            string expression = "ABS(MAX(STR,INT))";
            Token[] infix = Tokenizer.tokenize(expression);
            Token[] postfix = SYConverter.toPostfix(infix);
            String[] expected = { "(","(","STR","INT","MAX","ABS"};
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].value);
        }

        [TestMethod]
        public void converter_testFunctionAndOperatorMix()
        {
            string expression = "ABS(MAX(STR,INT)+MIN(10,-20))";
            Token[] infix = Tokenizer.tokenize(expression);
            Token[] postfix = SYConverter.toPostfix(infix);
            String[] expected = { "(", "(", "STR", "INT", "MAX","(","10","-20","MIN","+", "ABS" };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].value);
        }


        [TestMethod]
        public void converter_testOperatorWithParenthesis()
        {
            string expression = "10*(2+3+4)";
            Token[] infix = Tokenizer.tokenize(expression);
            Token[] postfix = SYConverter.toPostfix(infix);
            String[] expected = { "10","2","3","+","4","+","*"};
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].value);
        }

    }
}