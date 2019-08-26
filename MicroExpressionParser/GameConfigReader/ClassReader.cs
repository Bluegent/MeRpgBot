namespace RPGEngine.GameConfigReader
{
    using System;
    using System.Linq;

    using Newtonsoft.Json.Linq;

    using RPGEngine.Core;
    using RPGEngine.Game;
    using RPGEngine.Utils;

    public class ClassReader
    {
        public IGameEngine Engine { get; }

        public ClassReader(IGameEngine engine)
        {
            Engine = engine;
        }

        (string Key, double Value) ValueFromJson(JToken json)
        {
            JToken key = json[GcConstants.General.KEY];
            if (key == null)
            {
                throw new MeException($"Key {GcConstants.General.KEY} does not exist.");
            }

            string resultKey = key.Value<string>();
            JToken value = json[GcConstants.General.VALUE];
            if (value == null)
            {
                throw new MeException($"Key {GcConstants.General.VALUE} does not exist.");
            }

            double resultValue = value.Value<double>();

            var result = (Key: resultKey, Value: resultValue);
            return result;
        }

        (string Key, SkillTemplate Template) SkillFromJson(JToken json)
        {
            string skillKey = json.Value<string>();
            return (skillKey,Engine.GetSkillManager().GetSkill(skillKey));
        }

        public ClassTemplate FromJson(JObject json)
        {
            ClassTemplate result = new ClassTemplate();
            result.LoadBase(json);
            JToken baseValues = json[GcConstants.Classes.BASE_VALUES];
            if (baseValues != null)
            {
                JToken[] baseValuesArray = baseValues.ToArray();
                foreach (JToken value in baseValuesArray)
                {
                    var bValue = ValueFromJson(value);
                    result.BasicValues.Add(bValue.Key, bValue.Value);
                }
            }
            else
            {
                throw new MeException($"Key {GcConstants.Classes.BASE_VALUES} does not exist.");
            }

            JToken basicAttributes = json[GcConstants.Classes.BASIC_ATTRIBUTES];
            if (basicAttributes != null)
            {
                JToken[] baseAttributesArray = basicAttributes.ToArray();
                foreach (JToken value in baseAttributesArray)
                {
                    var aValue = ValueFromJson(value);
                    result.Attributes.Add(aValue.Key, aValue.Value);
                }
            }
            else
            {
                throw new MeException($"Key {GcConstants.Classes.BASIC_ATTRIBUTES} does not exist.");
            }
            JToken skills = json[GcConstants.Classes.SKILLS];
            if (skills != null)
            {
                JToken[] skillsArray = skills.ToArray();
                foreach (JToken value in skillsArray)
                {
                    var skill = SkillFromJson(value);
                    if (skill.Template != null)
                    {
                        result.Skills.Add(skill.Key, skill.Template);
                    }
                    else
                    {
                        throw new MeException($"A skill with key {skill.Key} does not exist.");
                    }
                }
            }
            else
            {
                throw new MeException($"Key {GcConstants.Classes.SKILLS} does not exist.");
            }
            
            JToken baseAttack = json[GcConstants.Classes.BASE_ATTACK];
            if (baseAttack == null)
            {
                throw new MeException($"Key {GcConstants.Classes.BASE_ATTACK} does not exist.");
            }
            else
            {
                var skill = SkillFromJson(baseAttack);
                if (skill.Template != null)
                {
                    result.BaseAttack = skill.Template;
                }
                else
                {
                    throw new MeException($"A skill with key {skill.Key} does not exist.");
                }
            }
            return result;
        }
    }
}