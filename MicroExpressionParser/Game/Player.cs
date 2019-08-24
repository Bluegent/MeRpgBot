using System.Collections.Generic;
using RPGEngine.Core;
using RPGEngine.Entities;

namespace RPGEngine.Game
{
    public class Player 
    {
        public ClassTemplate Class { get; }
        public Dictionary<string, double> Stats;

        public LevelableEntity Entity { get; }

        public Player(IGameEngine engine, ClassTemplate myClass)
        {
            Stats = new Dictionary<string, double>();
            Class = myClass;
            Dictionary<string, SkillInstance> skills = new Dictionary<string, SkillInstance>();
            //TODO: deserialize properly
            foreach (SkillTemplate skill in Class.Skills.Values)
            {
                skills.Add(skill.Key,new SkillInstance() {Skill =  skill,SkillLevel = 0});
            }

            Entity = new LevelableEntity(engine);
            Entity.Skills = skills;
        }
    }
}
