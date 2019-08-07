using System;
using MicroExpressionParser;
using MicroExpressionParser.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ParserTest.Core
{
    using RPGEngine.Core;
    using RPGEngine.Language;
    using RPGEngine.Parser;

    [TestClass]
    public class EntityTest
    {
        public static readonly GameEngine Engine = new GameEngine();
        [TestMethod]
        public void EntityTestModifierStatusEffect()
        {
            MockEntity ent = new MockEntity(Engine);
            string expression = $"{Constants.MOD_VALUE_F}(STR,$0)";
            MeNode[] statuses = Engine.GetSanitizer().SplitStatus(expression);
            StatusTemplate test = new StatusTemplate() { ComponentFormulas = statuses, Interval = TreeConverter.Build("0", Engine)};
            double[] values = { 10 };
            ent.ApplyStatus(test, ent,5,values);
            double expected = ent.GetProperty("STR").Value+10;
            ent.Update();
            Assert.AreEqual(expected, ent.GetProperty("STR").Value);
        }

        [TestMethod]
        public void EntityTestModifierStatusEffectsRemoved()
        {
            MockEntity ent = new MockEntity(Engine);
            string expression = $"{Constants.MOD_VALUE_F}(STR,$0)";
            MeNode[] statuses = Engine.GetSanitizer().SplitStatus(expression);
            StatusTemplate test = new StatusTemplate() { ComponentFormulas = statuses,Interval = TreeConverter.Build("0", Engine)};
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
            MockEntity ent = new MockEntity(Engine);
            string expression = $"{Constants.MOD_VALUE_F}(STR,$0)";
            MeNode[] statuses = Engine.GetSanitizer().SplitStatus(expression);
            int duration = 5;
            StatusTemplate test = new StatusTemplate() { ComponentFormulas = statuses,Interval = TreeConverter.Build("0", Engine),Type = StackingType.Independent,MaxStacks =  TreeConverter.Build("0",Engine)};
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
            MockEntity ent = new MockEntity(Engine);
            string expression = $"{Constants.MOD_VALUE_F}(STR,$0);{Constants.MOD_VALUE_F}(AGI,$1)";
            MeNode[] statuses = Engine.GetSanitizer().SplitStatus(expression);

            StatusTemplate test = new StatusTemplate() { ComponentFormulas = statuses,Interval = TreeConverter.Build("0", Engine)};
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
            MockEntity ent = new MockEntity(Engine);
            double damage = 10;
            string expression = $"{Constants.HARM_F}({Constants.TargetKeyword},{Constants.TargetKeyword},T,{damage})";
            MeNode[] statuses = Engine.GetSanitizer().SplitStatus(expression);
            StatusTemplate test = new StatusTemplate() { ComponentFormulas = statuses,Interval = TreeConverter.Build("0", Engine)};
            ent.ApplyStatus(test,ent,5,null);
            double expectedHp = ent.GetProperty("CHP").Value - damage;
            ent.Update();
            Assert.AreEqual(expectedHp,ent.GetProperty("CHP").Value);
        }

        [TestMethod]
        public void EntityTestModifierAndHarm()
        {
            MockEntity ent = new MockEntity(Engine);
            string expression = $"{Constants.HARM_F}({Constants.TargetKeyword},{Constants.TargetKeyword},T,$0);{Constants.MOD_VALUE_F}(STR,$1)";
            MeNode[] statuses = Engine.GetSanitizer().SplitStatus(expression);
            StatusTemplate test = new StatusTemplate() { ComponentFormulas = statuses,Interval = TreeConverter.Build("0", Engine)};
            double[] values = { 20, 10 };
            ent.ApplyStatus(test, ent, 5, values);
            double expectedHp = ent.GetProperty("CHP").Value - values[0];
            double expected = ent.GetProperty("STR").Value + values[1];

            ent.Update();

            Assert.AreEqual(expectedHp, ent.GetProperty("CHP").Value);
            Assert.AreEqual(expected, ent.GetProperty("STR").Value);
        }

        [TestMethod]
        public void EntityTestModifierHarmTickrate()
        {
            MockEntity ent = new MockEntity(Engine);
            int[] timeValues = { 10, 5 };
            string expression = $"{Constants.HARM_F}({Constants.TargetKeyword},{Constants.TargetKeyword},T,$0)";
            MeNode[] statuses = Engine.GetSanitizer().SplitStatus(expression);
            StatusTemplate test = new StatusTemplate() { ComponentFormulas = statuses, Interval = TreeConverter.Build(timeValues[0].ToString(), Engine) };
            double[] values = { 20};
            ent.ApplyStatus(test, ent, timeValues[0], values);
            double expectedHp = ent.GetProperty("CHP").Value - values[0];
            MockTimer timer = (MockTimer)Engine.GetTimer();
            ent.Update();
            Assert.AreEqual(expectedHp, ent.GetProperty("CHP").Value);

            timer.ForceTick();
            timer.ForceTick();
            ent.Update();
            
            Assert.AreEqual(expectedHp, ent.GetProperty("CHP").Value);
        }

        [TestMethod]
        public void EntityTestHarmStatusIntervalIsFormula()
        {
            MockEntity ent = new MockEntity(Engine);
            double damage = 10;
            string expression = $"{Constants.HARM_F}({Constants.TargetKeyword},{Constants.TargetKeyword},T,{damage})";
            MeNode[] statuses = Engine.GetSanitizer().SplitStatus(expression);
            string intervalExpression = $"10-{Constants.GET_PROP_F}({Constants.SourceKeyword},INT)*2";
            MeNode intervalNode = TreeConverter.Build(intervalExpression, Engine);
            StatusTemplate test = new StatusTemplate() { ComponentFormulas = statuses, Interval = intervalNode };
            ent.ApplyStatus(test, ent, 5, null);
            double expectedHp = ent.GetProperty("CHP").Value - damage;
            double expectedHp2 = ent.GetProperty("CHP").Value - damage*2;
            MockTimer timer = (MockTimer)Engine.GetTimer();

            ent.Update();
            timer.ForceTick();
            Assert.AreEqual(expectedHp, ent.GetProperty("CHP").Value);

            ent.Update();
            Assert.AreEqual(expectedHp2, ent.GetProperty("CHP").Value);
        }
    }
}

