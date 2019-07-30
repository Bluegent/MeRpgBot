using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroExpressionParser;

namespace ParserTest
{
    [TestClass]
    public class TokenizerTest
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            ParserConstants.Init();
        }
        [TestMethod]
        public void TokenizerTestEmptyExpression()
        {
            string expression = "";
            Assert.AreEqual(0,Tokenizer.Tokenize(expression).Length);
        }

        [TestMethod]
        public void TokenizerTestEmptySpacedExpression()
        {
            string expression = "                   \n\r\t";
            Assert.AreEqual(0, Tokenizer.Tokenize(expression).Length);
        }


        [TestMethod]
        public void TokenizerTestNormalExpression()
        {
            string expression = "MAX(STR*10,INT*10)";
            Token[] resultTokens = Tokenizer.Tokenize(expression);
            String[] expectedTokens = {"MAX","(","STR","*","10",",","INT","*","10",")" };
            Assert.AreEqual(expectedTokens.Length, resultTokens.Length);
            for(int i=0;i<resultTokens.Length;++i)
                Assert.AreEqual(expectedTokens[i], resultTokens[i].Value);
        }

        [TestMethod]
        public void TokenizerTestFunctionAndOperatorMix()
        {
            string expression = "ABS(10-100)";
            Token[] resultTokens = Tokenizer.Tokenize(expression);
            String[] expectedTokens = { "ABS","(","10","-","100",")" };
            Assert.AreEqual(expectedTokens.Length, resultTokens.Length);
            for (int i = 0; i < resultTokens.Length; ++i)
                Assert.AreEqual(expectedTokens[i], resultTokens[i].Value);
        }


        [TestMethod]
        public void TokenizerTestNegativeVar()
        {
            string expression = "ABS(-10)";
            Token[] resultTokens = Tokenizer.Tokenize(expression);
            String[] expectedTokens = { "ABS","(","-10",")"};
            Assert.AreEqual(expectedTokens.Length, resultTokens.Length);
            for (int i = 0; i < resultTokens.Length; ++i)
                Assert.AreEqual(expectedTokens[i], resultTokens[i].Value);
        }


        [TestMethod]
        public void TokenizerTestNestedFunctions()
        {
            string expression = "MAX(ABS(10))";
            Token[] resultTokens = Tokenizer.Tokenize(expression);
            String[] expectedTokens = { "MAX", "(", "ABS", "(", "10", ")", ")"};
            Assert.AreEqual(expectedTokens.Length, resultTokens.Length);
            for (int i = 0; i < resultTokens.Length; ++i)
                Assert.AreEqual(expectedTokens[i], resultTokens[i].Value);
        }

        [TestMethod]
        public void TokenizerTestSpacedExpression()
        {
            string expression = "MAX   (    STR  *\n10,\n\r    INT*10)";
            Token[] resultTokens = Tokenizer.Tokenize(expression);
            String[] expectedTokens = { "MAX", "(", "STR", "*", "10", ",", "INT", "*", "10", ")" };
            Assert.AreEqual(expectedTokens.Length, resultTokens.Length);
            for (int i = 0; i < resultTokens.Length; ++i)
                Assert.AreEqual(expectedTokens[i], resultTokens[i].Value);
        }

        [TestMethod]
        public void TokenizerTestDoubleValue()
        {
            string expression = "10.5-3";
            Token[] resultTokens = Tokenizer.Tokenize(expression);
            String[] expectedTokens = { "10.5","-","3" };
            Assert.AreEqual(expectedTokens.Length, resultTokens.Length);
            for (int i = 0; i < resultTokens.Length; ++i)
                Assert.AreEqual(expectedTokens[i], resultTokens[i].Value);
        }


    }
}

