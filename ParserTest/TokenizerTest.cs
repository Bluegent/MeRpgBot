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
            ParserConstants.init();
        }
        [TestMethod]
        public void testEmptyExpression()
        {
            string expression = "";
            Assert.AreEqual(0,Tokenizer.tokenize(expression).Length);
        }

        [TestMethod]
        public void testEmptySpacedExpression()
        {
            string expression = "                   \n\r\t";
            Assert.AreEqual(0, Tokenizer.tokenize(expression).Length);
        }


        [TestMethod]
        public void testNormalExpression()
        {
            string expression = "MAX(STR*10,INT*10)";
            Token[] resultTokens = Tokenizer.tokenize(expression);
            String[] expectedTokens = {"MAX","(","STR","*","10",",","INT","*","10",")" };
            Assert.AreEqual(expectedTokens.Length, resultTokens.Length);
            for(int i=0;i<resultTokens.Length;++i)
                Assert.AreEqual(expectedTokens[i], resultTokens[i].value);
        }

        [TestMethod]
        public void testNegativeVar()
        {
            string expression = "ABS(-10)";
            Token[] resultTokens = Tokenizer.tokenize(expression);
            String[] expectedTokens = { "ABS","(","-10",")"};
            Assert.AreEqual(expectedTokens.Length, resultTokens.Length);
            for (int i = 0; i < resultTokens.Length; ++i)
                Assert.AreEqual(expectedTokens[i], resultTokens[i].value);
        }


        [TestMethod]
        public void testNestedFunctions()
        {
            string expression = "MAX(ABS(10))";
            Token[] resultTokens = Tokenizer.tokenize(expression);
            String[] expectedTokens = { "MAX", "(", "ABS", "(", "10", ")", ")"};
            Assert.AreEqual(expectedTokens.Length, resultTokens.Length);
            for (int i = 0; i < resultTokens.Length; ++i)
                Assert.AreEqual(expectedTokens[i], resultTokens[i].value);
        }

        [TestMethod]
        public void testSpacedExpression()
        {
            string expression = "MAX   (    STR  *\n10,\n\r    INT*10)";
            Token[] resultTokens = Tokenizer.tokenize(expression);
            String[] expectedTokens = { "MAX", "(", "STR", "*", "10", ",", "INT", "*", "10", ")" };
            Assert.AreEqual(expectedTokens.Length, resultTokens.Length);
            for (int i = 0; i < resultTokens.Length; ++i)
                Assert.AreEqual(expectedTokens[i], resultTokens[i].value);
        }
    }
}

