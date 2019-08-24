using Microsoft.VisualStudio.TestTools.UnitTesting;

using RPGEngine.Core;
using RPGEngine.Language;
using RPGEngine.Parser;
using RPGEngine.GameInterface;
using RPGEngine.Entities;

namespace EngineTest.Parser
{


    [TestClass]
    public class FunctionalTreeConverterTest
    {
        public static GameEngine Engine;
        public static Entity MockPlayer;

        [ClassInitialize]
        public static void StartUp(TestContext ctx)
        {
            
            Engine = new GameEngine(new DiscordLogHelper(new ConsoleLogger()));
            Definer.Instance().Engine = Engine;
            MockPlayer = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };
            Engine.AddPlayer(MockPlayer);
            DamageType trueDamage = new DamageType(Engine, "T", null, null, null, null);
            Engine.AddDamageType(trueDamage);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestGetPropertyTest()
        {
            string expression = $"{LConstants.GET_PROP_F}({MockPlayer.Key},STR)";
            Assert.AreEqual(5, TreeResolver.Resolve(expression, Engine).Value.ToDouble());
        }

        [TestMethod]
        public void FunctionalTreeConverterTestComplexFunctionWithAttributes()
        {
            string expression = $"{LConstants.GET_PROP_F}({MockPlayer.Key},STR)*100+{LConstants.MAX_F}({LConstants.GET_PROP_F}({MockPlayer.Key},AGI),3)";
            double expected = MockPlayer.GetProperty("STR").Value * 100 + (MockPlayer.GetProperty("AGI").Value > 3? MockPlayer.GetProperty("AGI").Value : 3);
            Assert.AreEqual(expected, TreeResolver.Resolve(expression, Engine).Value.ToDouble());
        }


        [TestMethod]
        public void FunctionalTreeConverterTestSimpleFunction()
        {
            string expression = $"{LConstants.MAX_F}(10,20)";
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
            string expression = $"{LConstants.MAX_F}(10,{LConstants.MAX_F}(3,4))";
            Assert.AreEqual(10, TreeResolver.Resolve(expression, Engine).Value.ToDouble());
        }

        [TestMethod]
        public void FunctionalTreeConverterTestOperatorAndFunction()
        {
            string expression = $"{LConstants.ABS_F}(10-100)";
            Assert.AreEqual(90, TreeResolver.Resolve(expression, Engine).Value.ToDouble());
        }


        [TestMethod]
        public void FunctionalTreeConverterTestArray()
        {
            string expression = $"{LConstants.ARRAY_F}(10,10,10)";
            double[] expected = { 10, 10, 10 };
            double[] actual = MeVariable.ToDoubleArray(TreeResolver.Resolve(expression, Engine).Value.ToArray());
            Assert.AreEqual(expected.Length, actual.Length);
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestArrayAndFunction()
        {
            string expression = $"{LConstants.ARRAY_F}(10,{LConstants.MAX_F}(10,20),10)";
            double[] expected = { 10, 20, 10 };
            double[] actual = MeVariable.ToDoubleArray(TreeResolver.Resolve(expression, Engine).Value.ToArray());
            Assert.AreEqual(expected.Length, actual.Length);
            CollectionAssert.AreEqual(expected, actual);
        }
        [TestMethod]
        public void FunctionalTreeConverterTestFunctionWithNoParameters()
        {
            
            string expression = $"{LConstants.GET_PLAYERS_F}()";
            string expected = "MOCK_PLAYER";
            Token[] tokens = Tokenizer.Tokenize(expression);
            MeVariable[] players = TreeResolver.Resolve(tokens, Engine).Value.ToArray();
            Assert.AreEqual(1,players.Length);
            Assert.AreEqual(expected, players[0].ToEntity().Name);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestHarmEntity()
        {
            string expression = $"{LConstants.HARM_F}({MockPlayer.Key},{MockPlayer.Key},T,20)";
            double expected = MockPlayer.GetProperty(Entity.HP_KEY).Value - 20;
            TreeResolver.Resolve(expression, Engine);

            Assert.AreEqual(expected, MockPlayer.GetProperty(Entity.HP_KEY).Value);
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
            string expression = $"{LConstants.IF_F}(10>3,10,11)";
            double actual = TreeResolver.Resolve(expression, Engine).Value.ToDouble();
            Assert.AreEqual(10, actual);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestExecuteLaterFunctionThatDoesntChangeThings()
        {
            string expression = $"{LConstants.IF_F}(10>3,10,{LConstants.HARM_F}({MockPlayer.Key},{MockPlayer.Key},T,10))";
            double expectedHp = MockPlayer.GetProperty(Entity.HP_KEY).Value;
            TreeResolver.Resolve(expression, Engine);
            Assert.AreEqual(expectedHp, MockPlayer.GetProperty(Entity.HP_KEY).Value);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestExecuteLaterFunctionThatChangesThings()
        {
            string expression = $"{LConstants.IF_F}(1>3,10,{LConstants.HARM_F}({MockPlayer.Key},{MockPlayer.Key},T,10))";
            double expectedHp = MockPlayer.GetProperty(Entity.HP_KEY).Value - 10;
            TreeResolver.Resolve(expression, Engine);
            Assert.AreEqual(expectedHp, MockPlayer.GetProperty(Entity.HP_KEY).Value);
        }

        [TestMethod]
        public void FunctionalTreeConverterTestNestedIf()
        {
            string expression = $"{LConstants.IF_F}({LConstants.MAX_F}(10,3)>3,{LConstants.IF_F}(10>3,10,20),30)";
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
            Assert.AreEqual(tree.Value.ToOperator().Key, "+");
            Assert.AreEqual(2, tree.Leaves.Count);
            for (int i = 0; i < expected.Length; ++i)
                Assert.AreEqual(expected[i], tree.Leaves[i].Value.ToDouble());
        }

    }
}

