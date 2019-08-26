using Microsoft.VisualStudio.TestTools.UnitTesting;

using RPGEngine.Core;
using RPGEngine.GameConfigReader;
using RPGEngine.GameInterface;
using RPGEngine.Language;

namespace EngineTest.GameConfigReader
{
    using System;

    using Newtonsoft.Json.Linq;

    using RPGEngine;
    using RPGEngine.Entities;
    using RPGEngine.Game;

    [TestClass]
    public class SkillReaderTest
    {
        public static readonly GameEngine Engine = new GameEngine(new DiscordLogHelper(new ConsoleLogger()));

        public static readonly SkillReader Reader = new SkillReader(Engine);

        [ClassInitialize]
        public static void StartUp(TestContext ctx)
        {
            Definer.Instance().Engine = Engine;
            DamageType trueDamage = new DamageType(Engine, "T",null,null,null,null);
            Engine.AddDamageType(trueDamage);
        }

        [TestMethod]
        public void SkillReaderTestDefaultValues()
        {

            string key = "TEST_STATUS";
            string jsonStr = $"{{\"{GcConstants.General.KEY}\":\"{key}\",";
            jsonStr += $"\"{GcConstants.Skills.VALUES_BY_LEVEL}\":[";
            jsonStr += "{";
            jsonStr += $"\"{GcConstants.General.FORMULA}\":";
            jsonStr += $"\"{LConstants.HARM_F}({LConstants.TargetKeyword},{LConstants.SourceKeyword},T,10)\"";
            jsonStr += "}";
            jsonStr += "]}";

            JObject json = JObject.Parse(jsonStr);

            SkillTemplate skill = Reader.FromJson(json);

            Assert.AreEqual(skill.Key, key);
            Assert.AreEqual(1,skill.Aliases.Count);
            Assert.AreEqual(key,skill.Aliases[0]);
            
            Assert.AreEqual(skill.Type,SkillType.Cast);
            Assert.AreEqual(1,skill.ByLevel.Count);


            SkillLevelTemplate levelTemplate = skill.ByLevel[0];
            Assert.AreEqual(double.Parse(GcConstants.Skills.DEFAULT_COST_VALUE),levelTemplate.Cost.Amount.Value.ToDouble());
            Assert.AreEqual(Entity.HP_KEY,levelTemplate.Cost.ResourceKey);

            Assert.AreEqual(double.Parse(GcConstants.Skills.DEFAULT_CAST_DURATION),levelTemplate.Duration.Value.ToDouble());
            Assert.AreEqual(double.Parse(GcConstants.Skills.DEFAULT_COOLDOWN), levelTemplate.Cooldown.Value.ToDouble());

            Assert.AreEqual(Engine.GetDefaultSkillThreat(), levelTemplate.SkillThreat.Value.ToDouble());
            Assert.IsNull(levelTemplate.Interval);
            Assert.AreEqual(GcConstants.Skills.DEFAULT_INTERRUPT, levelTemplate.PushBack.Value.ToBoolean());
            Assert.AreEqual(GcConstants.Skills.DEFAULT_PUSHBACK, levelTemplate.Interruptible.Value.ToBoolean());

            Assert.AreEqual(1,levelTemplate.Formulas.Count);
            Assert.IsNotNull(levelTemplate.Formulas[0]);
        }
    }
}