using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPGEngine.Core;
using RPGEngine.Game;
using RPGEngine.Language;
using RPGEngine.GameInterface;
using RPGEngine.Parser;
using RPGEngine.Entities;
using RPGEngine.Cleanup;

namespace EngineTest.Core
{

    [TestClass]
    public class CastTest
    {
        private static readonly GameEngine Engine = new GameEngine(new DiscordLogHelper(new ConsoleLogger()));
        private BaseEntity _testPlayer;
        private static SkillTemplate _testSkill;

        private static SkillTemplate _instantHarm;
        private static SkillTemplate _testChannelSkill;
        private static SkillTemplate _costly;
        private static SkillTemplate _unpushable;


        [ClassInitialize]
        public static void StartUp(TestContext context)
        {
            DamageType trueDamage = new DamageType(Engine, "T", null, null, null, null);
            trueDamage.Name = "true";
            Engine.AddDamageType(trueDamage);

            SkillCost nullCost = new SkillCost("MP", TreeConverter.Build("0", Engine));
            SkillCost notFree = new SkillCost("MP", TreeConverter.Build("50", Engine));

            _testSkill = new SkillTemplate();
            _testSkill.Type = SkillType.Cast;
            _testSkill.Key = "TEST_CAST";
            SkillLevelTemplate testLevelTemplate = new SkillLevelTemplate();
            testLevelTemplate.Cooldown = TreeConverter.Build("3", Engine);
            testLevelTemplate.Duration = TreeConverter.Build($"{LConstants.SourceKeyword}{LConstants.PROP_OP}INT", Engine);
            testLevelTemplate.Interruptible = TreeConverter.Build("true", Engine);
            testLevelTemplate.Formulas.Add(TreeConverter.Build($"{LConstants.HARM_F}({LConstants.TargetKeyword},{LConstants.SourceKeyword},{trueDamage.Key},10)", Engine));
            testLevelTemplate.PushBack = TreeConverter.Build("true", Engine);
            testLevelTemplate.Cost = nullCost;
            _testSkill.ByLevel.Add(testLevelTemplate);

            _testChannelSkill = new SkillTemplate();
            _testChannelSkill.Type = SkillType.Channel;
            _testChannelSkill.Key = "TEST_CHANNEL";
            SkillLevelTemplate testLevelTemplate2 = new SkillLevelTemplate();
            testLevelTemplate2.Cooldown = TreeConverter.Build("120", Engine);
            testLevelTemplate2.Duration = TreeConverter.Build("60", Engine);
            testLevelTemplate2.Interval = TreeConverter.Build("10", Engine);
            testLevelTemplate2.PushBack = TreeConverter.Build("true", Engine);
            testLevelTemplate2.Interruptible = TreeConverter.Build("true", Engine);
            testLevelTemplate2.Cost = nullCost;
            MeNode channelFormula = TreeConverter.Build(
                $"{LConstants.HARM_F}({LConstants.TargetKeyword},{LConstants.SourceKeyword},{trueDamage.Key},10)",
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
            hurtLevelTemplate.PushBack = TreeConverter.Build("false", Engine);
            hurtLevelTemplate.Interruptible = TreeConverter.Build("true", Engine);
            hurtLevelTemplate.Formulas.Add(TreeConverter.Build($"{LConstants.HARM_F}({LConstants.TargetKeyword},{LConstants.SourceKeyword},{trueDamage.Key},10)", Engine));
            hurtLevelTemplate.Cost = nullCost;
            _instantHarm.ByLevel.Add(hurtLevelTemplate);



            _unpushable = new SkillTemplate();
            _unpushable.Type = SkillType.Cast;
            _unpushable.Key = "NOPUSH";
            SkillLevelTemplate unpushTemplate = new SkillLevelTemplate();
            unpushTemplate.Cooldown = TreeConverter.Build("0", Engine);
            unpushTemplate.Duration = TreeConverter.Build("5", Engine);
            unpushTemplate.Interruptible = TreeConverter.Build("false", Engine);
            unpushTemplate.PushBack = TreeConverter.Build("false", Engine);
            unpushTemplate.Formulas.Add(TreeConverter.Build($"{LConstants.HARM_F}({LConstants.TargetKeyword},{LConstants.SourceKeyword},{trueDamage.Key},10)", Engine));
            unpushTemplate.Cost = nullCost;
            _unpushable.ByLevel.Add(unpushTemplate);


            _costly = new SkillTemplate();
            _costly.Type = SkillType.Cast;
            _costly.Key = "COSTLY";
            SkillLevelTemplate costlyTemplate = new SkillLevelTemplate();
            costlyTemplate.Cooldown = TreeConverter.Build("0", Engine);
            costlyTemplate.Duration = TreeConverter.Build("0", Engine);
            costlyTemplate.Interruptible = TreeConverter.Build("true", Engine);
            costlyTemplate.Formulas.Add(TreeConverter.Build($"{LConstants.HARM_F}({LConstants.TargetKeyword},{LConstants.SourceKeyword},{trueDamage.Key},10)", Engine));
            costlyTemplate.PushBack = TreeConverter.Build("true", Engine);
            costlyTemplate.Cost = notFree;
            _costly.ByLevel.Add(costlyTemplate);
        }

        [TestInitialize]
        public void Before()
        {
            _testPlayer = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };
            _testPlayer.Skills.Add(_testSkill.Key, new SkillInstance() { Skill = _testSkill, SkillLevel = 0 });
            _testPlayer.Skills.Add(_testChannelSkill.Key, new SkillInstance() { Skill = _testChannelSkill, SkillLevel = 0 });
            _testPlayer.Skills.Add(_unpushable.Key, new SkillInstance() { SkillLevel = 0, Skill = _unpushable });
            _testPlayer.Skills.Add(_costly.Key,new SkillInstance() {SkillLevel =  0, Skill = _costly});
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
            double expected = _testPlayer.GetProperty(Entity.HP_KEY).Value;

            _testPlayer.Cast(_testPlayer, _testSkill.Key);
            _testPlayer.Update();

            double actual = _testPlayer.GetProperty(Entity.HP_KEY).Value;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CastTestFormulaIsExecutedWithTime()
        {
            double expected = _testPlayer.GetProperty(Entity.HP_KEY).Value - 10;
            double before = _testPlayer.GetProperty(Entity.HP_KEY).Value;

            _testPlayer.Cast(_testPlayer, _testSkill.Key);
            MockTimer timer = (MockTimer)Engine.GetTimer();
            MeNode duration = _testSkill.ByLevel[0].Duration;
            duration = Sanitizer.ReplaceTargetAndSource(duration, _testPlayer, _testPlayer);
            double actual;
            long skillDuration = duration.Resolve().Value.ToLong(); ;
            for (int i = 0; i < skillDuration; ++i)
            {
                timer.ForceTick();
                actual = _testPlayer.GetProperty(Entity.HP_KEY).Value;
                Assert.AreEqual(before, actual);
                _testPlayer.Update();
            }

            actual = _testPlayer.GetProperty(Entity.HP_KEY).Value;
            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void CastTestChannelSkill()
        {
            BaseEntity mob = new MockEntity(Engine);
            double expected = mob.GetProperty(Entity.HP_KEY).Value - 60;

            _testPlayer.Cast(mob, _testChannelSkill.Key);
            MockTimer timer = (MockTimer)Engine.GetTimer();
            MeNode duration = _testChannelSkill.ByLevel[0].Duration;
            duration = Sanitizer.ReplaceTargetAndSource(duration, _testPlayer, _testPlayer);
            
            long skillDuration = duration.Resolve().Value.ToLong(); 
            for (int i = 0; i <= skillDuration; ++i)
            {
                timer.ForceTick();
                _testPlayer.Update();
            }
          
            double actual = mob.GetProperty(Entity.HP_KEY).Value;
            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void CastTestPushbackRegularSkill()
        {


            BaseEntity mob = new MockEntity(Engine);
            long delay = 5;

            double expectedMobHealth = mob.GetProperty(Entity.HP_KEY).Value;
            double expectedMobHealthAfter = expectedMobHealth - 10;
            _testPlayer.Cast(mob, _testSkill.Key);
            _testPlayer.AddPushback(delay);

            MockTimer timer = (MockTimer)Engine.GetTimer();

            MeNode duration = _testSkill.ByLevel[0].Duration;
            duration = Sanitizer.ReplaceTargetAndSource(duration, _testPlayer, _testPlayer);
            long skillDuration = duration.Resolve().Value.ToLong();
            for (int i = 0; i < skillDuration + delay - 1; ++i)
            {
                timer.ForceTick();
                _testPlayer.Update();
                mob.Update();
                Assert.AreEqual(expectedMobHealth, mob.GetProperty(Entity.HP_KEY).Value);
            }

            timer.ForceTick();
            _testPlayer.Update();
            mob.Update();
            Assert.AreEqual(expectedMobHealthAfter, mob.GetProperty(Entity.HP_KEY).Value);

        }


        [TestMethod]
        public void CastTestPushbackChannelSkill()
        {
            long delay = 10;
            BaseEntity mob = new MockEntity(Engine);

            double expectedMobHealth = mob.GetProperty(Entity.HP_KEY).Value - 50;
            _testPlayer.Cast(mob, _testChannelSkill.Key);
            _testPlayer.AddPushback(delay);

            MockTimer timer = (MockTimer)Engine.GetTimer();

            MeNode duration = _testChannelSkill.ByLevel[0].Duration;
            duration = Sanitizer.ReplaceTargetAndSource(duration, _testPlayer, _testPlayer);
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
            Assert.AreEqual(expectedMobHealth, mob.GetProperty(Entity.HP_KEY).Value);
        }

        [TestMethod]
        public void CastTestNoPushBack()
        {
            long delay = 10;
            BaseEntity mob = new MockEntity(Engine);

            double expectedMobHealth = mob.GetProperty(Entity.HP_KEY).Value - 10;
            _testPlayer.Cast(mob, _unpushable.Key);
            _testPlayer.AddPushback(delay);

            MockTimer timer = (MockTimer)Engine.GetTimer();

            MeNode duration = _unpushable.ByLevel[0].Duration;
            duration = Sanitizer.ReplaceTargetAndSource(duration, _testPlayer, _testPlayer);
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
            Assert.AreEqual(expectedMobHealth, mob.GetProperty(Entity.HP_KEY).Value);
        }

        [TestMethod]
        public void CastTestInterrupt()
        {

            BaseEntity mob = new MockEntity(Engine);


            double expectedMobHealth = mob.GetProperty(Entity.HP_KEY).Value;
            _testPlayer.Cast(mob, _testSkill.Key);
            _testPlayer.InterruptCasting();

            MockTimer timer = (MockTimer)Engine.GetTimer();

            MeNode duration = _unpushable.ByLevel[0].Duration;
            duration = Sanitizer.ReplaceTargetAndSource(duration, _testPlayer, _testPlayer);
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
            Assert.AreEqual(expectedMobHealth, mob.GetProperty(Entity.HP_KEY).Value);
        }


        [TestMethod]
        public void CastTestNonInterrupt()
        {

            BaseEntity mob = new MockEntity(Engine);


            double expectedMobHealth = mob.GetProperty(Entity.HP_KEY).Value-10;
            _testPlayer.Cast(mob, _unpushable.Key);
            _testPlayer.InterruptCasting();

            MockTimer timer = (MockTimer)Engine.GetTimer();

            MeNode duration = _unpushable.ByLevel[0].Duration;
            duration = Sanitizer.ReplaceTargetAndSource(duration, _testPlayer, _testPlayer);
            long skillDuration = duration.Resolve().Value.ToLong();

            for (int i = 0; i < skillDuration; ++i)
            {
                timer.ForceTick();
            }

            timer.ForceTick();
            _testPlayer.Update();
            Assert.AreEqual(expectedMobHealth, mob.GetProperty(Entity.HP_KEY).Value);
        }

        [TestMethod]
        public void CastTestResourceEnoughMana()
        {

            BaseEntity mob = new MockEntity(Engine);
            double expectedMobHealth = mob.GetProperty(Entity.HP_KEY).Value - 10;
            _testPlayer.Cast(mob, _costly.Key);
            Assert.AreEqual(0, _testPlayer.ResourceMap["MP"].CurrentAmount);
            MockTimer timer = (MockTimer)Engine.GetTimer();
            timer.ForceTick();
            _testPlayer.Update();
            Assert.AreEqual(expectedMobHealth, mob.GetProperty(Entity.HP_KEY).Value);
            

        }

        [TestMethod]
        public void CastTestResourceNotEnoughMana()
        {

            BaseEntity mob = new MockEntity(Engine);
            double expectedMobHealth = mob.GetProperty(Entity.HP_KEY).Value - 10;
            _testPlayer.Cast(mob, _costly.Key);
            Assert.AreEqual(0, _testPlayer.ResourceMap["MP"].CurrentAmount);

            MockTimer timer = (MockTimer)Engine.GetTimer();
            timer.ForceTick();
            _testPlayer.Update();
            mob.Update();
            Assert.AreEqual(expectedMobHealth, mob.GetProperty(Entity.HP_KEY).Value);
          

            bool secondRes = _testPlayer.Cast(mob, _costly.Key);

            Assert.AreEqual(false, secondRes);
        }
    }
}
