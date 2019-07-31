using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroExpressionParser;

namespace ParserTest
{
    [TestClass]
    public class StatusEffectTest
    {
        [TestMethod]
        public void FromJsonTest_Succes()
        {
            string json = "{\"key\":\"test\", \"name\":\"Test\", \"description\":\"Description\", \"max_stack\":10, \"formula\":\"STR\"}";
            StatusEffect expected = new StatusEffect() { Key = "test", Name = "Test", Description = "Description", Formula = "STR", MaxStack = 10 };
            StatusEffect result = StatusEffect.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.Name, result.Name);
            Assert.AreEqual(expected.Description, result.Description);
            Assert.AreEqual(expected.Formula, result.Formula);
            Assert.AreEqual(expected.MaxStack, result.MaxStack);
        }

        [TestMethod]
        public void FromJsonTest_IncompleteJson_NoKey()
        {
            string json = "{\"name\":\"Test\", \"description\":\"Description\", \"max_stack\":10, \"formula\":\"STR\"}";
            StatusEffect expected = new StatusEffect() { Key = null, Name = "Test", Description = "Description", Formula = "STR", MaxStack = 10 };
            StatusEffect result = StatusEffect.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.Name, result.Name);
            Assert.AreEqual(expected.Description, result.Description);
            Assert.AreEqual(expected.Formula, result.Formula);
            Assert.AreEqual(expected.MaxStack, result.MaxStack);
        }

        [TestMethod]
        public void FromJsonTest_IncompleteJson_NoName()
        {
            string json = "{\"key\":\"test\", \"description\":\"Description\", \"max_stack\":10, \"formula\":\"STR\"}";
            StatusEffect expected = new StatusEffect() { Key = "test", Name = null, Description = "Description", Formula = "STR", MaxStack = 10 };
            StatusEffect result = StatusEffect.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.Name, result.Name);
            Assert.AreEqual(expected.Description, result.Description);
            Assert.AreEqual(expected.Formula, result.Formula);
            Assert.AreEqual(expected.MaxStack, result.MaxStack);
        }

        [TestMethod]
        public void FromJsonTest_IncompleteJson_NoDescription()
        {
            string json = "{\"key\":\"test\", \"name\":\"Test\", \"max_stack\":10, \"formula\":\"STR\"}";
            StatusEffect expected = new StatusEffect() { Key = "test", Name = "Test", Description = null, Formula = "STR", MaxStack = 10 };
            StatusEffect result = StatusEffect.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.Name, result.Name);
            Assert.AreEqual(expected.Description, result.Description);
            Assert.AreEqual(expected.Formula, result.Formula);
            Assert.AreEqual(expected.MaxStack, result.MaxStack);
        }

        [TestMethod]
        public void FromJsonTest_IncompleteJson_NoFormula()
        {
            string json = "{\"key\":\"test\", \"name\":\"Test\", \"description\":\"Description\", \"max_stack\":10}";
            StatusEffect expected = new StatusEffect() { Key = "test", Name = "Test", Description = "Description", Formula = null, MaxStack = 10 };
            StatusEffect result = StatusEffect.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.Name, result.Name);
            Assert.AreEqual(expected.Description, result.Description);
            Assert.AreEqual(expected.Formula, result.Formula);
            Assert.AreEqual(expected.MaxStack, result.MaxStack);
        }

        [TestMethod]
        public void FromJsonTest_IncompleteJson_NoMaxStack()
        {
            string json = "{\"key\":\"test\", \"name\":\"Test\", \"description\":\"Description\", \"formula\":\"STR\"}";
            StatusEffect expected = new StatusEffect() { Key = "test", Name = "Test", Description = "Description", Formula = "STR", MaxStack = 0 };
            StatusEffect result = StatusEffect.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.Name, result.Name);
            Assert.AreEqual(expected.Description, result.Description);
            Assert.AreEqual(expected.Formula, result.Formula);
            Assert.AreEqual(expected.MaxStack, result.MaxStack);
        }

        [TestMethod]
        public void FromJsonTest_EmptyJson()
        {
            string json = "";
            StatusEffect expected = null;
            StatusEffect result = StatusEffect.FromJson(json);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FromJsonTest_NullJson()
        {
            string json = null;
            StatusEffect expected = null;
            StatusEffect result = StatusEffect.FromJson(json);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FromJsonTest_InvalidJson()
        {
            string json = "Invalid";
            StatusEffect expected = null;
            StatusEffect result = StatusEffect.FromJson(json);
            Assert.AreEqual(expected, result);

        }
    }
}
