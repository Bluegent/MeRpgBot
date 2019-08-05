using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParserTest
{
    using MicroExpressionParser;
    using MicroExpressionParser.Core;
    using MicroExpressionParser.Sanitizer;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SanitizerTest
    {
        public static readonly GameEngine Engine = new GameEngine();
        public static readonly Sanitizer SanitizerInstance = new Sanitizer(Engine);
        public static readonly Entity MockPlayer = new MockEntity() { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };
        public static readonly Entity MockEnemy = new MockEntity() { Name = "ENEMY", Key = "MOCK_ENEMY" };

        [ClassInitialize]
        public static void StartUp(TestContext context)
        {
            ParserConstants.Init(Engine);
            Engine.AddPlayer(MockPlayer);
            Engine.AddEnemy(MockEnemy);
        }

        [TestMethod]
        public void SanitizerTestSanitizeSimpleCompoundStat()
        {
            string expression = "10+STR/4";
            string[] expected = { "10", "+", StringConstants.GET_PROP_F, "(", MockPlayer.Key, "STR", ")", "/", "4" };
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
            string expression = $"{StringConstants.HARM_F}({StringConstants.TargetKeyword},{StringConstants.SourceKeyword}, P, {StringConstants.GET_PROP_F}({StringConstants.SourceKeyword}, STR))";
            string[] expected = { StringConstants.HARM_F, "(", MockEnemy.Key, ",", MockPlayer.Key, ",", "P", ",", StringConstants.GET_PROP_F, "(", MockPlayer.Key, ",", "STR", ")", ")" };
            Token[] actual = SanitizerInstance.ReplaceEntities(Tokenizer.Tokenize(expression), MockPlayer, MockEnemy);
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < actual.Length; ++i)
                Assert.AreEqual(expected[i], actual[i].Value);
        }

        [TestMethod]
        public void SanitizerTestResolveSimpleSkill()
        {
            string expression = $"{StringConstants.HARM_F}({StringConstants.TargetKeyword},{StringConstants.SourceKeyword}, P, {StringConstants.GET_PROP_F}({StringConstants.SourceKeyword}, STR))";
            double expected = MockEnemy.GetProperty("CHP").Value - MockPlayer.GetProperty("STR").Value;
            SanitizerInstance.SanitizeSkill(expression, MockPlayer, MockEnemy);
            Assert.AreEqual(expected, MockEnemy.GetProperty("CHP").Value);
        }

        [TestMethod]
        public void SanitizerTestResolveMutlipleCalls()
        {
            string expression = $"{StringConstants.HARM_F}({StringConstants.TargetKeyword},{StringConstants.SourceKeyword}, P, {StringConstants.GET_PROP_F}({StringConstants.SourceKeyword}, STR));{StringConstants.HARM_F}({StringConstants.TargetKeyword},{StringConstants.SourceKeyword}, P, {StringConstants.GET_PROP_F}({StringConstants.SourceKeyword}, STR))";
            double expected = MockEnemy.GetProperty("CHP").Value - MockPlayer.GetProperty("STR").Value * 2;
            SanitizerInstance.SanitizeSkill(expression, MockPlayer, MockEnemy);
            Assert.AreEqual(expected, MockEnemy.GetProperty("CHP").Value);
        }

        [TestMethod]
        public void SanitizerTestStatus()
        {
            string expression = "APPLYSTATUS($S,shonen_powerup,60,ARRAY(10,10,-5))";
        }
    }
}
