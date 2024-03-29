﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RPGEngine.Core;
using RPGEngine.Language;
using RPGEngine.GameConfigReader;
using RPGEngine.GameInterface;

namespace EngineTest.GameConfigReader
{
    [TestClass]
    public class BaseObjectTest
    {
        public static readonly GameEngine Engine = new GameEngine(new DiscordLogHelper(new ConsoleLogger()));

        [ClassInitialize]
        public static void StartUp(TestContext ctx)
        {
            Definer.Instance().Engine = Engine;
        }

        [TestMethod]
        public void BaseObjectTestAlltValues()
        {
            string name = "TEST_NAME";
            string key = "TEST_KEY";
            string desc = "TEST_DESC";
            string jsonStr = $"{{\"{GcConstants.General.KEY}\":\"{key}\",\"{GcConstants.General.NAME}\":\"{name}\",\"{GcConstants.General.DESC}\":\"{desc}\"}}";

            JObject json = JObject.Parse(jsonStr);
            BaseObject obj = new BaseObject();
            obj.LoadBase(json);

            Assert.AreEqual(key,obj.Key);
            Assert.AreEqual(desc,obj.Description);
            Assert.AreEqual(name,obj.Name);
        }

        [TestMethod]
        public void BaseObjectTestDefaultValues()
        {
            string key = "TEST_KEY";
            string jsonStr = $"{{\"{GcConstants.General.KEY}\":\"{key}\"}}";

            JObject json = JObject.Parse(jsonStr);
            BaseObject obj = new BaseObject();
            obj.LoadBase(json);

            Assert.AreEqual(key, obj.Key);
            Assert.AreEqual(GcConstants.Defaults.DESC, obj.Description);
            Assert.AreEqual(key, obj.Name);
        }

    }
}
