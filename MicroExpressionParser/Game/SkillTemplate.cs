using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGEngine.Core;
using RPGEngine.GameConfigReader;

namespace RPGEngine.Game
{
    public enum SkillType
    {
        Cast, //after a dureation, the skill formula is applied
        Channel //for the duration of the channel, every interval seconds the skill formula is applied
    }

    public class SkillCost
    {
        public string ResourceKey { get; }
        public MeNode Amount { get; }
    }

    public class SkillLevelTemplate
    {
        public MeNode Cooldown { get; set; }
        public List<MeNode> Formulas { get; set; }
        public MeNode Interval { get; set; }
        public MeNode Duration { get; set; }
        public MeNode PushBack { get; set; }
        public SkillCost Cost { get; set; }
        public bool Interruptible { get; set; }
        public long NeededLevel { get; set; }

        public SkillLevelTemplate()
        {
            Formulas = new List<MeNode>();
        }
    }

    public class SkillTemplate : BaseObject
    {
        public List<string> aliases { get; set; }
        public SkillType Type { get; set; }
        public List<SkillLevelTemplate> ByLevel { get; set; }

        public SkillTemplate()
        {
            ByLevel = new List<SkillLevelTemplate>();
        }

        public static SkillType FromString(string str)
        {
            foreach (SkillType type in Enum.GetValues(typeof(SkillType)))
                if (str.Equals(type.ToString().ToLower()))
                    return type;
            return SkillType.Cast;
        }

    }
}
