namespace EngineTest
{
    using MicroExpressionParser;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using RPGEngine.Core;
    using RPGEngine.Language;
    using RPGEngine.Parser;

    [TestClass]
    public class SanitizerTest
    {
        public static readonly GameEngine Engine = new GameEngine();
        public static readonly Sanitizer SanitizerInstance = new Sanitizer(Engine);
        public static readonly Entity MockPlayer = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };
        public static readonly Entity MockEnemy = new MockEntity(Engine) { Name = "ENEMY", Key = "MOCK_ENEMY" };

        [ClassInitialize]
        public static void StartUp(TestContext context)
        {
            Engine.AddPlayer(MockPlayer);
            Engine.AddEnemy(MockEnemy);
            DamageType trueDamage = new DamageType(Engine,"T",null,null,null,null);
            Engine.AddDamageType(trueDamage);
        }

        [TestMethod]
        public void SanitizerTestSanitizeSimpleCompoundStat()
        {
            string expression = "10+STR/4";
            string[] expected = { "10", "+", Constants.GET_PROP_F, "(", MockPlayer.Key, "STR", ")", "/", "4" };
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
            string expression = $"{Constants.HARM_F}({Constants.TargetKeyword},{Constants.SourceKeyword}, T, {Constants.GET_PROP_F}({Constants.SourceKeyword}, STR))";
            string[] expected = { Constants.HARM_F, "(", MockEnemy.Key, ",", MockPlayer.Key, ",", "T", ",", Constants.GET_PROP_F, "(", MockPlayer.Key, ",", "STR", ")", ")" };
            Token[] actual = SanitizerInstance.ReplaceEntities(Tokenizer.Tokenize(expression), MockPlayer, MockEnemy);
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < actual.Length; ++i)
                Assert.AreEqual(expected[i], actual[i].Value);
        }

        [TestMethod]
        public void SanitizerTestResolveSimpleSkill()
        {
            string expression = $"{Constants.HARM_F}({Constants.TargetKeyword},{Constants.SourceKeyword}, T, {Constants.GET_PROP_F}({Constants.SourceKeyword}, STR))";
            double expected = MockEnemy.GetProperty("CHP").Value - MockPlayer.GetProperty("STR").Value;
            SanitizerInstance.SanitizeSkill(expression, MockPlayer, MockEnemy);
            Assert.AreEqual(expected, MockEnemy.GetProperty("CHP").Value);
        }

        [TestMethod]
        public void SanitizerTestResolveMutlipleCalls()
        {
            string expression = $"{Constants.HARM_F}({Constants.TargetKeyword},{Constants.SourceKeyword}, T, {Constants.GET_PROP_F}({Constants.SourceKeyword}, STR));{Constants.HARM_F}({Constants.TargetKeyword},{Constants.SourceKeyword}, T, {Constants.GET_PROP_F}({Constants.SourceKeyword}, STR))";
            double expected = MockEnemy.GetProperty("CHP").Value - MockPlayer.GetProperty("STR").Value * 2;
            SanitizerInstance.SanitizeSkill(expression, MockPlayer, MockEnemy);
            Assert.AreEqual(expected, MockEnemy.GetProperty("CHP").Value);
        }

    }
}
