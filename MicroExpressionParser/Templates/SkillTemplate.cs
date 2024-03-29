﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGEngine.Core;
using RPGEngine.GameConfigReader;

namespace RPGEngine.Templates
{
    using RPGEngine.Game;

    public class SkillTemplate : BaseObject
    {
        public List<string> Aliases { get; set; }
        public SkillType Type { get; set; }
        public List<SkillLevelTemplate> ByLevel { get; set; }

        public SkillTemplate()
        {
            ByLevel = new List<SkillLevelTemplate>();
            Aliases = new List<string>();
        }

        public void TypeFromString(string str)
        {
            foreach (SkillType type in Enum.GetValues(typeof(SkillType)))
                if (str.Equals(type.ToString().ToLower()))
                {
                    Type = type;
                    return;
                }
            throw new MeException($"Unknown skill type {str} for skill {Name}.");
        }

    }

    public enum SkillType
    {
        Cast, //after a dureation, the skill formula is applied
        Channel //for the duration of the channel, every interval seconds the skill formula is applied
    }

    public class SkillCost
    {
        public ResourceTemplate Resource;
        public MeNode Amount { get; }

        public SkillCost(ResourceTemplate resource, MeNode amount)
        {
            Amount = amount;
            Resource = resource;
        }
    }

    public class SkillLevelTemplate
    {
        public MeNode Cooldown { get; set; }
        public List<MeNode> Formulas { get; set; }
        public MeNode Interval { get; set; }
        public MeNode Duration { get; set; }
        public MeNode PushBack { get; set; }
        public SkillCost Cost { get; set; }
        public MeNode Interruptible { get; set; }
        public long NeededLevel { get; set; }
        public MeNode SkillThreat { get; set; }

        public SkillLevelTemplate()
        {
            Formulas = new List<MeNode>();
        }
    }
}
