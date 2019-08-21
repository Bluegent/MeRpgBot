using RPGEngine.GameInterface;

namespace EngineTest.Parser
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using RPGEngine.Core;
    using RPGEngine.Language;
    using RPGEngine.Parser;

    [TestClass]
    public class ConverterTest
    {

        public static readonly GameEngine Engine = new GameEngine(new DiscordLogHelper(new ConsoleLogger()));
        [ClassInitialize]
        public static void StartUp(TestContext context)
        {
        }
        [TestMethod]
        public void ConverterTestSimpleFunction()
        {
            string expression = $"{LConstants.ABS_F}(STR)";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = InfixToPostfix.ToPostfix(infix);
            string[] expected = { "(", "STR", LConstants.ABS_F };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }

        [TestMethod]
        public void ConverterTestFunctionAndOperator()
        {
            string expression = $"{LConstants.ABS_F}(10-100)";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = InfixToPostfix.ToPostfix(infix);
            string[] expected = { "(", "10", "100", "-", LConstants.ABS_F };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }

        [TestMethod]
        public void ConverterTestMultipleParams()
        {
            string expression = $"{LConstants.MAX_F}(STR,INT)";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = InfixToPostfix.ToPostfix(infix);
            string[] expected = { "(", "STR", "INT", LConstants.MAX_F };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }


        [TestMethod]
        public void ConverterTestNestedFunctions()
        {
            string expression = $"{LConstants.ABS_F}({LConstants.MAX_F}(STR,INT))";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = InfixToPostfix.ToPostfix(infix);
            string[] expected = { "(", "(", "STR", "INT", LConstants.MAX_F, LConstants.ABS_F };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }

        [TestMethod]
        public void ConverterTestFunctionAndOperatorMix()
        {
            string expression = $"{LConstants.ABS_F}({LConstants.MAX_F}(STR,INT)+{LConstants.MIN_F}(10,-20))";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = InfixToPostfix.ToPostfix(infix);
            string[] expected = { "(", "(", "STR", "INT", LConstants.MAX_F, "(", "10", "-20", LConstants.MIN_F, "+", LConstants.ABS_F };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }


        [TestMethod]
        public void ConverterTestOperatorWithParenthesis()
        {
            string expression = "10*(2+3+4)";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = InfixToPostfix.ToPostfix(infix);
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
            Token[] postfix = InfixToPostfix.ToPostfix(infix);
            string[] expected = { "X", "Y", "+", "!" };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }

    }
}