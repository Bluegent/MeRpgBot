using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroExpressionParser;
namespace ParserTest
{
    [TestClass]
    public class CompoundStatTest
    {
        [TestMethod]
        public void FromJsonTest_Succes()
        {
            string json = "{\"key\":\"DEF\",\"formula\":\"B_DEF\"}";
            CompoundStat expected = new CompoundStat() { Key = "DEF", Formula = "B_DEF" };
            CompoundStat result = CompoundStat.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.Formula, result.Formula);
        }

        [TestMethod]
        public void FromJsonTest_IncompleteJson_NoFormula()
        {
            string json = "{\"key\":\"DEF\"}";
            CompoundStat expected = new CompoundStat() { Key = "DEF", Formula = null };
            CompoundStat result = CompoundStat.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.Formula, result.Formula);
        }

        [TestMethod]
        public void FromJsonTest_IncompleteJson_NoKey()
        {
            string json = "{\"formula\":\"B_DEF\"}";
            CompoundStat expected = new CompoundStat() { Key =null, Formula = "B_DEF" };
            CompoundStat result = CompoundStat.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.Formula, result.Formula);
        }

        [TestMethod]
        public void FromJsonTest_EmptyJson()
        {
            string json = "";
            CompoundStat expected = null;
            CompoundStat result = CompoundStat.FromJson(json);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FromJsonTest_NullJson()
        {
            string json = null;
            CompoundStat expected = null;
            CompoundStat result = CompoundStat.FromJson(json);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FromJsonTest_InvalidJson()
        {
            string json = "Invalid";
            CompoundStat expected = null;
            CompoundStat result = CompoundStat.FromJson(json);
            Assert.AreEqual(expected, result);

        }
    }
}
