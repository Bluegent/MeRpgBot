
using RPGEngine;
using RPGEngine.Cleanup;
using RPGEngine.Game;
using RPGEngine.Entities;

namespace RPGEngine.Core
{
    using RPGEngine.Templates;

    public class SkillInstance
    {
        public SkillTemplate Skill { get; set; }
        public int SkillLevel { get; set; }
        public long CooldownFinishTime { get; set; }

        public SkillLevelTemplate Values()
        {
            return Skill.ByLevel[SkillLevel];
        }

        public MeNode[] Formulas => Values().Formulas.ToArray();
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
