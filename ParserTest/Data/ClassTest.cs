using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroExpressionParser;

namespace ParserTest
{
    [TestClass]
    public class ClassTest
    {
        [TestMethod]
        public void FromJsonTest_Succes()
        {
            string json = "{\"StatKey\":\"test\", \"base_values\": [{\"StatKey\":\"B_DMG\", \"value\":10}], \"basic_attributes\":[{\"StatKey\":\"STR\",\"value\":5}], \"skills\":[\"test\"]}";
            Class expected = new Class() { Key = "test", BaseValues = new List<BaseValue>() { new BaseValue() { Key = "B_DMG", Value = 10 } }, BasicAttributes = new List<BaseValue>() { new BaseValue() { Key = "STR", Value = 5 } }, Skills =new List<string>() { "test"} };
            Class result = Class.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
        }

        [TestMethod]
        public void FromJsonTest_EmptyJson()
        {
            string json = "";
            Class expected = null;
            Class result = Class.FromJson(json);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FromJsonTest_NullJson()
        {
            string json = null;
            Class expected = null;
            Class result = Class.FromJson(json);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FromJsonTest_InvalidJson()
        {
            string json = "Invalid";
            Class expected = null;
            Class result = Class.FromJson(json);
            Assert.AreEqual(expected, result);

        }
    }
}
