
using Microsoft.VisualStudio.TestTools.UnitTesting;


using RPGEngine.Game;
using RPGEngine.GameInterface;
using RPGEngine.Core;
using RPGEngine.Language;
using RPGEngine.Parser;
using RPGEngine.Entities;
namespace EngineTest.Core
{
  

    [TestClass]
    public class EntityTest
    {
        public static readonly GameEngine Engine = new GameEngine(new DiscordLogHelper(new ConsoleLogger()));
        public static readonly BaseEntity BaseEntity = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };
        
        [ClassInitialize]
        public static void StartUp(TestContext context)
        {
            DamageType trueDamage = new DamageType(Engine, "T", null, null, null, null);
            trueDamage.Name = "true";
            DamageType alwaysCrit = new DamageType(Engine, "AC", null, null, "100", "2");
            alwaysCrit.Name = "always_crit";
            DamageType alwaysDodge = new DamageType(Engine, "AD", null, "100", null, null);
            alwaysDodge.Name = "always_dodge";
            DamageType physical = new DamageType(Engine,"P", $"{LConstants.NON_NEG_F}({LConstants.ValueKeyword}-{LConstants.TargetKeyword}{LConstants.PROP_OP}DEF)",null,null,null);
            physical.Name = "physical";
            Engine.AddDamageType(trueDamage);
            Engine.AddDamageType(alwaysCrit);
            Engine.AddDamageType(alwaysDodge);
            Engine.AddDamageType(physical);
            Engine.AddPlayer(BaseEntity);
        }
        [TestMethod]
        public void EntityTestModifierStatusEffect()
        {
            BaseEntity ent = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };
            string expression = $"{LConstants.ADD_MOD_F}(STR,$0)";
            MeNode[] statuses = Engine.GetSanitizer().SplitAndConvert(expression);
            StatusTemplate test = new StatusTemplate(statuses) { Interval = TreeConverter.Build("0", Engine)};
            double[] values = { 10 };
            ent.ApplyStatus(test, ent,5,values);
            double expected = ent.GetProperty("STR").Value+10;
            ent.Update();
            Assert.AreEqual(expected, ent.GetProperty("STR").Value);
        }

        [TestMethod]
        public void EntityTestModifierStatusEffectsRemoved()
        {
            BaseEntity ent = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };
            string expression = $"{LConstants.ADD_MOD_F}(STR,$0)";
            MeNode[] statuses = Engine.GetSanitizer().SplitAndConvert(expression);
            StatusTemplate test = new StatusTemplate(statuses) { Interval = TreeConverter.Build("0", Engine)};
            double[] values = { 10 };
            int duration = 5;
            ent.ApplyStatus(test, ent, duration, values);
            double expected = ent.GetProperty("STR").Value + 10;
            double removedExpected = ent.GetProperty("STR").Value;
            MockTimer timer = (MockTimer)Engine.GetTimer();
            ent.Update();
            Assert.AreEqual(expected, ent.GetProperty("STR").Value);
            for(int i=0;i<= duration; ++i)
                timer.ForceTick();
            ent.Update();
            Assert.AreEqual(removedExpected, ent.GetProperty("STR").Value);
        }

        [TestMethod]
        public void EntityTestModifierStatusEffectsMultiple()
        {
            BaseEntity ent = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };
            string expression = $"{LConstants.ADD_MOD_F}(STR,$0)";
            MeNode[] statuses = Engine.GetSanitizer().SplitAndConvert(expression);
            int duration = 5;
            StatusTemplate test = new StatusTemplate(statuses) { Interval = TreeConverter.Build("0", Engine),Type = StackingType.Independent,MaxStacks =  TreeConverter.Build("0",Engine)};
            test.Key = "shonen_powerup";
            double[] values = { 10 };
            ent.ApplyStatus(test, ent, duration, values);
            ent.ApplyStatus(test, ent, duration, values);
            double expected = ent.GetProperty("STR").Value + values[0]*2;
            double removedExpected = ent.GetProperty("STR").Value;
            MockTimer timer = (MockTimer)Engine.GetTimer();
            ent.Update();
            Assert.AreEqual(expected, ent.GetProperty("STR").Value);
            for (int i = 0; i <= duration; ++i)
                timer.ForceTick();
            ent.Update();
            Assert.AreEqual(removedExpected, ent.GetProperty("STR").Value);
        }

        [TestMethod]
        public void EntityTestModifierMultipleStatusEffects()
        {
            BaseEntity ent = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };
            string expression = $"{LConstants.ADD_MOD_F}(STR,$0);{LConstants.ADD_MOD_F}(AGI,$1)";
            MeNode[] statuses = Engine.GetSanitizer().SplitAndConvert(expression);

            StatusTemplate test = new StatusTemplate(statuses){Interval = TreeConverter.Build("0", Engine)};
            double[] values = { 10 ,5};
            ent.ApplyStatus(test, ent, 5, values);
            double expectedStr = ent.GetProperty("STR").Value + values[0];
            double expetedDex = ent.GetProperty("AGI").Value + values[1];
            MockTimer timer = (MockTimer)Engine.GetTimer();
            ent.Update();
            Assert.AreEqual(expectedStr, ent.GetProperty("STR").Value);
            Assert.AreEqual(expetedDex, ent.GetProperty("AGI").Value);
        }

        [TestMethod]
        public void EntityTestHarmStatusEffect()
        {
            BaseEntity ent = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };
            double damage = 10;
            string expression = $"{LConstants.HARM_F}({LConstants.TargetKeyword},{LConstants.TargetKeyword},T,{damage})";
            MeNode[] statuses = Engine.GetSanitizer().SplitAndConvert(expression);
            StatusTemplate test = new StatusTemplate(statuses){Interval = TreeConverter.Build("0", Engine)};
            ent.ApplyStatus(test,ent,5,null);
            double expectedHp = ent.GetProperty(Entity.HP_KEY).Value - damage;
            ent.Update();
            Assert.AreEqual(expectedHp,ent.GetProperty(Entity.HP_KEY).Value);
        }

        [TestMethod]
        public void EntityTestModifierAndHarm()
        {
            BaseEntity ent = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };
            string expression = $"{LConstants.HARM_F}({LConstants.TargetKeyword},{LConstants.TargetKeyword},T,$0);{LConstants.ADD_MOD_F}(STR,$1)";
            MeNode[] statuses = Engine.GetSanitizer().SplitAndConvert(expression);
            StatusTemplate test = new StatusTemplate(statuses) { Interval = TreeConverter.Build("0", Engine)};
            double[] values = { 20, 10 };
            ent.ApplyStatus(test, ent, 5, values);
            double expectedHp = ent.GetProperty(Entity.HP_KEY).Value - values[0];
            double expected = ent.GetProperty("STR").Value + values[1];

            ent.Update();

            Assert.AreEqual(expectedHp, ent.GetProperty(Entity.HP_KEY).Value);
            Assert.AreEqual(expected, ent.GetProperty("STR").Value);
        }

        [TestMethod]
        public void EntityTestModifierHarmTickrate()
        {
            BaseEntity ent = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };
            int[] timeValues = { 10, 5 };
            string expression = $"{LConstants.HARM_F}({LConstants.TargetKeyword},{LConstants.TargetKeyword},T,$0)";
            MeNode[] statuses = Engine.GetSanitizer().SplitAndConvert(expression);
            StatusTemplate test = new StatusTemplate(statuses){ Interval = TreeConverter.Build(timeValues[0].ToString(), Engine) };
            double[] values = { 20};
            ent.ApplyStatus(test, ent, timeValues[0], values);
            double expectedHp = ent.GetProperty(Entity.HP_KEY).Value - values[0];
            MockTimer timer = (MockTimer)Engine.GetTimer();
            ent.Update();
            Assert.AreEqual(expectedHp, ent.GetProperty(Entity.HP_KEY).Value);

            timer.ForceTick();
            timer.ForceTick();
            ent.Update();
            
            Assert.AreEqual(expectedHp, ent.GetProperty(Entity.HP_KEY).Value);
        }

        [TestMethod]
        public void EntityTestHarmStatusIntervalIsFormula()
        {
            BaseEntity ent = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };
            double damage = 10;
            string expression = $"{LConstants.HARM_F}({LConstants.TargetKeyword},{LConstants.TargetKeyword},T,{damage})";
            MeNode[] statuses = Engine.GetSanitizer().SplitAndConvert(expression);
            string intervalExpression = $"10-{LConstants.GET_PROP_F}({LConstants.SourceKeyword},INT)*2";
            MeNode intervalNode = TreeConverter.Build(intervalExpression, Engine);
            StatusTemplate test = new StatusTemplate(statuses){ Interval = intervalNode };
            ent.ApplyStatus(test, ent, 5, null);
            double expectedHp = ent.GetProperty(Entity.HP_KEY).Value - damage;
            double expectedHp2 = ent.GetProperty(Entity.HP_KEY).Value - damage*2;
            MockTimer timer = (MockTimer)Engine.GetTimer();

            ent.Update();
            timer.ForceTick();
            Assert.AreEqual(expectedHp, ent.GetProperty(Entity.HP_KEY).Value);

            ent.Update();
            Assert.AreEqual(expectedHp2, ent.GetProperty(Entity.HP_KEY).Value);
        }

        [TestMethod]
        public void EntityTestAlwaysDodge()
        {
            string expression = $"{LConstants.HARM_F}({BaseEntity.Key},{BaseEntity.Key},AD,20)";
            double expected = BaseEntity.GetProperty(Entity.HP_KEY).Value;
            TreeResolver.Resolve(expression, Engine);

            Assert.AreEqual(expected, BaseEntity.GetProperty(Entity.HP_KEY).Value);
        }

        [TestMethod]
        public void EntityTestAlwaysCrit()
        {

            double amt = 20;
            string expression = $"{LConstants.HARM_F}({BaseEntity.Key},{BaseEntity.Key},AC,{amt})";
            double expected = BaseEntity.GetProperty(Entity.HP_KEY).Value - amt*2;
            TreeResolver.Resolve(expression, Engine);

            Assert.AreEqual(expected, BaseEntity.GetProperty(Entity.HP_KEY).Value);
        }

        [TestMethod]
        public void EntityTestDamageReduction()
        {

            double amt = 20;
            string expression = $"{LConstants.HARM_F}({BaseEntity.Key},{BaseEntity.Key},T,{amt});{LConstants.HARM_F}({BaseEntity.Key},{BaseEntity.Key},P,{amt});";
            double physAmt = amt - BaseEntity.GetProperty("DEF").Value;
            physAmt = physAmt < 0 ? 0 : physAmt;
            double expected = BaseEntity.GetProperty(Entity.HP_KEY).Value - amt - physAmt;
            MeNode[] trees = Engine.GetSanitizer().SplitAndConvert(expression);
            foreach (MeNode node in trees)
                node.Resolve();
            Assert.AreEqual(expected, BaseEntity.GetProperty(Entity.HP_KEY).Value);
        }

        [TestMethod]
        public void EntityTestPropertyTypeUpdatesWithOperator()
        {
            //initial test
            string strKey = "STR";
            string expression = $"{BaseEntity.Key}{LConstants.PROP_OP}{strKey}";
            double expected = BaseEntity.GetProperty(strKey).Value;
            MeNode tree = TreeConverter.Build(expression, Engine);
            MeNode partiallyResolved = TreeResolver.ResolveGivenOperations(tree, new string[1] { LConstants.PROP_OP });
            Assert.AreEqual(expected, partiallyResolved.Value.ToDouble());

            //apply a status that modifies the value
            string expression2 = $"{LConstants.ADD_MOD_F}(STR,$0)";
            MeNode[] statuses = Engine.GetSanitizer().SplitAndConvert(expression2);
            StatusTemplate test = new StatusTemplate(statuses){ Interval = TreeConverter.Build("0", Engine) };
            double[] values = { 10 };

            BaseEntity.ApplyStatus(test, BaseEntity, 10, values);
            BaseEntity.Update();

            //test again
            expected = BaseEntity.GetProperty(strKey).Value;
            Assert.AreEqual(expected, partiallyResolved.Value.ToDouble());
            BaseEntity.Cleanse();

        }

        [TestMethod]
        public void EntityTestPropertyTypeUpdatesWithFunction()
        {
            //initial test
            string strKey = "STR";
            string expression = $"{LConstants.GET_PROP_F}({BaseEntity.Key},{strKey})";
            double expected = BaseEntity.GetProperty(strKey).Value;
            MeNode tree = TreeConverter.Build(expression, Engine);
            MeNode partiallyResolved = TreeResolver.ResolveGivenOperations(tree, new string[1] { LConstants.GET_PROP_F });
            Assert.AreEqual(expected, partiallyResolved.Value.ToDouble());

            //apply a status that modifies the value
            string expression2 = $"{LConstants.ADD_MOD_F}(STR,$0)";
            MeNode[] statuses = Engine.GetSanitizer().SplitAndConvert(expression2);
            StatusTemplate test = new StatusTemplate(statuses){ Interval = TreeConverter.Build("0", Engine) };
            double[] values = { 10 };

            BaseEntity.ApplyStatus(test, BaseEntity, 10, values);
            BaseEntity.Update();

            //test again
            expected = BaseEntity.GetProperty(strKey).Value;
            Assert.AreEqual(expected, partiallyResolved.Value.ToDouble());

            BaseEntity.Cleanse();
        }
    }
}

