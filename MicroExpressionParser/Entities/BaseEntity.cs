
using System.Collections.Generic;
using RPGEngine.Cleanup;
using RPGEngine.Core;
using RPGEngine.Game;
using RPGEngine.Language;
using RPGEngine.Parser;

namespace RPGEngine.Entities
{
    public class BaseEntity : Entity
    {

        public const string C_HP_KEY = "CHP";
        public const string M_HP_KEY = "MHP";

        public List<AppliedStatus> Statuses;

        public IGameEngine Engine { get; set; }
        public SkillCastData CurrentlyCasting;

        public Dictionary<string, SkillInstance> Skills;

        public BaseEntity(IGameEngine engine) : base()
        {
            Free = true;
            CurrentlyCasting = null;
            Attributes = new Dictionary<string, double>();
            Engine = engine;
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
            MeNode cd = CurrentlyCasting.Skill.Values().Cooldown;
            cd = Sanitizer.ReplaceTargetAndSource(cd, this, CurrentlyCasting.Target);
            CurrentlyCasting.Skill.CooldownFinishTime = Engine.GetTimer().GetNow() + (long)cd.Resolve().Value.ToDouble() * 1000;
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
            if (InteralAttributes.ContainsKey(propertyKey))
                return true;
            return false;
        }

        public override void RegenResources(long deltaTMs)
        {
            foreach(ResourceInstance resIn in ResourceMap.Values)
            {
                resIn.Refresh();
                resIn.Regen(deltaTMs);
            }
        }

        private void TakeActualDamage(double amount)
        {
            ResourceMap[Entity.HP_KEY].CurrentAmount -= amount;
        }

        public override void TakeDamage(double amount, DamageType type, Entity source, bool periodic = false)
        {
            if (type.GetDodge(source, this))
            {
                if (!periodic)
                    Engine.Log().LogDodge(this, source);
                return;
            }
            double actualAmount = type.GetMitigatedAmount(amount, source, this);
            double resisted = amount - actualAmount;
            TakeActualDamage(actualAmount);

            if (!periodic)
            {
                Engine.Log().LogDamage(this, source, type, actualAmount, resisted);
            }
        }

        public override void GetHealed(double amount, Entity source, bool log = true)
        {
            TakeActualDamage(-amount);
        }

        public override bool Cast(Entity target, string skillKey)
        {
            SkillInstance skill = Skills.ContainsKey(skillKey) ? Skills[skillKey] : null;
            if (skill == null)
            {
                //log that you don't have that skill
                return false;
            }
            if (skill.CooldownFinishTime != 0)
            {
                //log that it's on cooldown
                return false;
            }
            if (!Free)
            {
                //log that player is busy
                return false;
            }

            if (!ResourceMap.ContainsKey(skill.Values().Cost.ResourceKey))
            {
                //log that the player doesn't have the right resource
                return false;
            }
            ResourceInstance res = ResourceMap[skill.Values().Cost.ResourceKey];
            double amount = Sanitizer.ReplacePropeties(skill.Values().Cost.Amount, this).Resolve().Value.ToDouble();
            if (!res.CanCast(amount))
            {
                //log that the player doesn't have enough of resource
                return false;
            }
            
            res.Cast(amount);

            Free = false;

            CurrentlyCasting = new SkillCastData(skill, target, this, Engine.GetTimer().GetNow());

            return true;
        }

        public override EntityProperty GetProperty(string key)
        {
            if (Attributes.ContainsKey(key))
                return new EntityProperty() { Key = key, Value = Attributes[key] };
            if(ResourceMap.ContainsKey(key))
                return new EntityProperty() { Key = key, Value = ResourceMap[key].CurrentAmount };
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
            ResetAttributes();
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
                Statuses.Remove(status);
        }


        private bool IsTime(AppliedStatus status)
        {
            if (status.LastTick == 0)
                return true;
            if (Engine.GetTimer().GetNow() - status.LastTick > status.Interval * 1000)
                return true;
            return false;
        }

        private void ApplyModifiers()
        {
            foreach (AppliedStatus status in Statuses)
            {
                if (IsTime(status))
                {
                    foreach (MeNode tree in status.Template.ComponentFormulas)
                    {
                        if (tree.Value.Type == VariableType.Function
                            && tree.Value.Value == Definer.Instance().Functions[LConstants.MOD_VALUE_F])
                        {
                            StatModifier mod = Engine.GetSanitizer().ResolveStatus(tree, status.NumericValues).ToModifier();
                            Attributes[mod.StatKey] += mod.Amount;
                        }
                    }
                }
            }
        }

        private void ApplyHealAndHarm()
        {
            foreach (AppliedStatus status in Statuses)
            {
                if (IsTime(status))
                {
                    foreach (MeNode tree in status.Template.ComponentFormulas)
                    {
                        if (tree.Value.Type == VariableType.Function
                            && (tree.Value.Value == Definer.Instance().Functions[LConstants.HARM_F]
                                || tree.Value.Value == Definer.Instance().Functions[LConstants.HEAL_F]))
                        {
                            MeNode newTree = Sanitizer.ReplaceTargetAndSource(tree, status.Source, this);
                            Engine.GetSanitizer().ReplaceNumericPlaceholders(newTree, status.NumericValues);
                            newTree.Resolve();
                        }
                    }

                }
            }
        }


        private void SetLastTicks()
        {
            foreach (AppliedStatus status in Statuses)
            {
                if (IsTime(status))
                {
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
                        Sanitizer.ReplaceTargetAndSource(node, this, CurrentlyCasting.Target).Resolve();
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

        public override void Update()
        {
            //Template handling
            RemoveExpiredStatuses();
            ResetAttributes();
            ApplyModifiers();
            RegenResources(1000);
            ApplyHealAndHarm();
            SetLastTicks();
            TickCooldownds();
            TickCurrentCast();
        }
    }

}
