using Microsoft.VisualStudio.TestTools.UnitTesting;

using RPGEngine.Entities;
using RPGEngine.Core;
using RPGEngine.Language;
using RPGEngine.Parser;
using RPGEngine.GameInterface;
using RPGEngine.Cleanup;

namespace EngineTest
{
    using RPGEngine.Templates;

    [TestClass]
    public class SanitizerTest
    {
        public static readonly GameEngine Engine = new GameEngine(new DiscordLogHelper(new ConsoleLogger()));
        public static readonly Sanitizer SanitizerInstance = new Sanitizer(Engine);
        public static readonly Entity MockPlayer = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };
        public static readonly Entity MockEnemy = new MockEntity(Engine) { Name = "ENEMY", Key = "MOCK_ENEMY" };

        [ClassInitialize]
        public static void StartUp(TestContext context)
        {
            Engine.AddPlayer(MockPlayer);
            Engine.AddEnemy(MockEnemy);
            DamageTypeTemplate trueDamage = new DamageTypeTemplate(Engine,null,null,null,null);
            trueDamage.Key = "T";
            Engine.AddDamageType(trueDamage);
        }

        [TestMethod]
        public void SanitizerTestSanitizeSimpleCompoundStat()
        {
            string expression = "10+STR/4";
            string[] expected = { "10", "+", LConstants.GET_PROP_F, "(", MockPlayer.Key, "STR", ")", "/", "4" };
            Token[] actual = SanitizerInstance.ReplaceProperties(Tokenizer.Tokenize(expression), MockPlayer);
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < actual.Length; ++i)
                Assert.AreEqual(expected[i], actual[i].Value);

        }

        [TestMethod]
        public void SanitizerTestResolveSimpleCompoundStat()
        {
            string expression = "10+STR/4";
            double expected = 10 + MockPlayer.GetProperty("STR").Value / 4;
            double actual = SanitizerInstance.SanitizeCompoundStat(MockPlayer, expression);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SanitizerTestSanitizeSimpleSkill()
        {
            string expression = $"{LConstants.HARM_F}({LConstants.TargetKeyword},{LConstants.SourceKeyword}, T, {LConstants.GET_PROP_F}({LConstants.SourceKeyword}, STR))";
            string[] expected = { LConstants.HARM_F, "(", MockEnemy.Key, ",", MockPlayer.Key, ",", "T", ",", LConstants.GET_PROP_F, "(", MockPlayer.Key, ",", "STR", ")", ")" };
            Token[] actual = SanitizerInstance.ReplaceEntities(Tokenizer.Tokenize(expression), MockPlayer, MockEnemy);
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < actual.Length; ++i)
                Assert.AreEqual(expected[i], actual[i].Value);
        }

        [TestMethod]
        public void SanitizerTestResolveSimpleSkill()
        {
            string expression = $"{LConstants.HARM_F}({LConstants.TargetKeyword},{LConstants.SourceKeyword}, T, {LConstants.GET_PROP_F}({LConstants.SourceKeyword}, STR))";
            double expected = MockEnemy.GetProperty(Entity.HP_KEY).Value - MockPlayer.GetProperty("STR").Value;
            SanitizerInstance.SanitizeSkill(expression, MockPlayer, MockEnemy);
            Assert.AreEqual(expected, MockEnemy.GetProperty(Entity.HP_KEY).Value);
        }

        [TestMethod]
        public void SanitizerTestResolveMutlipleCalls()
        {
            string expression = $"{LConstants.HARM_F}({LConstants.TargetKeyword},{LConstants.SourceKeyword}, T, {LConstants.GET_PROP_F}({LConstants.SourceKeyword}, STR));{LConstants.HARM_F}({LConstants.TargetKeyword},{LConstants.SourceKeyword}, T, {LConstants.GET_PROP_F}({LConstants.SourceKeyword}, STR))";
            double expected = MockEnemy.GetProperty(Entity.HP_KEY).Value - MockPlayer.GetProperty("STR").Value * 2;
            SanitizerInstance.SanitizeSkill(expression, MockPlayer, MockEnemy);
            Assert.AreEqual(expected, MockEnemy.GetProperty(Entity.HP_KEY).Value);
        }

        [TestMethod]

        public void SanitizerTestReplacePropetyOperators()
        {
            
            string expected = "AGI";
            string expression = $"1 + {expected}";
            double expectedValue = MockPlayer.GetProperty(expected).Value + 1;

            MeNode result = Sanitizer.ReplacePropeties(TreeConverter.Build(expression, Engine),MockPlayer);

            Assert.AreEqual(LConstants.PROP_OP, result.Leaves[1].Value.GetString());
            Assert.AreEqual(expected,result.Leaves[1].Leaves[1].Value.ToMeString());

            MeNode resolved = result.Resolve();
            
            Assert.AreEqual(expectedValue,resolved.Value.ToDouble());
        }

    }
}
