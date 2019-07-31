using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroExpressionParser;
namespace ParserTest
{
    [TestClass]
    public class BaseValueTest
    {
        [TestMethod]
        public void FromJsonTest_Succes()
        {
            string json = "{\"key\":\"B_DMG\",\"value\":10}";
            BaseValue expected = new BaseValue() { Key = "B_DMG", Value = 10 };
            BaseValue result = BaseValue.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.Value, result.Value);
        }

        [TestMethod]
        public void FromJsonTest_IncompleteJson_NoValue()
        {
            string json = "{\"key\":\"B_DMG\"}";
            BaseValue expected = new BaseValue() { Key = "B_DMG", Value = 0 };
            BaseValue result = BaseValue.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.Value, result.Value);
        }

        [TestMethod]
        public void FromJsonTest_IncompleteJson_NoKey()
        {
            string json = "{\"value\":10}";
            BaseValue expected = new BaseValue() { Key = null, Value = 10 };
            BaseValue result = BaseValue.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.Value, result.Value);
        }

        [TestMethod]
        public void FromJsonTest_EmptyJson()
        {
            string json = "";
            BaseValue expected = null;
            BaseValue result = BaseValue.FromJson(json);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FromJsonTest_NullJson()
        {
            string json = null;
            BaseValue expected = null;
            BaseValue result = BaseValue.FromJson(json);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FromJsonTest_InvalidJson()
        {
            string json = "Invalid";
            BaseValue expected = null;
            BaseValue result = BaseValue.FromJson(json);
            Assert.AreEqual(expected, result);

        }
    }
}
