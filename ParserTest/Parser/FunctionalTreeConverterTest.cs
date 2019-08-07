namespace EngineTest.Parser
{
    using MicroExpressionParser;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using RPGEngine.Core;
    using RPGEngine.Language;
    using RPGEngine.Parser;

    [TestClass]
    public class FunctionalTreeConverterTest
    {
        public static readonly GameEngine Engine = new GameEngine();

        public static readonly Entity MockPlayer = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };

        [ClassInitialize]
        public static void StartUp(TestContext context)
        {

            Engine.AddPlayer(MockPlayer);
            DamageType trueDamage = new DamageType(Engine, "T", null, null, null, null);
            Engine.AddDamageType(trueDamage);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestGetPropertyTest()
        {
            string expression = $"{Constants.GET_PROP_F}({MockPlayer.Key},STR)";
            Assert.AreEqual(5, TreeResolver.Resolve(expression, Engine).Value.ToDouble());
        }

        [TestMethod]
        public void FunctionalTreeConverterTestComplexFunctionWithAttributes()
        {
            string expression = $"{Constants.GET_PROP_F}({MockPlayer.Key},STR)*100+{Constants.MAX_F}({Constants.GET_PROP_F}({MockPlayer.Key},AGI),3)";
            double expected = MockPlayer.GetProperty("STR").Value * 100 + (MockPlayer.GetProperty("AGI").Value>3? MockPlayer.GetProperty("AGI").Value : 3);
            Assert.AreEqual(expected, TreeResolver.Resolve(expression, Engine).Value.ToDouble());
        }


        [TestMethod]
        public void FunctionalTreeConverterTestSimpleFunction()
        {
            string expression = $"{Constants.MAX_F}(10,20)";
            Assert.AreEqual(20, TreeResolver.Resolve(expression, Engine).Value.ToDouble());
        }

        [TestMethod]
        public void FunctionalTreeConverterTestSimpleOperation()
        {
            string expression = "10*13";
            Assert.AreEqual(130, TreeResolver.Resolve(expression, Engine).Value.ToDouble());
        }

        [TestMethod]
        public void FunctionalTreeConverterTestSimpleOperationNegativeResult()
        {
            string expression = "10-13";
            Assert.AreEqual(-3, TreeResolver.Resolve(expression, Engine).Value.ToDouble());
        }

        [TestMethod]
        public void FunctionalTreeConverterTestNestedOperation()
        {
            string expression = "10*13+10";
            Assert.AreEqual(140, TreeResolver.Resolve(expression, Engine).Value.ToDouble());
        }

        [TestMethod]
        public void FunctionalTreeConverterTestNestedFunctions()
        {
            string expression = $"{Constants.MAX_F}(10,{Constants.MAX_F}(3,4))";
            Assert.AreEqual(10, TreeResolver.Resolve(expression, Engine).Value.ToDouble());
        }

        [TestMethod]
        public void FunctionalTreeConverterTestOperatorAndFunction()
        {
            string expression = $"{Constants.ABS_F}(10-100)";
            Assert.AreEqual(90, TreeResolver.Resolve(expression, Engine).Value.ToDouble());
        }


        [TestMethod]
        public void FunctionalTreeConverterTestArray()
        {
            string expression = $"{Constants.ARRAY_F}(10,10,10)";
            double[] expected = { 10, 10, 10 };
            double[] actual = MeVariable.ToDoubleArray(TreeResolver.Resolve(expression, Engine).Value.ToArray());
            Assert.AreEqual(expected.Length, actual.Length);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestArrayAndFunction()
        {
            string expression = $"{Constants.ARRAY_F}(10,{Constants.MAX_F}(10,20),10)";
            double[] expected = { 10, 20, 10 };
            double[] actual = MeVariable.ToDoubleArray(TreeResolver.Resolve(expression, Engine).Value.ToArray());
            Assert.AreEqual(expected.Length, actual.Length);
            CollectionAssert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void FunctionalTreeConverterTestFunctionWithNoParameters()
        {
            string expression = $"{Constants.GET_PLAYERS_F}()";
            string expected = "MOCK_PLAYER";
            MeVariable player = TreeResolver.Resolve(expression, Engine).Value.ToArray()[0];
            Assert.AreEqual(expected, player.ToEntity().Name);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestHarmEntity()
        {
            string expression = $"{Constants.HARM_F}({MockPlayer.Key},{MockPlayer.Key},T,20)";
            double expected = MockPlayer.GetProperty("CHP").Value - 20;
            TreeResolver.Resolve(expression, Engine);

            Assert.AreEqual(expected, MockPlayer.GetProperty("CHP").Value);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestBooleanOperator()
        {
            string expression = "10>3";
            bool actual = TreeResolver.Resolve(expression, Engine).Value.ToBoolean();
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestBooleanAndUnaryOperator()
        {
            string expression = "!(10>3+8)";
            bool actual = TreeResolver.Resolve(expression, Engine).Value.ToBoolean();
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestExecuteLater()
        {
            string expression = $"{Constants.IF_F}(10>3,10,11)";
            double actual = TreeResolver.Resolve(expression, Engine).Value.ToDouble();
            Assert.AreEqual(10, actual);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestExecuteLaterFunctionThatDoesntChangeThings()
        {
            string expression = $"{Constants.IF_F}(10>3,10,{Constants.HARM_F}({MockPlayer.Key},{MockPlayer.Key},T,10))";
            double expectedHp = MockPlayer.GetProperty("CHP").Value;
            TreeResolver.Resolve(expression, Engine);
            Assert.AreEqual(expectedHp, MockPlayer.GetProperty("CHP").Value);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestExecuteLaterFunctionThatChangesThings()
        {
            string expression = $"{Constants.IF_F}(1>3,10,{Constants.HARM_F}({MockPlayer.Key},{MockPlayer.Key},T,10))";
            double expectedHp = MockPlayer.GetProperty("CHP").Value-10;
            TreeResolver.Resolve(expression, Engine);
            Assert.AreEqual(expectedHp, MockPlayer.GetProperty("CHP").Value);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestNestedIf()
        {
            string expression = $"{Constants.IF_F}({Constants.MAX_F}(10,3)>3,{Constants.IF_F}(10>3,10,20),30)";
            double expected = 10;
            double actual = TreeResolver.Resolve(expression, Engine).Value.ToDouble();
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void FunctionalTreeConverterTestPowerOperator()
        {
            string expression = "10^3";
            double expected = 1000;
            double actual = TreeResolver.Resolve(expression, Engine).Value.ToDouble();
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void FunctionalTreeConverterTestPowerOperatorCombination()
        {
            string expression = "10+10^3+10";
            double expected = 1020;
            double actual = TreeResolver.Resolve(expression, Engine).Value.ToDouble();
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void FunctionalTreeConverterTestCopy()
        {
            string expression = "11+10";
            double[] expected = { 11, 10 };
            MeNode tree = TreeConverter.Build(expression, Engine);
            MeNode copy = tree.Resolve();
            Assert.AreEqual(tree.Value.ToOperator().Character, "+");
            Assert.AreEqual(2, tree.Leaves.Count);
            for (int i = 0; i < expected.Length; ++i)
                Assert.AreEqual(expected[i], tree.Leaves[i].Value.ToDouble());
        }

    }
}

