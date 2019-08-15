using System.Globalization;
using RPGEngine.Game;

namespace RPGEngine.Core
{
    using System;
    using System.Collections.Generic;

    using MicroExpressionParser;

    using RPGEngine.Language;

    public class EntityProperty
    {
        public string Key { get; set; }
        public double Value { get; set; }
    }
    public abstract class Entity
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public Dictionary<string, double> StatMap { get; set; }
        public bool Free { get; protected set; }

        public abstract void TakeDamage(double amount, DamageType type, Entity source,bool log = true);

        public abstract void GetHealed(double amount, Entity source,bool log = true);

        public abstract void Cast(Entity target, string skillKey);

        public abstract EntityProperty GetProperty(string key);

        public abstract void ApplyStatus(StatusTemplate status, Entity source, double duration, double[] values);

        public abstract void Update();

        public abstract void Cleanse();


    }

    public class Property
    {
        private readonly Entity _reference;
        private readonly string _key;

        public double Value => _reference.GetProperty(_key).Value;

        public Property(Entity entity, string key)
        {
            _reference = entity;
            _key = key;
        }
    }

    public class BaseEntity : Entity
    {
        public List<AppliedStatus> Statuses;
        public Dictionary<string, double> FinalStats { get; set; }
        public IGameEngine Engine { get; set; }
        public SkillCastData CurrentlyCasting;

        public Dictionary<string, SkillInstance> Skills;

        public BaseEntity(IGameEngine engine)
        {
            Free = true;
            CurrentlyCasting = null;
            StatMap = new Dictionary<string, double>
                          {
                              { "CHP", 100 },
                              { "MHP", 100 },
                              { "STR", 5 },
                              { "INT", 5 },
                              { "AGI", 10 },
                              { "DEF",10},
                              { "MDEF",0 }
                          };
            FinalStats = new Dictionary<string, double>(StatMap);
            Engine = engine;
            Statuses = new List<AppliedStatus>();
            Skills = new Dictionary<string, SkillInstance>();


        }

        public override void TakeDamage(double amount, DamageType type, Entity source, bool log = true)
        {
            if (type.GetDodge(source, this))
            {
                if(log)
                    Engine.Log().LogDodge(this,source);
                return;
            }
            double actualAmount = type.GetMitigatedAmount(amount, source, this);
            double resisted = amount - actualAmount;
            FinalStats["CHP"] -= actualAmount;
            StatMap["CHP"] -= actualAmount;
            if(log)
                Engine.Log().LogDamage(this,source,type, actualAmount, resisted);
        }

        public override void GetHealed(double amount, Entity source, bool log = true)
        {
            FinalStats["CHP"] += amount;
        }

        public override void Cast(Entity target, string skillKey)
        {
            SkillInstance skill = Skills.ContainsKey(skillKey)? Skills[skillKey] : null;
            if (skill == null)
            {
                //log that you don't have that skill
                return;
            }
            if (skill.CooldownFinishTime != 0)
            {
                //log that it's on cooldown
                return;
            }
            Free = false;

            CurrentlyCasting = new SkillCastData() {Instance = skill};
            CurrentlyCasting.Target = target;
            CurrentlyCasting.NextInterval = 0;

            long now = Engine.GetTimer().GetNow();

            //set when we're done casting
            MeNode castDuration = skill.Skill.ByLevel[skill.SkillLevel].Duration;
            castDuration = Engine.GetSanitizer().ReplaceTargetAndSource(castDuration, this, target);
            CurrentlyCasting.CastFinishTime =  now + (long)castDuration.Resolve().Value.ToDouble();

            //get interval if it's a channel skill
            if (skill.Skill.Type == SkillType.Channel)
            {
                MeNode interval = skill.Values().Interval;
                interval = Engine.GetSanitizer().ReplaceTargetAndSource(interval, this, target);
                CurrentlyCasting.Interval = (long)interval.Resolve().Value.ToDouble();
            }

            //set skill's cooldown
            MeNode cd = skill.Skill.ByLevel[skill.SkillLevel].Cooldown;
            cd = Engine.GetSanitizer().ReplaceTargetAndSource(cd, this, target);
            skill.CooldownFinishTime = now + (long) cd.Resolve().Value.ToDouble();


        }

        public override  EntityProperty GetProperty(string key)
        {
            if (FinalStats.ContainsKey(key))
                return new EntityProperty() { Key = key, Value = FinalStats[key] };
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
            MeNode intervalTree = Engine.GetSanitizer().ReplaceTargetAndSource(status.Interval, source, this);
            newStatus.Interval = intervalTree.Resolve().Value.ToLong();
            Statuses.Add(newStatus);
        }

        private long GetRemoveTime(double duration)
        {
            if (duration > 0.0)
                return Engine.GetTimer().GetNow() + (long)duration * 1000;
            else
            {
                return 0;
            }
        }

        public override void Cleanse()
        {
            Statuses.Clear();
            ResetFinalStatMap();
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

        private void ResetFinalStatMap()
        {
            foreach (KeyValuePair<string, double> pair in StatMap)
            {

                FinalStats[pair.Key] = StatMap[pair.Key];
            }
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
                            && tree.Value.Value == Definer.Get().Functions[Constants.MOD_VALUE_F])
                        {
                            StatModifier mod = Engine.GetSanitizer().ResolveStatus(tree, status.NumericValues).ToModifier();
                            FinalStats[mod.StatKey] += mod.Amount;
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
                            && (tree.Value.Value == Definer.Get().Functions[Constants.HARM_F]
                                || tree.Value.Value == Definer.Get().Functions[Constants.HEAL_F]))
                        {
                            MeNode newTree = Engine.GetSanitizer().ReplaceTargetAndSource(tree, status.Source, this);
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
            if (CurrentlyCasting.Instance.Skill.Type == SkillType.Cast)
            {
                if (CurrentlyCasting.CastFinishTime < now)
                {
                    //casting has finished so resolve the formula
                    MeNode[] toResolve = CurrentlyCasting.Instance.Values().Formulas;
                    foreach (MeNode node in toResolve)
                    {
                        Engine.GetSanitizer().ReplaceTargetAndSource(node, this, CurrentlyCasting.Target).Resolve();
                    }
                    CurrentlyCasting = null;
                    Free = true;
                }
            }
            else if (CurrentlyCasting.Instance.Skill.Type == SkillType.Channel)
            {

                if (CurrentlyCasting.CastFinishTime < now)
                {
                    //channeling has ended, remove it
                    CurrentlyCasting = null;
                    Free = true;
                }
                else
                {
                    //apply the formulas if it's the case
                    if (CurrentlyCasting.NextInterval == 0 || CurrentlyCasting.NextInterval < now)
                    {
                        MeNode[] toResolve = CurrentlyCasting.Instance.Values().Formulas;
                        foreach (MeNode node in toResolve)
                        {
                            Engine.GetSanitizer().ReplaceTargetAndSource(node, this, CurrentlyCasting.Target).Resolve();
                        }
                        CurrentlyCasting.NextInterval = now + CurrentlyCasting.Interval;
                    }
                }
            }
        }

        public override void Update()
        {
            //Template handling
            RemoveExpiredStatuses();
            ResetFinalStatMap();
            ApplyModifiers();
            ApplyHealAndHarm();
            SetLastTicks();
            TickCooldownds();
            TickCurrentCast();

            //tick casting/channeling timers
            //set isFree flag

        }
    }
}
