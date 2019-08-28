using Microsoft.VisualStudio.TestTools.UnitTesting;

using RPGEngine.Core;
using RPGEngine.Game;
using RPGEngine.GameInterface;
using RPGEngine.Language;

namespace EngineTest.GameConfigReader
{
    using Newtonsoft.Json.Linq;

    using RPGEngine.GameConfigReader;
    using RPGEngine.Managers;

    [TestClass]
    public class CoreReaderTest
    {
        public static readonly GameEngine Engine = new GameEngine(new DiscordLogHelper(new ConsoleLogger()));

        public static readonly CoreReader Reader = new CoreReader(Engine);

        [ClassInitialize]
        public static void StartUp(TestContext ctx)
        {
            Definer.Instance().Engine = Engine;
        }

        [TestMethod]
        public void CoreReaderTestDefaultValues()
        {
            JObject json = new JObject();
            long firstExp = 1000;
            json.Add(GcConstants.Core.LEVEL_ONE_EXP, firstExp);
            string resFormula = "1000*10";
            json.Add(GcConstants.Core.REVIVE_TIME,resFormula);
            string expFormula = "$PREV * 1.1 +50*2^($LEVEL/5)";
            json.Add(GcConstants.Core.EXP_FORMULA,expFormula);
            long maxLevel = 100;
            json.Add(GcConstants.Core.MAX_LEVEL,maxLevel);

            CoreManager manager = Reader.FromJson(json);

            Assert.AreEqual(firstExp,manager.StartExp);
            Assert.AreEqual(1000*10,manager.ReviveTime.Resolve().Value.ToLong());
            Assert.IsNotNull(manager.ExpFormula);
            Assert.AreEqual(maxLevel,manager.MaxLevel);
            Assert.AreEqual(GcConstants.Core.DEFAULT_THREAT,manager.DefaultSkillThreat);
            Assert.AreEqual(GcConstants.Core.DEFAULT_ATTRIBUTE_POINTS,manager.AttributePointsPerLevel);
        }
    }
}
