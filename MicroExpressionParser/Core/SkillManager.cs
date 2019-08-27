
using System.Collections.Generic;
using RPGEngine.Game;

namespace RPGEngine.Core
{
    using System.Collections;

    using Newtonsoft.Json.Linq;

    using RPGEngine.GameConfigReader;
    using RPGEngine.Utils;

    public class SkillManager
    {
        public Dictionary<string, SkillTemplate> SkillsByAlias;
        public IGameEngine Engine { get; set; }

        public SkillManager()
        {
            SkillsByAlias = new Dictionary<string, SkillTemplate>();
        }

        public void AddSkill(SkillTemplate skill)
        {
            if(SkillsByAlias.ContainsKey(skill.Key))
				throw new MeException($"Attempted to add skill with key {skill.Key}, but a skill with that key already exists.");
			SkillsByAlias.Add(skill.Key,skill);

            foreach (string alias in skill.Aliases)
            {
                if (!alias.Equals(skill.Key))
                {
                    if (SkillsByAlias.ContainsKey(alias))
                        throw new MeException(
                            $"Attempted to add skill with alias {alias}, but a skill with that alias already exists.");
                    SkillsByAlias.Add(alias, skill);
                }
            }
        }

        public SkillTemplate GetSkill(string key)
        {
            return SkillsByAlias.ContainsKey(key) ? SkillsByAlias[key] : null;
        }

        public void LoadSkillsFromFile(string path)
        {
            JArray skillJson = FileReader.FromPath<JArray>(path);
            SkillReader reader = new SkillReader(Engine);

            foreach (JToken skillValue in skillJson)
            {
                if (skillValue.Type != JTokenType.Object)
                {
                    throw new MeException($"Expected a json object \"{path}\"at  \"{skillValue}\".");
                }

                SkillTemplate newSkill = reader.FromJson(skillValue.ToObject<JObject>());
                AddSkill(newSkill);
            }
        }
    }
}
