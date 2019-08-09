namespace EngineTest.Parser
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using RPGEngine.Core;
    using RPGEngine.Language;
    using RPGEngine.Parser;

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
            string expression = $"{Constants.ABS_F}(STR)";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = InfixToPostfix.ToPostfix(infix);
            string[] expected = { "(", "STR", Constants.ABS_F };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }

        [TestMethod]
        public void ConverterTestFunctionAndOperator()
        {
            string expression = $"{Constants.ABS_F}(10-100)";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = InfixToPostfix.ToPostfix(infix);
            string[] expected = { "(", "10", "100", "-", Constants.ABS_F };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }

        [TestMethod]
        public void ConverterTestMultipleParams()
        {
            string expression = $"{Constants.MAX_F}(STR,INT)";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = InfixToPostfix.ToPostfix(infix);
            string[] expected = { "(", "STR", "INT", Constants.MAX_F };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }


        [TestMethod]
        public void ConverterTestNestedFunctions()
        {
            string expression = $"{Constants.ABS_F}({Constants.MAX_F}(STR,INT))";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = InfixToPostfix.ToPostfix(infix);
            string[] expected = { "(", "(", "STR", "INT", Constants.MAX_F, Constants.ABS_F };
            Assert.AreEqual(expected.Length, postfix.Length);
            for (int i = 0; i < postfix.Length; ++i)
                Assert.AreEqual(expected[i], postfix[i].Value);
        }

        [TestMethod]
        public void ConverterTestFunctionAndOperatorMix()
        {
            string expression = $"{Constants.ABS_F}({Constants.MAX_F}(STR,INT)+{Constants.MIN_F}(10,-20))";
            Token[] infix = Tokenizer.Tokenize(expression);
            Token[] postfix = InfixToPostfix.ToPostfix(infix);
            string[] expected = { "(", "(", "STR", "INT", Constants.MAX_F, "(", "10", "-20", Constants.MIN_F, "+", Constants.ABS_F };
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