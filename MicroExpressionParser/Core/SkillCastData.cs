
using RPGEngine;
using RPGEngine.Cleanup;
using RPGEngine.Game;
using RPGEngine.Entities;

namespace RPGEngine.Core
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Remoting.Messaging;

    using Newtonsoft.Json.Linq;

    using RPGEngine.GameConfigReader;
    using RPGEngine.GameInterface;
    using RPGEngine.Managers;
    using RPGEngine.Templates;
    using RPGEngine.Utils;

    public class SkillInstance : IJsonSerializable
    {
        public SkillTemplate Skill { get; set; }
        public int SkillLevel { get; set; }
        public long CooldownFinishTime { get; set; }

        public SkillLevelTemplate Values()
        {
            return Skill.ByLevel[SkillLevel];
        }

        public MeNode[] Formulas => Values().Formulas.ToArray();

        public JObject ToJObject()
        {
            JObject result=new JObject();
            result.Add(GcConstants.General.KEY,Skill.Key);
            result.Add(GcConstants.Skills.COOLDOWN,CooldownFinishTime);
            result.Add(GcConstants.General.LEVEL,SkillLevel);
            return result;
        }

        public bool FromJObject(JObject obj, IGameEngine engine)
        {
            string key = obj[GcConstants.General.KEY].ToObject<string>();
            SkillTemplate template = engine.GetSkillManager().GetSkill(key);
            if (template == null)
                return false;
            Skill = template;
            int level = obj[GcConstants.General.LEVEL].ToObject<int>();
            SkillLevel = level;
            long cooldown = obj[GcConstants.Skills.COOLDOWN].ToObject<long>();
            CooldownFinishTime = cooldown;
            return true;
        }
    }

    public class SkillCastData
    {
        public SkillInstance Skill { get; }
        public long CastFinishTime { get; set; }
        public Entity Target { get; }
        public Entity Source { get; }
        public long NextInterval { get; set;  }
        public long Interval { get;  }
        public bool Interruptible { get; }
        public bool PushBackable { get; }

        public SkillCastData(SkillInstance instance, Entity target, Entity source, long now)
        {
            Skill = instance;
            Target = target;
            Source = source;

            MeNode interruptible = Skill.Values().Interruptible;
            interruptible = Sanitizer.ReplaceTargetAndSource(interruptible, Source, Target);
            Interruptible = interruptible.Resolve().Value.ToBoolean();

            MeNode pushbackAble = Skill.Values().PushBack;
            pushbackAble = Sanitizer.ReplaceTargetAndSource(pushbackAble, Source, Target);
            PushBackable = pushbackAble.Resolve().Value.ToBoolean();

            NextInterval = 0;

            MeNode castDuration = Skill.Values().Duration;
            castDuration = Sanitizer.ReplaceTargetAndSource(castDuration, Source, Target);
            CastFinishTime = now + (long)castDuration.Resolve().Value.ToDouble() * 1000;

            if (Skill.Skill.Type == SkillType.Channel)
            {
                MeNode interval = Skill.Values().Interval;
                interval = Sanitizer.ReplaceTargetAndSource(interval, Source, Target);
                Interval = interval.Resolve().Value.ToLong();
            }
            else
            {
                Interval = 0;
            }
        }
    }
}
