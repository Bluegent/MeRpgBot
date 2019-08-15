using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGEngine.Game;

namespace RPGEngine.Core
{

    public class SkillInstance
    {
        public SkillTemplate Skill { get; set; }
        public int SkillLevel { get; set; }
        public long CooldownFinishTime { get; set; }

        public SkillLevelTemplate Values()
        {
            return Skill.ByLevel[SkillLevel];
        }
    }

    public class SkillCastData
    {
        public SkillInstance Instance { get; set; }
        public long CastFinishTime { get; set; }
        public Entity Target { get; set; }
        public long NextInterval { get; set; }
        public long Interval { get; set; }
    }
}
