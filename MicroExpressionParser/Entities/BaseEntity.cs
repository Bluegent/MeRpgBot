
using System.Collections.Generic;
using RPGEngine.Cleanup;
using RPGEngine.Core;
using RPGEngine.Game;
using RPGEngine.Language;

namespace RPGEngine.Entities
{
    using RPGEngine.Templates;

    public class BaseEntity : Entity
    {
        public List<AppliedStatus> Statuses;
        public SkillCastData CurrentlyCasting;
        public Entity CurrentTarget { get; private set; }
        public bool HasTarget
        {
            get
            {
                return (CurrentTarget != null && !CurrentTarget.IsDead);
            }
        }

        public Dictionary<string, SkillInstance> Skills;

        public BaseEntity(IGameEngine engine) : base(engine)
        {
            Free = true;
            CurrentlyCasting = null;
            Statuses = new List<AppliedStatus>();
            Skills = new Dictionary<string, SkillInstance>();


        }

        public override void AddPushback(long delay)
        {
            if (CurrentlyCasting == null)
                return;
            if (!CurrentlyCasting.Skill.Values().PushBack.Resolve().Value.ToBoolean())
                return;
            switch (CurrentlyCasting.Skill.Skill.Type)
            {
                case SkillType.Cast:
                    {
                        CurrentlyCasting.CastFinishTime += delay * 1000;
                        break;
                    }

                case SkillType.Channel:
                    {
                        CurrentlyCasting.CastFinishTime -= delay * 1000;
                        break;
                    }
            }
        }

        private void SetCurrentCD()
        {
            //set skill's cooldown]
            if (CurrentlyCasting == null)
                return;
            MeNode cd = CurrentlyCasting.Skill.Values().Cooldown;
            cd = Sanitizer.ReplaceTargetAndSource(cd, this, CurrentlyCasting.Target);
            CurrentlyCasting.Skill.CooldownFinishTime = Engine.GetTimer().GetFuture((long)cd.Resolve().Value.ToDouble());
        }

        private void FinishCasting()
        {
            SetCurrentCD();

            //casting has ended, remove it
            CurrentlyCasting = null;
            Free = true;
        }

        public override void InterruptCasting()
        {
            if (CurrentlyCasting == null)
                return;
            if (!CurrentlyCasting.Interruptible)
                return;
            FinishCasting();
        }

        public override bool HasProperty(string propertyKey)
        {
            if (Attributes.ContainsKey(propertyKey))
                return true;
            if (ResourceMap.ContainsKey(propertyKey))
                return true;
            if (Stats.ContainsKey(propertyKey))
                return true;
            if (BaseValueMap.ContainsKey(propertyKey))
                return true;

            return false;
        }

        public override void RegenResources()
        {
            foreach (ResourceInstance resIn in ResourceMap.Values)
            {
                resIn.Regen(Engine.GetTimer().GetNow());
            }
        }

        private void TakeActualDamage(double amount)
        {
            ResourceMap[Entity.HP_KEY].Value -= amount;
        }

        public override void Die()
        {
            ResourceMap[HP_KEY].Value = 0;
            Cleanse();
            FinishCasting();
        }

        public override void Revive()
        {
            if (!IsDead)
                return;
            RefreshProperties();
            foreach (ResourceInstance res in ResourceMap.Values)
            {
                res.Replentish();
            }
        }

        public override double TakeDamage(double amount, DamageTypeTemplate typeTemplate, Entity source, bool periodic = false)
        {
            if (typeTemplate.GetDodge(source, this))
            {
                if (!periodic)
                    Engine.Log().LogDodge(this, source);
                return 0;
            }
            double actualAmount = typeTemplate.GetMitigatedAmount(amount, source, this);
            double resisted = amount - actualAmount;
            TakeActualDamage(actualAmount);

            if (!periodic)
            {
                Engine.Log().LogDamage(this, source, typeTemplate, actualAmount, resisted);
            }
            return actualAmount;
        }

        public override double GetHealed(double amount, Entity source, bool log = true)
        {
            TakeActualDamage(-amount);
            return amount;
        }

        public override bool Cast(Entity target, string skillKey)
        {
            SkillInstance skill = Skills.ContainsKey(skillKey) ? Skills[skillKey] : null;
            if (skill == null)
            {
                Engine.Log().Log($"You do not have a skill with the key \"{skillKey}\".");
                //log that you don't have that skill
                return false;
            }
            if (skill.CooldownFinishTime != 0)
            {
                long seconds = (skill.CooldownFinishTime - Engine.GetTimer().GetNow()) / GameConstants.TickTime;
                Engine.Log().Log($"Skill {skill.Skill.Name} is on cooldown for {seconds} seconds.");
                return false;
            }
            if (!Free)
            {
                Engine.Log().Log($"You are busy.");
                return false;
            }

            if (!ResourceMap.ContainsKey(skill.Values().Cost.ResourceKey))
            {
                Engine.Log().Log($"You don't have the necessary resource \"{skill.Values().Cost.ResourceKey}\".");
                return false;
            }
            ResourceInstance res = ResourceMap[skill.Values().Cost.ResourceKey];
            double amount = Sanitizer.ReplacePropeties(skill.Values().Cost.Amount, this).Resolve().Value.ToDouble();
            if (!res.CanCast(amount))
            {
                Engine.Log().Log($"Not enough {skill.Values().Cost.ResourceKey}.");
                return false;
            }

            res.Cast(amount);

            Free = false;

            CurrentlyCasting = new SkillCastData(skill, target, this, Engine.GetTimer().GetNow());
            Engine.Log().Log($"{Name} is casting {skill.Skill.Name}");
            return true;
        }




        public override BaseProperty GetProperty(string key)
        {
            if (Attributes.ContainsKey(key))
                return Attributes[key];
            if (ResourceMap.ContainsKey(key))
                return ResourceMap[key];
            if (Stats.ContainsKey(key))
                return Stats[key];
            if (BaseValueMap.ContainsKey(key))
                return BaseValueMap[key];
            return null;
        }

        private AppliedStatus GetStatusInstance(string key)
        {

            foreach (AppliedStatus myStatus in Statuses)
            {
                if (myStatus.Template.Key.Equals(key))
                {
                    return myStatus;
                }
            }

            return null;
        }

        private int GetStatusStackCount(string key)
        {
            int count = 0;
            foreach (AppliedStatus myStatus in Statuses)
            {
                if (myStatus.Template.Key.Equals(key))
                {
                    ++count;
                }
            }

            return count;
        }

        private void AddStatusFromTemplate(StatusTemplate status, Entity source, double duration, double[] values)
        {
            long removeTime = GetRemoveTime(duration);
            AppliedStatus newStatus = new AppliedStatus() { Source = source, LastTick = 0, RemovalTime = removeTime, Template = status, NumericValues = values };
            MeNode intervalTree = Sanitizer.ReplaceTargetAndSource(status.Interval, source, this);
            newStatus.Interval = intervalTree.Resolve().Value.ToLong();
            Statuses.Add(newStatus);
            foreach (MeNode tree in newStatus.Template.Modifiers)
            {
                StatModifier mod = Engine.GetSanitizer().ResolveStatus(tree, newStatus.NumericValues).ToModifier();
                newStatus.MyMods.Add(mod);
                Attributes[mod.StatKey].Modifiers.Add(mod);
            }
            RefreshProperties();
        }

        private long GetRemoveTime(double duration)
        {
            if (duration > 0.0)
                return Engine.GetTimer().GetNow() + (long)duration * 1000;
            return 0;
        }

        public override void Cleanse()
        {
            Statuses.Clear();
            RefreshProperties();
        }

        public override void ApplyStatus(StatusTemplate status, Entity source, double duration, double[] values)
        {
            switch (status.Type)
            {
                case StackingType.Refresh:
                    {
                        AppliedStatus refresh = GetStatusInstance(status.Key);
                        if (refresh != null)
                            refresh.RemovalTime = Engine.GetTimer().GetNow() + (long)duration * 1000;
                        break;
                    }
                case StackingType.None:
                    {
                        AppliedStatus refresh = GetStatusInstance(status.Key);
                        if (refresh == null)
                        {
                            AddStatusFromTemplate(status, source, duration, values);
                        }

                        break;
                    }
                case StackingType.Independent:
                    {
                        long maxStacks = status.MaxStacks.Resolve().Value.ToLong();
                        int currentStacks = GetStatusStackCount(status.Key);
                        if (maxStacks == 0 || maxStacks > currentStacks)
                        {
                            AddStatusFromTemplate(status, source, duration, values);
                        }

                        break;
                    }
            }

        }

        private void RemoveExpiredStatuses()
        {
            long now = Engine.GetTimer().GetNow();
            List<AppliedStatus> remove = new List<AppliedStatus>();
            foreach (AppliedStatus status in Statuses)
            {
                if (status.RemovalTime != 0 && status.RemovalTime < now)
                    remove.Add(status);
            }

            foreach (AppliedStatus status in remove)
            {
                status.Remove(this);
                Statuses.Remove(status);
            }
            if (remove.Count != 0)
                RefreshProperties();
        }


        private bool IsTime(AppliedStatus status)
        {
            if (status.LastTick == 0)
                return true;
            if (Engine.GetTimer().GetNow() - status.LastTick > status.Interval * 1000)
                return true;
            return false;
        }

        private void ApplyHealAndHarm()
        {
            foreach (AppliedStatus status in Statuses)
            {
                if (IsTime(status))
                {
                    foreach (MeNode tree in status.Template.HpMods)
                    {
                        MeNode newTree = Sanitizer.ReplaceTargetAndSource(tree, status.Source, this);
                        Engine.GetSanitizer().ReplaceNumericPlaceholders(newTree, status.NumericValues);
                        newTree = newTree.Resolve();
                        if (tree.Value.GetString() == LConstants.HEAL_F)
                        {
                            status.TotalHeal += newTree.Value.ToDouble();
                        }
                        else
                        {
                            status.TotalDamamge += newTree.Value.ToDouble();
                        }
                    }
                    status.LastTick = Engine.GetTimer().GetNow();
                }
            }
        }


        private void TickCooldownds()
        {
            long now = Engine.GetTimer().GetNow();
            foreach (SkillInstance skill in Skills.Values)
            {
                if (skill.CooldownFinishTime < now)
                {
                    skill.CooldownFinishTime = 0;
                }
            }
        }

        private void TickCurrentCast()
        {
            if (CurrentlyCasting == null)
                return;
            long now = Engine.GetTimer().GetNow();
            if (CurrentlyCasting.Skill.Skill.Type == SkillType.Cast)
            {
                if (CurrentlyCasting.CastFinishTime <= now)
                {
                    //casting has finished so resolve the formula
                    MeNode[] toResolve = CurrentlyCasting.Skill.Formulas;
                    foreach (MeNode node in toResolve)
                    {

                        MeNode sanitized = Sanitizer.ReplaceTargetAndSource(node, this, CurrentlyCasting.Target);
                        string unsanitized = node.ToString();
                        string debug = sanitized.ToString();
                        sanitized.Resolve();
                    }

                    FinishCasting();
                }
            }
            else if (CurrentlyCasting.Skill.Skill.Type == SkillType.Channel)
            {

                if (CurrentlyCasting.CastFinishTime <= now)
                {
                    FinishCasting();
                }
                else
                {
                    //apply the formulas if it's the case
                    if (CurrentlyCasting.NextInterval == 0 || CurrentlyCasting.NextInterval < now)
                    {
                        MeNode[] toResolve = CurrentlyCasting.Skill.Formulas;
                        foreach (MeNode node in toResolve)
                        {
                            Sanitizer.ReplaceTargetAndSource(node, this, CurrentlyCasting.Target).Resolve();
                        }
                        CurrentlyCasting.NextInterval = now + CurrentlyCasting.Interval * 1000;
                    }
                }
            }
        }

        public void Target(Entity target)
        {
            if (!Free)
            {
                Engine.Log().Log("Casting...");
                return;
            }

            if (IsDead)
            {
                Engine.Log().Log("You are dead.");
                return;
            }
            CurrentTarget = target;
            if(target != null)
            Engine.Log().Log($"{Name} is now targeting {target.Name}.");
        }

        public override void Update()
        {
            //Template handling
            if (!IsDead)
            {
                RemoveExpiredStatuses();
                RegenResources();
                ApplyHealAndHarm();
                TickCooldownds();
                TickCurrentCast();
                CheckDeath();
            }
            else
            {
                CheckRevive();
            }
        }
    }

}
