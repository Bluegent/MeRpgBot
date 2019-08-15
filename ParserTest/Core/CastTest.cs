using System;
using System.Collections.Generic;
using MicroExpressionParser.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPGEngine.Core;
using RPGEngine.Game;
using RPGEngine.Language;
using RPGEngine.Logging;
using RPGEngine.Parser;

namespace EngineTest.Core
{
    [TestClass]
    public class CastTest
    {
        private static readonly GameEngine Engine = new GameEngine(new DiscordLogHelper(new ConsoleLogger()));
        private BaseEntity BaseEntity;
        private static readonly Dictionary<string, SkillTemplate> skills = new Dictionary<string, SkillTemplate>();
        private static SkillTemplate testSkill;

        [ClassInitialize]
        public static void StartUp(TestContext context)
        {
            DamageType trueDamage = new DamageType(Engine, "T", null, null, null, null);
            trueDamage.Name = "true";
            Engine.AddDamageType(trueDamage);
            testSkill = new SkillTemplate();
            testSkill.Type = SkillType.Cast;
            testSkill.Key = "TEST_SKILL";
            SkillLevelTemplate testLevelTemplate = new SkillLevelTemplate();
            testLevelTemplate.Cooldown = TreeConverter.Build("3", Engine);
            testLevelTemplate.Duration = TreeConverter.Build($"{Constants.SourceKeyword}{Constants.PROP_OP}INT", Engine);
            testLevelTemplate.Formulas.Add(TreeConverter.Build($"{Constants.HARM_F}({Constants.TargetKeyword},{Constants.SourceKeyword},{trueDamage.Key},10)",Engine));
            testSkill.ByLevel.Add(testLevelTemplate);

           
        }

        [TestInitialize]
        public void Before()
        {
            BaseEntity = new BaseEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };
            BaseEntity.Skills.Add(testSkill.Key, new SkillInstance() { Skill = testSkill, SkillLevel = 0 });
            Engine.AddPlayer(BaseEntity);
        }

        [TestMethod]
        public void CastTestNullSkill()
        {
            bool expected = false;
            bool result = BaseEntity.Cast(BaseEntity, "NOT_A_SKILL");
            Assert.AreEqual(expected,result);
        }

        [TestMethod]
        public void CastTestDoubleCast()
        {
            bool expected = false;
            bool expectedFirstCast = true;
            bool firstCast = BaseEntity.Cast(BaseEntity, testSkill.Key);
            bool result = BaseEntity.Cast(BaseEntity,testSkill.Key);
            Assert.AreEqual(expectedFirstCast,firstCast);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CastTestFormulaIsNotExecutedInstantly()
        {
            double expected = BaseEntity.GetProperty("CHP").Value;

            BaseEntity.Cast(BaseEntity, testSkill.Key);
            BaseEntity.Update();

            double actual = BaseEntity.GetProperty("CHP").Value;
            Assert.AreEqual(expected,actual);
        }

        [TestMethod]
        public void CastTestFormulaIsExecutedWithTime()
        {
            double expected = BaseEntity.GetProperty("CHP").Value - 10;
            double before = BaseEntity.GetProperty("CHP").Value;

            BaseEntity.Cast(BaseEntity, testSkill.Key);
            MockTimer timer = (MockTimer)Engine.GetTimer();
            MeNode duration = testSkill.ByLevel[0].Duration;
            duration = Engine.GetSanitizer().ReplaceTargetAndSource(duration, BaseEntity, BaseEntity);
            double actual;
            long skillDuration = duration.Resolve().Value.ToLong(); ;
            for(int i=0; i <= skillDuration;++i)
            {
                timer.ForceTick();
                actual = BaseEntity.GetProperty("CHP").Value;
                Assert.AreEqual(before, actual);
                BaseEntity.Update();
            }

            actual = BaseEntity.GetProperty("CHP").Value;
            Assert.AreEqual(expected, actual);

        }
    }
}
