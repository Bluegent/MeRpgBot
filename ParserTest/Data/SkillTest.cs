using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroExpressionParser;
using System.Collections.Generic;

namespace ParserTest
{
    [TestClass]
    public class SkillTest
    {
        [TestMethod]
        public void FromJsonTest_Succes()
        {
            string json = "{\"StatKey\":\"test\", \"name\":\"Test Skill\", \"aliases\":[\"test\"], \"description\":\"Description\", \"type\":\"cast\", \"values_by_level\":[{\"cooldown\":\"1200\", \"duration\":\"60\",\"formula\":\"STR\", \"needed_level\":15, \"interval\":\"1\"}]}";
            Skill expected = new Skill() { Key = "test", Name = "Test Skill", Description = "Description",Aliases=new List<string>(){ "test"},Type="cast",ValuesByLevel=new List<ValueByLevel>() { new ValueByLevel() { Cooldown = "1200", Duration = "60", Formula = "STR", Interval = "1", NeededLevel = 15 } } };
            Skill result = Skill.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.Name, result.Name);
            Assert.AreEqual(expected.Description, result.Description);
            Assert.AreEqual(expected.Type, result.Type);
            Assert.AreEqual(expected.Aliases.Count, result.Aliases.Count);
            Assert.AreEqual(expected.ValuesByLevel.Count, result.ValuesByLevel.Count);
        }

        [TestMethod]
        public void FromJsonTest_EmptyJson()
        {
            string json = "";
            Skill expected = null;
            Skill result = Skill.FromJson(json);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FromJsonTest_NullJson()
        {
            string json = null;
            Skill expected = null;
            Skill result = Skill.FromJson(json);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FromJsonTest_InvalidJson()
        {
            string json = "Invalid";
            Skill expected = null;
            Skill result = Skill.FromJson(json);
            Assert.AreEqual(expected, result);

        }
    }
}
