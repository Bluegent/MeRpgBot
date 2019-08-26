using Microsoft.VisualStudio.TestTools.UnitTesting;

using RPGEngine.Core;
using RPGEngine.GameConfigReader;
using RPGEngine.GameInterface;
using RPGEngine.Language;

namespace EngineTest.GameConfigReader
{
    using Newtonsoft.Json.Linq;
    [TestClass]
    public class ResourceReaderTest
    {

        public static readonly GameEngine Engine = new GameEngine(new DiscordLogHelper(new ConsoleLogger()));

        public static readonly ResourceReader Reader = new ResourceReader(Engine);

        [ClassInitialize]
        public static void StartUp(TestContext ctx)
        {
            Definer.Instance().Engine = Engine;
            DamageType trueDamage = new DamageType(Engine, "T", null, null, null, null);
            Engine.AddDamageType(trueDamage);
        }

        [TestMethod]
        public void ResourceReaderTestDefaultValues()
        {
            string key = "TEST_RES";
            JObject resource = new JObject();
            resource.Add(GcConstants.General.KEY, key);
            resource.Add(GcConstants.General.FORMULA,"VIT*20");

            ResourceTemplate res = Reader.FromJson(resource);

            Assert.AreEqual(key,res.Key);
            Assert.IsNotNull(res.Formula);
            Assert.AreEqual(GcConstants.Resources.DEFAULT_INTERVAL, res.RegenInterval.Value.ToLong());

            Assert.AreEqual(GcConstants.Resources.DEFAULT_REGEN, res.RegenFormula.Value.ToDouble());
            Assert.AreEqual(GcConstants.Resources.DEFAULT_MODIFIER, res.StartMod.Value.ToDouble());

        }



        [TestMethod]
        public void ResourceReaderTestNonDefaultValues()
        {
            string key = "TEST_RES";
            JObject resource = new JObject();
            resource.Add(GcConstants.General.KEY, key);
            resource.Add(GcConstants.General.FORMULA, "VIT*20");
            long interval = 5;
            resource.Add(GcConstants.Resources.INTERVAL,interval);

            resource.Add(GcConstants.Resources.REGEN,"100/10");
            double mod = 1;
            resource.Add(GcConstants.Resources.MODIFIER,mod);
            ResourceTemplate res = Reader.FromJson(resource);

            Assert.AreEqual(key, res.Key);
            Assert.IsNotNull(res.Formula);
            Assert.AreEqual(interval, res.RegenInterval.Value.ToLong());

            Assert.AreEqual(10, res.RegenFormula.Resolve().Value.ToDouble());
            Assert.AreEqual(mod, res.StartMod.Value.ToDouble());

        }

    }
}
