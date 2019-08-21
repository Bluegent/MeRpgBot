
using MicroExpressionParser.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPGEngine.Core;
using RPGEngine.Game;
using RPGEngine.Language;
using RPGEngine.GameInterface;
using RPGEngine.Parser;

namespace EngineTest.Core
{
    using System.Diagnostics.Eventing.Reader;

    [TestClass]
    public class CastTest
    {
        private static readonly GameEngine Engine = new GameEngine(new DiscordLogHelper(new ConsoleLogger()));
        private BaseEntity _testPlayer;
        private static SkillTemplate _testSkill;

        private static SkillTemplate _instantHarm;
        private static SkillTemplate _testChannelSkill;


        [ClassInitialize]
        public static void StartUp(TestContext context)
        {
            DamageType trueDamage = new DamageType(Engine, "T", null, null, null, null);
            trueDamage.Name = "true";
            Engine.AddDamageType(trueDamage);

            _testSkill = new SkillTemplate();
            _testSkill.Type = SkillType.Cast;
            _testSkill.Key = "TEST_CAST";
            SkillLevelTemplate testLevelTemplate = new SkillLevelTemplate();
            testLevelTemplate.Cooldown = TreeConverter.Build("3", Engine);
            testLevelTemplate.Duration = TreeConverter.Build($"{Constants.SourceKeyword}{Constants.PROP_OP}INT", Engine);
            testLevelTemplate.Formulas.Add(TreeConverter.Build($"{Constants.HARM_F}({Constants.TargetKeyword},{Constants.SourceKeyword},{trueDamage.Key},10)", Engine));
            testLevelTemplate.PushBack = TreeConverter.Build("3", Engine);
            _testSkill.ByLevel.Add(testLevelTemplate);

            _testChannelSkill = new SkillTemplate();
            _testChannelSkill.Type = SkillType.Channel;
            _testChannelSkill.Key = "TEST_CHANNEL";
            SkillLevelTemplate testLevelTemplate2 = new SkillLevelTemplate();
            testLevelTemplate2.Cooldown = TreeConverter.Build("120", Engine);
            testLevelTemplate2.Duration = TreeConverter.Build("60", Engine);
            testLevelTemplate2.Interval = TreeConverter.Build("10", Engine);
            testLevelTemplate2.PushBack = TreeConverter.Build("10", Engine);
            MeNode channelFormula = TreeConverter.Build(
                $"{Constants.HARM_F}({Constants.TargetKeyword},{Constants.SourceKeyword},{trueDamage.Key},10)",
                Engine);
            Engine.GetSanitizer().SetHarmsToPeriodic(channelFormula);
                
            testLevelTemplate2.Formulas.Add(channelFormula);
            _testChannelSkill.ByLevel.Add(testLevelTemplate2);

            _instantHarm = new SkillTemplate();
            _instantHarm.Type = SkillType.Cast;
            _instantHarm.Key = "HURT";
            SkillLevelTemplate hurtLevelTemplate = new SkillLevelTemplate();
            hurtLevelTemplate.Cooldown = TreeConverter.Build("0", Engine);
            hurtLevelTemplate.Duration = TreeConverter.Build("0", Engine);
            hurtLevelTemplate.PushBack = TreeConverter.Build("0", Engine);
            hurtLevelTemplate.Formulas.Add(TreeConverter.Build($"{Constants.HARM_F}({Constants.TargetKeyword},{Constants.SourceKeyword},{trueDamage.Key},10)", Engine));
            _instantHarm.ByLevel.Add(hurtLevelTemplate);


        }

        [TestInitialize]
        public void Before()
        {
            _testPlayer = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };
            _testPlayer.Skills.Add(_testSkill.Key, new SkillInstance() { Skill = _testSkill, SkillLevel = 0 });
            _testPlayer.Skills.Add(_testChannelSkill.Key, new SkillInstance() { Skill = _testChannelSkill, SkillLevel = 0 });
            Engine.AddPlayer(_testPlayer);
        }

        [TestMethod]
        public void CastTestNullSkill()
        {
            bool expected = false;
            bool result = _testPlayer.Cast(_testPlayer, "NOT_A_SKILL");
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CastTestDoubleCast()
        {
            bool expected = false;
            bool expectedFirstCast = true;
            bool firstCast = _testPlayer.Cast(_testPlayer, _testSkill.Key);
            bool result = _testPlayer.Cast(_testPlayer, _testSkill.Key);
            Assert.AreEqual(expectedFirstCast, firstCast);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CastTestFormulaIsNotExecutedInstantly()
        {
            double expected = _testPlayer.GetProperty("CHP").Value;

            _testPlayer.Cast(_testPlayer, _testSkill.Key);
            _testPlayer.Update();

            double actual = _testPlayer.GetProperty("CHP").Value;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CastTestFormulaIsExecutedWithTime()
        {
            double expected = _testPlayer.GetProperty("CHP").Value - 10;
            double before = _testPlayer.GetProperty("CHP").Value;

            _testPlayer.Cast(_testPlayer, _testSkill.Key);
            MockTimer timer = (MockTimer)Engine.GetTimer();
            MeNode duration = _testSkill.ByLevel[0].Duration;
            duration = Engine.GetSanitizer().ReplaceTargetAndSource(duration, _testPlayer, _testPlayer);
            double actual;
            long skillDuration = duration.Resolve().Value.ToLong(); ;
            for (int i = 0; i < skillDuration; ++i)
            {
                timer.ForceTick();
                actual = _testPlayer.GetProperty("CHP").Value;
                Assert.AreEqual(before, actual);
                _testPlayer.Update();
            }

            actual = _testPlayer.GetProperty("CHP").Value;
            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void CastTestChannelSkill()
        {
            double expected = _testPlayer.GetProperty("CHP").Value - 60;

            _testPlayer.Cast(_testPlayer, _testChannelSkill.Key);
            MockTimer timer = (MockTimer)Engine.GetTimer();
            MeNode duration = _testChannelSkill.ByLevel[0].Duration;
            duration = Engine.GetSanitizer().ReplaceTargetAndSource(duration, _testPlayer, _testPlayer);
            double actual;
            long skillDuration = duration.Resolve().Value.ToLong(); ;
            for (int i = 0; i <= skillDuration; ++i)
            {
                timer.ForceTick();
                _testPlayer.Update();

            }

            actual = _testPlayer.GetProperty("CHP").Value;
            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void CastTestPushbackRegularSkill()
        {


            BaseEntity mob = new MockEntity(Engine);
            mob.Skills.Add(_instantHarm.Key, new SkillInstance() { Skill = _instantHarm, SkillLevel = 0 });

            double expectedMobHealth = mob.GetProperty(BaseEntity.C_HP_KEY).Value;
            double expectedMobHealthAfter = expectedMobHealth - 10;
            _testPlayer.Cast(mob, _testSkill.Key);
            mob.Cast(_testPlayer, _instantHarm.Key);
            mob.Update();

            MockTimer timer = (MockTimer)Engine.GetTimer();

            MeNode duration = _testSkill.ByLevel[0].Duration;
            duration = Engine.GetSanitizer().ReplaceTargetAndSource(duration, _testPlayer, _testPlayer);
            long skillDuration = duration.Resolve().Value.ToLong();
            for (int i = 0; i < skillDuration + 2; ++i)
            {
                timer.ForceTick();
                _testPlayer.Update();
                mob.Update();
                Assert.AreEqual(expectedMobHealth, mob.GetProperty(BaseEntity.C_HP_KEY).Value);
            }

            timer.ForceTick();
            _testPlayer.Update();
            mob.Update();
            Assert.AreEqual(expectedMobHealthAfter, mob.GetProperty(BaseEntity.C_HP_KEY).Value);

        }


        [TestMethod]
        public void CastTestPushbackChannelSkill()
        {
            BaseEntity mob = new MockEntity(Engine);
            mob.Skills.Add(_instantHarm.Key, new SkillInstance() { Skill = _instantHarm, SkillLevel = 0 });

            double expectedMobHealth = mob.GetProperty(BaseEntity.C_HP_KEY).Value - 50;
            _testPlayer.Cast(mob, _testChannelSkill.Key);
            mob.Cast(_testPlayer, _instantHarm.Key);
            mob.Update();

            MockTimer timer = (MockTimer)Engine.GetTimer();

            MeNode duration = _testChannelSkill.ByLevel[0].Duration;
            duration = Engine.GetSanitizer().ReplaceTargetAndSource(duration, _testPlayer, _testPlayer);
            long skillDuration = duration.Resolve().Value.ToLong();

            for (int i = 0; i < skillDuration; ++i)
            {
                timer.ForceTick();
                _testPlayer.Update();
                mob.Update();
            }

            timer.ForceTick();
            _testPlayer.Update();
            mob.Update();
            Assert.AreEqual(expectedMobHealth, mob.GetProperty(BaseEntity.C_HP_KEY).Value);
        }
    }
}
