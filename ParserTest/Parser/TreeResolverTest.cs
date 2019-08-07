using System;
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
        }

        [TestMethod]
        public void TreeResolverTestResolveOnlyFunctionsNamed()
        {
            string expression = $"{Constants.HARM_F}({MockPlayer.Key},{MockPlayer.Key},P,{Constants.GET_PROP_F}({MockPlayer.Key},{Constants.IF_F}({Constants.GET_PROP_F}({MockPlayer.Key},STR) > {Constants.GET_PROP_F}({MockPlayer.Key},AGI),STR,AGI)))";
            MeNode tree = TreeConverter.Build(expression, Engine);
            MeNode partiallyResolved = TreeResolver.ResolveGivenFunction(tree, Constants.GET_PROP_F);
            Assert.AreEqual(tree.Value.Type, partiallyResolved.Value.Type);

        }
    }
}
