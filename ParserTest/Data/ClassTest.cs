using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPGEngine;

namespace ParserTest
{
    [TestClass]
    public class ClassTest
    {
        [TestMethod]
        public void FromJsonTest_Succes()
        {
            string json = "{\"StatKey\":\"test\", \"base_values\": [{\"StatKey\":\"B_DMG\", \"value\":10}], \"basic_attributes\":[{\"StatKey\":\"STR\",\"value\":5}], \"skills\":[\"test\"]}";
            ClassJson expected = new ClassJson() { Key = "test", BaseValues = new List<BaseValue>() { new BaseValue() { Key = "B_DMG", Value = 10 } }, BasicAttributes = new List<BaseValue>() { new BaseValue() { Key = "STR", Value = 5 } }, Skills =new List<string>() { "test"} };
            ClassJson result = ClassJson.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
        }

        [TestMethod]
        public void FromJsonTest_EmptyJson()
        {
            string json = "";
            ClassJson expected = null;
            ClassJson result = ClassJson.FromJson(json);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FromJsonTest_NullJson()
        {
            string json = null;
            ClassJson expected = null;
            ClassJson result = ClassJson.FromJson(json);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FromJsonTest_InvalidJson()
        {
            string json = "Invalid";
            ClassJson expected = null;
            ClassJson result = ClassJson.FromJson(json);
            Assert.AreEqual(expected, result);

        }
    }
}
