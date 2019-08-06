using System;
using MicroExpressionParser;
using MicroExpressionParser.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ParserTest.Core
{
    [TestClass]
    public class EntityTest
    {
        public static readonly GameEngine Engine = new GameEngine();
        [TestMethod]
        public void EntityTestModifierStatusEffect()
        {
            MockEntity ent = new MockEntity(Engine);
            string expression = $"{StringConstants.MOD_VALUE_F}(STR,$0)";
            FunctionalNode[] statuses = Engine.GetSanitizer().SplitStatus(expression);
            StatusTemplate test = new StatusTemplate() { formulas = statuses, interval = 0 };
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
            string expression = $"{StringConstants.MOD_VALUE_F}(STR,$0)";
            FunctionalNode[] statuses = Engine.GetSanitizer().SplitStatus(expression);
            StatusTemplate test = new StatusTemplate() { formulas = statuses, interval = 0 };
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
            string expression = $"{StringConstants.MOD_VALUE_F}(STR,$0)";
            FunctionalNode[] statuses = Engine.GetSanitizer().SplitStatus(expression);
            int duration = 5;
            StatusTemplate test = new StatusTemplate() { formulas = statuses, interval = 0 };
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
            string expression = $"{StringConstants.MOD_VALUE_F}(STR,$0);{StringConstants.MOD_VALUE_F}(AGI,$1)";
            FunctionalNode[] statuses = Engine.GetSanitizer().SplitStatus(expression);
            StatusTemplate test = new StatusTemplate() { formulas = statuses, interval = 0 };
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
            string expression = $"{StringConstants.HARM_F}({StringConstants.TargetKeyword},{StringConstants.TargetKeyword},T,{damage})";
            FunctionalNode[] statuses = Engine.GetSanitizer().SplitStatus(expression);
            StatusTemplate test = new StatusTemplate() { formulas = statuses, interval = 0 };
            ent.ApplyStatus(test,ent,5,null);
            double expectedHp = ent.GetProperty("CHP").Value - damage;
            ent.Update();
            Assert.AreEqual(expectedHp,ent.GetProperty("CHP").Value);
        }

        [TestMethod]
        public void EntityTestModifierAndHarm()
        {
            MockEntity ent = new MockEntity(Engine);
            string expression = $"{StringConstants.HARM_F}({StringConstants.TargetKeyword},{StringConstants.TargetKeyword},T,$0);{StringConstants.MOD_VALUE_F}(STR,$1)";
            FunctionalNode[] statuses = Engine.GetSanitizer().SplitStatus(expression);
            StatusTemplate test = new StatusTemplate() { formulas = statuses, interval = 0 };
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
            string expression = $"{StringConstants.HARM_F}({StringConstants.TargetKeyword},{StringConstants.TargetKeyword},T,$0)";
            FunctionalNode[] statuses = Engine.GetSanitizer().SplitStatus(expression);
            StatusTemplate test = new StatusTemplate() { formulas = statuses, interval = timeValues[1] };
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
    }
}

