using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using RPGEngine.Core;
using RPGEngine.Game;
using RPGEngine.Parser;
using RPGEngine.Utils;

namespace RPGEngine.GameConfigReader
{
    using RPGEngine.Entities;

    public class SkillReader
    {

        public IGameEngine Engine { get; }

        public SkillReader(IGameEngine engine)
        {
            Engine = engine;
        }


        private SkillCost CostFromJson(JToken json, SkillTemplate skill)
        {
            string key;
            string amountFormula;
            if (json == null)
            {
                key = Entity.HP_KEY;
                amountFormula = GcConstants.Skills.DEFAULT_COST_VALUE;
            }
            else
            {
                JObject costObj = json.ToObject<JObject>();

                key = JsonUtils.ValidateJsonEntry(GcConstants.General.KEY,
                    costObj,
                    JTokenType.String,
                    $"No cost key for skill {skill.Name}.")
                    .ToString();
                if (!Engine.GetPropertyManager().HasResource(key))
                {
                    throw new MeException($"Unknown resource {key} used by skill {skill.Name}.");
                }

                amountFormula = JsonUtils.ValidateJsonEntry(GcConstants.General.VALUE,
                        costObj,
                        JTokenType.String,
                        $"No cost amount for skill {skill.Name}.")
                    .ToString();
            }

            return new SkillCost(key, TreeConverter.Build(amountFormula, Engine));
        }

        private SkillLevelTemplate LevelFromJson(JObject levelValue, SkillTemplate skill)
        {
            SkillLevelTemplate levelTemplate = new SkillLevelTemplate();

            //get cost
            levelTemplate.Cost = CostFromJson(levelValue[GcConstants.Skills.COST], skill);

            //get needed level, push-back and interrupt because they have default values
            levelTemplate.NeededLevel = JsonUtils.GetValueOrDefault<long>(levelValue, GcConstants.Skills.NEEDED_LEVEL, GcConstants.Skills.DEFAULT_NEEDED_LEVEL);
            levelTemplate.PushBack = new MeNode(JsonUtils.GetValueOrDefault(levelValue, GcConstants.Skills.PUSH_BACK, GcConstants.Skills.DEFAULT_PUSHBACK));
            levelTemplate.Interruptible = new MeNode(JsonUtils.GetValueOrDefault(levelValue, GcConstants.Skills.INTERRUPT, GcConstants.Skills.DEFAULT_INTERRUPT));

            //get cast duration
            string durationFormula = JsonUtils.GetValueOrDefault(levelValue,
                GcConstants.Skills.CAST_DURATION,
                GcConstants.Skills.DEFAULT_CAST_DURATION);
            levelTemplate.Duration = TreeConverter.Build(durationFormula, Engine);

            //get cooldown
            string cdFormula = JsonUtils.GetValueOrDefault(
                levelValue,
                GcConstants.Skills.COOLDOWN,
                GcConstants.Skills.DEFAULT_COOLDOWN);
            levelTemplate.Cooldown = TreeConverter.Build(cdFormula, Engine);

            //get skill threat
            string threatFormula = JsonUtils.GetValueOrDefault<string>(levelValue, GcConstants.Skills.THREAT, null);
            if (threatFormula == null)
            {
                threatFormula = Engine.GetDefaultSkillThreat().ToString();
            }
            levelTemplate.SkillThreat = TreeConverter.Build(threatFormula, Engine);

            //get formulas
            string formulas = JsonUtils.ValidateJsonEntry(GcConstants.General.FORMULA,
                levelValue,
                JTokenType.String,
                $"Missing formula for skill {skill.Name}.").ToString();
            levelTemplate.Formulas.AddRange(Engine.GetSanitizer().SplitAndConvert(formulas));


            //get interval
            if (skill.Type == SkillType.Channel)
            {
                string intervalFormula = JsonUtils.GetValueOrDefault(
                    levelValue,
                    GcConstants.Skills.INTERVAL,
                    GcConstants.Skills.DEFAULT_INTERVAL_VALUE);
                levelTemplate.Interval = TreeConverter.Build(intervalFormula, Engine);
            }

            return levelTemplate;

        }

        public SkillTemplate FromJson(JObject json)
        {
            //get our base values
            SkillTemplate result = new SkillTemplate();
            result.LoadBase(json);

            //get the aliases
            JToken alias = json[GcConstants.Skills.ALIASES];
            if (alias == null)
            {
                result.Aliases.Add(result.Key);
            }
            else
            {
                JToken[] aliases = alias.ToArray();
                foreach (JToken tok in aliases)
                {
                    result.Aliases.Add(tok.ToString());
                }
            }

            //get skill type
            string type = JsonUtils.GetValueOrDefault(
                json,
                GcConstants.Skills.SKILL_TYPE,
                GcConstants.Skills.DEFAULT_TYPE);
            result.TypeFromString(type);

            //start reading the values by skill level
            JToken[] values = JsonUtils.ValidateJsonEntry(GcConstants.Skills.VALUES_BY_LEVEL,
                    json,
                    JTokenType.Array,
                    $"Missing \"{GcConstants.Skills.VALUES_BY_LEVEL}\" or wrong format entry for skill {result.Name}")
                .ToArray();

            if (values.Length == 0)
            {
                throw new MeException($"Skill {result.Name} has no level values.");
            }
            foreach (JToken jToken in values)
            {
                //validate sub object
                if (jToken.Type != JTokenType.Object)
                    throw new MeException($"Invalid entry {jToken.ToString()} when parsing skill {result.Name}");
                JObject levelValue = (JObject)jToken;

                result.ByLevel.Add(LevelFromJson(levelValue, result));
            }
            return result;
        }

    }
}

/*
        "key":"basic_heal",
        "name":"Inspiration",
        "aliases":["heal","mend"],
        "description":"Remember the words of your mentor and feel better.",   
        "type":"cast",
        "values_by_level":
        [
            {
                "cooldown":"300",
                "duration":"10",
                "push_back":"true",
                "interrupt":"true",
                "formula":"HEAL($T,$S,10+GET_PROP($S,INT)*5)",
                "needed_level":5,
                "cost":{"key":"MP","value":"10"},
				"treat":"5"
            },
*/
