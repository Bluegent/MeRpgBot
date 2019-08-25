
using System.Collections.Generic;
using RPGEngine.Game;

namespace RPGEngine.Core
{
    public class SkillManager
    {
        public Dictionary<string, SkillTemplate> SkillsByAlias;

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
                if (SkillsByAlias.ContainsKey(skill.Key))
                    throw new MeException($"Attempted to add skill with alias {alias}, but a skill with that alias already exists.");
                SkillsByAlias.Add(skill.Key, skill);
            }
        }

        public SkillTemplate GetSkill(string key)
        {
            return SkillsByAlias.ContainsKey(key) ? SkillsByAlias[key] : null;
        }
    }
}
