using System;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPGEngine.GameInterface;

namespace EngineTest.Parser
{
    using RPGEngine.Core;
    using RPGEngine.Language;
    using RPGEngine.Parser;

    [TestClass]
    public class TreeResolverTest
    {
        public static readonly GameEngine Engine = new GameEngine(new DiscordLogHelper(new ConsoleLogger()));

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
            string expression = $"{LConstants.HARM_F}({MockPlayer.Key},{MockPlayer.Key},P,{LConstants.GET_PROP_F}({MockPlayer.Key},{LConstants.IF_F}({LConstants.GET_PROP_F}({MockPlayer.Key},STR) > {LConstants.GET_PROP_F}({MockPlayer.Key},AGI),STR,AGI)))";
            MeNode tree = TreeConverter.Build(expression, Engine);
            MeNode partiallyResolved = TreeResolver.ResolveGivenOperations(tree, new string[1] { LConstants.GET_PROP_F});
            Assert.AreEqual(tree.Value.Type, partiallyResolved.Value.Type);

        }


        [TestMethod]
        public void TreeResolverTestArrayProperty()
        {
            string expression = $"{LConstants.ARRAY_F}(10,11,12){LConstants.PROP_OP}{LConstants.ARR_LENGTH}";
            double expected = 3;
            MeNode tree = TreeConverter.Build(expression, Engine);
            MeNode result = tree.Resolve();
            Assert.AreEqual(expected, result.Value.ToDouble());

        }


        [TestMethod]
        public void TreeResolverTestEntityProperty()
        {
            string strKey = "STR";
            string expression = $"{MockPlayer.Key}{LConstants.PROP_OP}{strKey}";
            double expected = MockPlayer.GetProperty(strKey).Value;
            MeNode tree = TreeConverter.Build(expression, Engine);
            MeNode result = tree.Resolve();
            Assert.AreEqual(expected, result.Value.ToDouble());
        }
    }
}
