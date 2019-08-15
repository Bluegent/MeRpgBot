using System;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.Parser
{
    using MicroExpressionParser;

    using RPGEngine.Core;
    using RPGEngine.Language;
    using RPGEngine.Parser;

    [TestClass]
    public class TreeResolverTest
    {
        public static readonly GameEngine Engine = new GameEngine();

        public static readonly Entity MockPlayer = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };

        [ClassInitialize]
        public static void StartUp(TestContext context)
        {

            Engine.AddPlayer(MockPlayer);
            Definer.Get().Engine = Engine;
        }

        [TestMethod]
        public void TreeResolverTestResolveOnlyFunctionsNamed()
        {
            string expression = $"{Constants.HARM_F}({MockPlayer.Key},{MockPlayer.Key},P,{Constants.GET_PROP_F}({MockPlayer.Key},{Constants.IF_F}({Constants.GET_PROP_F}({MockPlayer.Key},STR) > {Constants.GET_PROP_F}({MockPlayer.Key},AGI),STR,AGI)))";
            MeNode tree = TreeConverter.Build(expression, Engine);
            MeNode partiallyResolved = TreeResolver.ResolveGivenOperations(tree, new string[1] { Constants.GET_PROP_F});
            Assert.AreEqual(tree.Value.Type, partiallyResolved.Value.Type);

        }


        [TestMethod]
        public void TreeResolverTestArrayProperty()
        {
            string expression = $"{Constants.ARRAY_F}(10,11,12){Constants.PROP_OP}{Constants.ARR_LENGTH}";
            double expected = 3;
            MeNode tree = TreeConverter.Build(expression, Engine);
            MeNode result = tree.Resolve();
            Assert.AreEqual(expected, result.Value.ToDouble());

        }


        [TestMethod]
        public void TreeResolverTestEntityProperty()
        {
            string strKey = "STR";
            string expression = $"{MockPlayer.Key}{Constants.PROP_OP}{strKey}";
            double expected = MockPlayer.GetProperty(strKey).Value;
            MeNode tree = TreeConverter.Build(expression, Engine);
            MeNode result = tree.Resolve();
            Assert.AreEqual(expected, result.Value.ToDouble());
        }
    }
}
