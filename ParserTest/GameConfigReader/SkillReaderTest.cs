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
            Assert.AreEqual(GcConstants.Skills.DEFAULT_NEEDED_LEVEL,levelTemplate.NeededLevel);

            Assert.AreEqual(double.Parse(GcConstants.Skills.DEFAULT_CAST_DURATION),levelTemplate.Duration.Value.ToDouble());
            Assert.AreEqual(double.Parse(GcConstants.Skills.DEFAULT_COOLDOWN), levelTemplate.Cooldown.Value.ToDouble());

            Assert.AreEqual(Engine.GetDefaultSkillThreat(), levelTemplate.SkillThreat.Value.ToDouble());
            Assert.IsNull(levelTemplate.Interval);
            Assert.AreEqual(GcConstants.Skills.DEFAULT_INTERRUPT, levelTemplate.PushBack.Value.ToBoolean());
            Assert.AreEqual(GcConstants.Skills.DEFAULT_PUSHBACK, levelTemplate.Interruptible.Value.ToBoolean());

            Assert.AreEqual(1,levelTemplate.Formulas.Count);
            Assert.IsNotNull(levelTemplate.Formulas[0]);
        }


        [TestMethod]
        public void SkillReaderTestNonDefaultValue()
        {

            string key = "TEST_STATUS";
            string testAlias = "TEST_ALIAS";
            string testAlias2 = "TEST_ALIAS2";

            JObject json = new JObject();
            json.Add(new JProperty(GcConstants.General.KEY, key));
            json.Add( new JProperty(GcConstants.Skills.ALIASES, new JArray( new[]{testAlias,testAlias2})));
            json.Add(GcConstants.Skills.SKILL_TYPE,SkillType.Channel.ToString().ToLower());

            JArray valuesByLevel = new JArray();
            JObject valueByLevel1 = new JObject();

            valueByLevel1.Add(GcConstants.General.FORMULA,$"{LConstants.HARM_F}({LConstants.TargetKeyword},{LConstants.SourceKeyword},T,10)");
            valueByLevel1.Add(GcConstants.Skills.CAST_DURATION,"10*10");

            JObject cost = new JObject();
            cost.Add(GcConstants.General.KEY,"MP");
            cost.Add(GcConstants.General.VALUE,100);

            valueByLevel1.Add(GcConstants.Skills.COST,cost);
            long level = 10;
            valueByLevel1.Add(GcConstants.Skills.NEEDED_LEVEL, level);

            long cd = 25;
            valueByLevel1.Add(GcConstants.Skills.COOLDOWN,cd);

            long interval = 15;
            valueByLevel1.Add(GcConstants.Skills.INTERVAL,interval);

            long threat = 3;
            valueByLevel1.Add(GcConstants.Skills.THREAT,threat);

            valueByLevel1.Add(GcConstants.Skills.INTERRUPT,false);
            valueByLevel1.Add(GcConstants.Skills.PUSH_BACK, false);


            valuesByLevel.Add(valueByLevel1);
            json.Add(new JProperty(GcConstants.Skills.VALUES_BY_LEVEL,valuesByLevel));

            SkillTemplate skill = Reader.FromJson(json);

            SkillLevelTemplate levelTemplate = skill.ByLevel[0];
            Assert.AreEqual(2, skill.Aliases.Count);
            Assert.AreEqual(testAlias, skill.Aliases[0]);
            Assert.AreEqual(testAlias2, skill.Aliases[1]);


            Assert.AreEqual(skill.Key, key);

            Assert.AreEqual(skill.Type,SkillType.Channel);
            Assert.AreEqual(1,skill.ByLevel.Count);



            Assert.AreEqual(100,levelTemplate.Cost.Amount.Value.ToDouble());
            Assert.AreEqual("MP",levelTemplate.Cost.ResourceKey);

            Assert.AreEqual(100,levelTemplate.Duration.Resolve().Value.ToDouble());
            Assert.AreEqual(cd, levelTemplate.Cooldown.Value.ToDouble());
            Assert.AreEqual(level,levelTemplate.NeededLevel);

            Assert.AreEqual(threat, levelTemplate.SkillThreat.Value.ToDouble());
            Assert.AreEqual(interval,levelTemplate.Interval.Value.ToDouble());
            Assert.AreEqual(false, levelTemplate.PushBack.Value.ToBoolean());
            Assert.AreEqual(false, levelTemplate.Interruptible.Value.ToBoolean());

            Assert.AreEqual(1,levelTemplate.Formulas.Count);
            Assert.IsNotNull(levelTemplate.Formulas[0]);
        }
    }
}