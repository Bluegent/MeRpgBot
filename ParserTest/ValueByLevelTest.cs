using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroExpressionParser;
namespace ParserTest
{
    [TestClass]
    public class ValueByLevelTest
    {
        [TestMethod]
        public void FromJsonTest_Succes()
        {
            string json = "{\"cooldown\":\"1200\", \"duration\":\"60\",\"formula\":\"STR\", \"needed_level\":15, \"interval\":\"1\"}";
            ValueByLevel expected = new ValueByLevel() { Cooldown = "1200", Duration = "60", Formula = "STR", Interval = "1", NeededLevel = 15 };
            ValueByLevel result = ValueByLevel.FromJson(json);
            Assert.AreEqual(expected.Cooldown,result.Cooldown);
            Assert.AreEqual(expected.Duration,result.Duration);
            Assert.AreEqual(expected.Formula,result.Formula);
            Assert.AreEqual(expected.Interval,result.Interval);
            Assert.AreEqual(expected.NeededLevel, result.NeededLevel);

        }

        [TestMethod]
        public void FromJsonTest__IncompleteJson_NoCooldown()
        {
            string json = "{\"duration\":\"60\",\"formula\":\"STR\", \"needed_level\":15, \"interval\":\"1\"}";
            ValueByLevel expected = new ValueByLevel() { Cooldown = null, Duration = "60", Formula = "STR", Interval = "1", NeededLevel = 15 };
            ValueByLevel result = ValueByLevel.FromJson(json);
            Assert.AreEqual(expected.Cooldown, result.Cooldown);
            Assert.AreEqual(expected.Duration, result.Duration);
            Assert.AreEqual(expected.Formula, result.Formula);
            Assert.AreEqual(expected.Interval, result.Interval);
            Assert.AreEqual(expected.NeededLevel, result.NeededLevel);

        }

        [TestMethod]
        public void FromJsonTest__IncompleteJson_NoDuration()
        {
            string json = "{\"cooldown\":\"1200\", \"formula\":\"STR\", \"needed_level\":15, \"interval\":\"1\"}";
            ValueByLevel expected = new ValueByLevel() { Cooldown = "1200", Duration = null, Formula = "STR", Interval = "1", NeededLevel = 15 };
            ValueByLevel result = ValueByLevel.FromJson(json);
            Assert.AreEqual(expected.Cooldown, result.Cooldown);
            Assert.AreEqual(expected.Duration, result.Duration);
            Assert.AreEqual(expected.Formula, result.Formula);
            Assert.AreEqual(expected.Interval, result.Interval);
            Assert.AreEqual(expected.NeededLevel, result.NeededLevel);

        }
        [TestMethod]
        public void FromJsonTest__IncompleteJson_NoFormula()
        {
            string json = "{\"cooldown\":\"1200\", \"duration\":\"60\", \"needed_level\":15, \"interval\":\"1\"}";
            ValueByLevel expected = new ValueByLevel() { Cooldown = "1200", Duration = "60", Formula = null, Interval = "1", NeededLevel = 15 };
            ValueByLevel result = ValueByLevel.FromJson(json);
            Assert.AreEqual(expected.Cooldown, result.Cooldown);
            Assert.AreEqual(expected.Duration, result.Duration);
            Assert.AreEqual(expected.Formula, result.Formula);
            Assert.AreEqual(expected.Interval, result.Interval);
            Assert.AreEqual(expected.NeededLevel, result.NeededLevel);

        }
        [TestMethod]
        public void FromJsonTest__IncompleteJson_NoInterval()
        {
            string json = "{\"cooldown\":\"1200\", \"duration\":\"60\",\"formula\":\"STR\", \"needed_level\":15}";
            ValueByLevel expected = new ValueByLevel() { Cooldown = "1200", Duration = "60", Formula = "STR", Interval = null, NeededLevel = 15 };
            ValueByLevel result = ValueByLevel.FromJson(json);
            Assert.AreEqual(expected.Cooldown, result.Cooldown);
            Assert.AreEqual(expected.Duration, result.Duration);
            Assert.AreEqual(expected.Formula, result.Formula);
            Assert.AreEqual(expected.Interval, result.Interval);
            Assert.AreEqual(expected.NeededLevel, result.NeededLevel);

        }
        [TestMethod]
        public void FromJsonTest__IncompleteJson_NoNeededLevel()
        {
            string json = "{\"cooldown\":\"1200\", \"duration\":\"60\",\"formula\":\"STR\", \"interval\":\"1\"}";
            ValueByLevel expected = new ValueByLevel() { Cooldown = "1200", Duration = "60", Formula = "STR", Interval = "1", NeededLevel = 0 };
            ValueByLevel result = ValueByLevel.FromJson(json);
            Assert.AreEqual(expected.Cooldown, result.Cooldown);
            Assert.AreEqual(expected.Duration, result.Duration);
            Assert.AreEqual(expected.Formula, result.Formula);
            Assert.AreEqual(expected.Interval, result.Interval);
            Assert.AreEqual(expected.NeededLevel, result.NeededLevel);

        }

        [TestMethod]
        public void FromJsonTest_EmptyJson()
        {
            string json = "";
            ValueByLevel expected = null;
            ValueByLevel result = ValueByLevel.FromJson(json);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FromJsonTest_NullJson()
        {
            string json = null;
            ValueByLevel expected = null;
            ValueByLevel result = ValueByLevel.FromJson(json);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FromJsonTest_InvalidJson()
        {
            string json = "Invalid";
            ValueByLevel expected = null;
            ValueByLevel result = ValueByLevel.FromJson(json);
            Assert.AreEqual(expected, result);

        }
    }
}
