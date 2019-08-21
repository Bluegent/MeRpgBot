using System;
using System.Collections.Generic;
using MicroExpressionParser.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPGEngine.Core;
using RPGEngine.Game;
using RPGEngine.Language;
using RPGEngine.GameInterface;
using RPGEngine.Parser;

namespace EngineTest.Core
{
    [TestClass]
    public class CastTest
    {
        private static readonly GameEngine Engine = new GameEngine(new DiscordLogHelper(new ConsoleLogger()));
        private BaseEntity BaseEntity;
        private static SkillTemplate testSkill;
        private static SkillTemplate testChannelSkill;


        [ClassInitialize]
        public static void StartUp(TestContext context)
        {
            DamageType trueDamage = new DamageType(Engine, "T", null, null, null, null);
            trueDamage.Name = "true";
            Engine.AddDamageType(trueDamage);
            testSkill = new SkillTemplate();
            testSkill.Type = SkillType.Cast;
            testSkill.Key = "TEST_CAST";
            SkillLevelTemplate testLevelTemplate = new SkillLevelTemplate();
            testLevelTemplate.Cooldown = TreeConverter.Build("3", Engine);
            testLevelTemplate.Duration = TreeConverter.Build($"{Constants.SourceKeyword}{Constants.PROP_OP}INT", Engine);
            testLevelTemplate.Formulas.Add(TreeConverter.Build($"{Constants.HARM_F}({Constants.TargetKeyword},{Constants.SourceKeyword},{trueDamage.Key},10)",Engine));
            testSkill.ByLevel.Add(testLevelTemplate);

            testChannelSkill = new SkillTemplate();
            testChannelSkill.Type = SkillType.Channel;
            testChannelSkill.Key = "TEST_CHANNEL";
            SkillLevelTemplate testLevelTemplate2 = new SkillLevelTemplate();
            testLevelTemplate2.Cooldown = TreeConverter.Build("120", Engine);
            testLevelTemplate2.Duration = TreeConverter.Build("60", Engine);
            testLevelTemplate2.Interval = TreeConverter.Build("10", Engine);
            testLevelTemplate2.Formulas.Add(TreeConverter.Build($"{Constants.HARM_F}({Constants.TargetKeyword},{Constants.SourceKeyword},{trueDamage.Key},10)", Engine));
            testChannelSkill.ByLevel.Add(testLevelTemplate2);


        }

        [TestInitialize]
        public void Before()
        {
            BaseEntity = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };
            BaseEntity.Skills.Add(testSkill.Key, new SkillInstance() { Skill = testSkill, SkillLevel = 0 });
            BaseEntity.Skills.Add(testChannelSkill.Key, new SkillInstance() { Skill = testChannelSkill, SkillLevel = 0 });
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

        [TestMethod]
        public void CastTestChannelSkill()
        {
            double expected = BaseEntity.GetProperty("CHP").Value - 60;

            BaseEntity.Cast(BaseEntity, testChannelSkill.Key);
            MockTimer timer = (MockTimer) Engine.GetTimer();
            MeNode duration = testChannelSkill.ByLevel[0].Duration;
            duration = Engine.GetSanitizer().ReplaceTargetAndSource(duration, BaseEntity, BaseEntity);
            double actual;
            long skillDuration = duration.Resolve().Value.ToLong(); ;
            for (int i = 0; i <= skillDuration; ++i)
            {
                timer.ForceTick();
                BaseEntity.Update();

            }

            actual = BaseEntity.GetProperty("CHP").Value;
            Assert.AreEqual(expected, actual);

        }
    }
}
