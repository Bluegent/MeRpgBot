using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGEngine.Core;

namespace RPGEngine.Game
{
    public class Player : BaseEntity
    {
        public ClassTemplate Class { get; }
        public Player(IGameEngine engine, ClassTemplate myClass) : base(engine)
        {
            Class = myClass;
            Skills = new Dictionary<string, SkillInstance>();
            //TODO: deserialize properly
            foreach (SkillTemplate skill in Class.Skills.Values)
            {
                Skills.Add(skill.Key,new SkillInstance() {Skill =  skill,SkillLevel = 0});
            }
        }
    }
}
