namespace RPGEngine.GameConfigReader
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
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
        Tuple<string, SkillTemplate> SkillFromJson(JToken json, ClassTemplate result)
        {
            string skillKey = json.Value<string>();
            SkillTemplate skill = Engine.GetSkillManager().GetSkill(skillKey);
            if (skill == null)
            {
                throw new MeException($"Class {result.Name} references skill with key {skillKey}, which does not exist.");
            }
            return new Tuple<string, SkillTemplate>(skillKey, skill);
        }


        public void AddBaseValueVector(string key, JObject json, ClassTemplate result, Dictionary<string, double> values)
        {
            JToken[] baseValueArray = JsonUtils.ValidateJsonEntry(
                key,
                json,
                JTokenType.Array, $"Key {GcConstants.Classes.BASE_VALUES} does not exist for class {result.Name}.").ToArray();
            foreach (JToken baseValue in baseValueArray)
            {
                if (baseValue.Type != JTokenType.Object)
                {
                    throw new MeException($"Unknown key-value pair for basic attributes in class {result.Name}.");
                }

                JObject baseValueObject = (JObject)baseValue;
                JToken objectKey = JsonUtils.ValidateJsonEntry(GcConstants.General.KEY, baseValueObject, JTokenType.String, $"Unknown key for basic attributes in class {result.Name}.");
                JToken value = JsonUtils.ValidateJsonEntry(GcConstants.General.VALUE, baseValueObject, JTokenType.String, $"Unknown value for basic attributes {objectKey} in class {result.Name}."); ;
                values.Add(objectKey.ToString(), double.Parse(value.ToString(),NumberStyles.Any,CultureInfo.InvariantCulture));
            }
        }


        public ClassTemplate FromJson(JObject json)
        {
            ClassTemplate result = new ClassTemplate();
            result.LoadBase(json);

            AddBaseValueVector(GcConstants.Classes.BASE_VALUES, json, result, result.BasicValues);
            AddBaseValueVector(GcConstants.Classes.BASIC_ATTRIBUTES, json, result, result.Attributes);

            JToken[] skills = JsonUtils.GetValueOrDefault<JToken[]>(json, GcConstants.Classes.SKILLS, null);
            if (skills != null)
            {
                foreach (JToken value in skills)
                {
                    Tuple<string, SkillTemplate> skill = SkillFromJson(value, result);
                    result.Skills.Add(skill.Item1, skill.Item2);

                }
            }

            JToken baseAttack = JsonUtils.ValidateJsonEntry(GcConstants.Classes.BASE_ATTACK, json, JTokenType.String, $"Class {result.Name} does not contain a {GcConstants.Classes.BASE_ATTACK} entry.");
            Tuple<string, SkillTemplate> baseSkill = SkillFromJson(baseAttack, result);
            result.BaseAttack = baseSkill.Item2;


            return result;
        }
    }
}