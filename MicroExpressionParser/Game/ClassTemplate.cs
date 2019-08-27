using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGEngine.GameConfigReader;

namespace RPGEngine.Game
{
    using RPGEngine.Core;

    public class ClassTemplate : BaseObject
    {
        public Dictionary<string,SkillTemplate> Skills { get; }
        public Dictionary<string, double> Attributes { get; }
        public Dictionary<string, double> BasicValues { get; }
        public SkillTemplate BaseAttack { get; set; }

        public Dictionary<string, ResourceTemplate>  Resources { get; }

        public ClassTemplate()
        {
            Skills = new Dictionary<string, SkillTemplate>();
            Attributes = new Dictionary<string, double>();
            BasicValues = new Dictionary<string, double>();
            Resources = new Dictionary<string, ResourceTemplate>();
        }

        public SkillTemplate GetSkill(string key)
        {
            return Skills.ContainsKey(key) ? Skills[key] : null;
        }
    }
}
