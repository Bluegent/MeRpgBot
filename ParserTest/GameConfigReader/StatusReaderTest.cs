using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RPGEngine.Core;
using RPGEngine.Game;
using RPGEngine.GameConfigReader;
using RPGEngine.Language;
using RPGEngine.GameInterface;

namespace EngineTest.GameConfigReader
{
    [TestClass]
    public class StatusReaderTest
    {
        public static readonly GameEngine Engine = new GameEngine(new DiscordLogHelper(new ConsoleLogger()));
        public static readonly StatusReader Reader = new StatusReader(Engine);

        [ClassInitialize]
        public static void StartUp(TestContext ctx)
        {
            Definer.Instance().Engine = Engine;
        }
        [TestMethod]
        public void StatusReaderTestAllValues()
        {

            string key = "TEST_KEY";
            double maxStack = 10;
            double interval = 10;
            StackingType type = StackingType.Refresh;  
            string intervalFormula = $"{LConstants.MAX_F}(10,5)";
            string formula = $"{ LConstants.HARM_F }({ LConstants.TargetKeyword},{ LConstants.TargetKeyword},T,$0)";

            string jsonStr = $"{{\"{GcConstants.General.KEY}\":\"{key}\",";
            jsonStr += $"\"{GcConstants.Statuses.MAX_STACK}\":\"{maxStack}\",";
            jsonStr += $"\"{GcConstants.Statuses.STACK_TYPE}\":\"{type.ToString().ToLower()}\",";
            jsonStr += $"\"{GcConstants.Statuses.INTERVAL}\":\"{intervalFormula}\",";
            jsonStr += $"\"{GcConstants.General.FORMULA}\":\"{formula}\"}}";

            JObject json = JObject.Parse(jsonStr);
            StatusTemplate status = Reader.FromJSON(json);

            Assert.AreEqual(maxStack,status.MaxStacks.Resolve().Value.ToDouble());
            Assert.AreEqual(interval,status.Interval.Resolve().Value.ToDouble());
            Assert.AreEqual(type,status.Type);
            Assert.AreEqual(interval,status.Interval.Resolve().Value.ToDouble());

        }
    }
}
