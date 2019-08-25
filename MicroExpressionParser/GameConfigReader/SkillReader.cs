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
    public class SkillReader
    {
        public IGameEngine Engine { get; }

        public SkillReader(IGameEngine engine)
        {
            Engine = engine;
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
            string type = json.ContainsKey(GcConstants.Skills.SKILL_TYPE)
                ? json[GcConstants.Skills.SKILL_TYPE].ToString()
                : GcConstants.Skills.DEFAULT_TYPE;
            result.TypeFromString(type);

            //start reading the values by skill level


            JToken[] values = JsonUtils.ValidateJsonEntry(GcConstants.Skills.VALUES_BY_LEVEL,
                    json,
                    JTokenType.Array,
                    $"Missing \"{GcConstants.Skills.VALUES_BY_LEVEL}\" or wrong format entry for skill {result.Name}")
                .ToArray();

            foreach (JToken jToken in values)
            {
                //validate sub object
                if (jToken.Type != JTokenType.Object)
                    throw new MeException($"Invalid entry {jToken.ToString()} when parsing skill {result.Name}");
                JObject levelValue = (JObject)jToken; 
                SkillLevelTemplate levelTemplate = new SkillLevelTemplate();

                //get needed level, push-back and interrupt, because they have default values
                levelTemplate.NeededLevel = new MeNode(JsonUtils.GetValueOrDefault<long>(levelValue, GcConstants.Skills.NEEDED_LEVEL, 1));
                levelTemplate.PushBack = new MeNode(JsonUtils.GetValueOrDefault(levelValue, GcConstants.Skills.PUSH_BACK, true));
                levelTemplate.Interruptible = new MeNode(JsonUtils.GetValueOrDefault(levelValue, GcConstants.Skills.INTERRUPT, true));
       
                //get skill threat
                string threatFormula = JsonUtils.GetValueOrDefault<string>(levelValue, GcConstants.Skills.THREAT, null);
                if (threatFormula == null)
                {
                    threatFormula = Engine.GetDefaultSkillThreat().ToString();
                }
                levelTemplate.SkillThreat = TreeConverter.Build(threatFormula,Engine);

                //get formulas
                string formulas = JsonUtils.ValidateJsonEntry(GcConstants.General.FORMULA,
                    levelValue,
                    JTokenType.String,
                    $"Missing formula for skill {result.Name}.").ToString();
                levelTemplate.Formulas.AddRange(Engine.GetSanitizer().SplitAndConvert(formulas));

                result.ByLevel.Add(levelTemplate);
            }
            return result;
        }

    }
}

/*
 *         "key":"basic_heal",
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
