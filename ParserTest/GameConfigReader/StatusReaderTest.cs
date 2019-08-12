using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RPGEngine.Core;
using RPGEngine.GameConfigReader;
using RPGEngine.Language;

namespace EngineTest.GameConfigReader
{
    [TestClass]
    public class StatusReaderTest
    {
        public static readonly GameEngine Engine = new GameEngine();
        public static readonly StatusReader Reader = new StatusReader(Engine);

        [ClassInitialize]
        public static void StartUp(TestContext ctx)
        {
            Definer.Get().Engine = Engine;
        }
        [TestMethod]
        public void StatusReaderTestAllValues()
        {

            string key = "TEST_KEY";
            double maxStack = 10;
            double interval = 10;
            StackingType type = StackingType.Refresh;  
            string intervalFormula = $"{Constants.MAX_F}(10,5)";
            string formula = $"{ Constants.HARM_F }({ Constants.TargetKeyword},{ Constants.TargetKeyword},T,$0)";

            string jsonStr = $"{{\"{GcConstants.KEY}\":\"{key}\",";
            jsonStr += $"\"{GcConstants.MAX_STACK}\":\"{maxStack}\",";
            jsonStr += $"\"{GcConstants.STACK_TYPE}\":\"{type.ToString().ToLower()}\",";
            jsonStr += $"\"{GcConstants.INTERVAL}\":\"{intervalFormula}\",";
            jsonStr += $"\"{GcConstants.FORMULA}\":\"{formula}\"}}";

            JObject json = JObject.Parse(jsonStr);
            StatusTemplate status = Reader.FromJSON(json);

            Assert.AreEqual(maxStack,status.MaxStacks.Resolve().Value.ToDouble());
            Assert.AreEqual(interval,status.Interval.Resolve().Value.ToDouble());
            Assert.AreEqual(type,status.Type);
            Assert.AreEqual(interval,status.Interval.Resolve().Value.ToDouble());

        }
    }
}
