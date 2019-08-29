
using System.Collections.Generic;
using RPGEngine.Game;

namespace RPGEngine.Managers
{
    using System.Collections;

    using global::Discord.Commands;

    using Newtonsoft.Json.Linq;

    using RPGEngine.Core;
    using RPGEngine.GameConfigReader;
    using RPGEngine.Templates;
    using RPGEngine.Utils;

    public class SkillManager
    {
        public Dictionary<string, SkillTemplate> SkillsByKey;

        public Dictionary<string, string> AliasToKey;
        public IGameEngine Engine { get; set; }

        public SkillManager()
        {
            SkillsByKey = new Dictionary<string, SkillTemplate>();
            AliasToKey = new Dictionary<string, string>();
        }

        public void AddSkill(SkillTemplate skill)
        {
            if (SkillsByKey.ContainsKey(skill.Key))
                throw new MeException($"Attempted to add skill with key {skill.Key}, but a skill with that key already exists.");
            SkillsByKey.Add(skill.Key, skill);

            foreach (string alias in skill.Aliases)
            {
                if (!alias.Equals(skill.Key))
                {
                    if (AliasToKey.ContainsKey(alias))
                        throw new MeException(
                            $"Attempted to add skill with alias {alias}, but a skill with that alias already exists.");
                    AliasToKey.Add(alias, skill.Key);
                }
            }
        }

        public SkillTemplate GetSkill(string key)
        {
            if (AliasToKey.ContainsKey(key))
            {
                string skillKey = AliasToKey[key];
                if (SkillsByKey.ContainsKey(skillKey))
                {
                    return SkillsByKey[skillKey];
                }
                return null;

            }
            else
            {
                return SkillsByKey.ContainsKey(key) ? SkillsByKey[key] : null;
            }
        }

        public void LoadSkillsFromFile(string path)
        {
            JArray skillJson = FileHandler.FromPath<JArray>(path);
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

        public string GetKeyFromAlias(string alias)
        {
            return AliasToKey.ContainsKey(alias) ? AliasToKey[alias] : null;
        }
    }
}
