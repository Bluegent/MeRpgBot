
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

        public bool IsAutoCasting;

        public string AutoCastSkill;
        public long NextAutoCast;

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
            IsAutoCasting = false;
            NextAutoCast = 0;
            AutoCastSkill = null;


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
            if (IsAutoCasting)
            {
                NextAutoCast = CurrentlyCasting.Skill.CooldownFinishTime + GameConstants.TickTime;
            }
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
            ResourceMap[Entity.HP_KEY].Add(-1*amount);
        }

        public override void Die()
        {
            if(IsDead)
                return;
            
            ResourceMap[HP_KEY].Value = 0;
            Cleanse();
            CancelCurrentCast();
            StopAutoCasting();
            IsDead = true;
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

            IsDead = false;
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
            if(log)
                Engine.Log().LogHeal(this,source,amount);
            return amount;
        }

        public override bool Cast(Entity target, string skillKey, bool autocast = false)
        {
            string tryAlias = Engine.GetSkillManager().GetKeyFromAlias(skillKey);
            if (tryAlias == null)
                tryAlias = skillKey;

            SkillInstance skill = Skills.ContainsKey(tryAlias) ? Skills[tryAlias] : null;
            if (skill == null)
            {
                Engine.Log().Log($"[{Name}] You don't have that skill({tryAlias}).");
                //log that you don't have that skill
                return false;
            }
            if (skill.CooldownFinishTime != 0)
            {
                long seconds = (skill.CooldownFinishTime - Engine.GetTimer().GetNow()) / GameConstants.TickTime;
                Engine.Log().Log($"[{Name}] {skill.Skill.Name} is on cooldown for {seconds} s.");
                return false;
            }

            if (!Free)
            {
                Engine.Log().Log($"[{Name}] You are busy.");
                return false;
            }

            if (!ResourceMap.ContainsKey(skill.Values().Cost.Resource.Key))
            {
                Engine.Log().Log($"[{Name}] You don't have \"{skill.Values().Cost.Resource.Name}\".");
                return false;
            }
            ResourceInstance res = ResourceMap[skill.Values().Cost.Resource.Key];
            double amount = Sanitizer.ReplacePropeties(skill.Values().Cost.Amount, this).Resolve().Value.ToDouble();
            if (!res.CanCast(amount))
            {
                Engine.Log().Log($"[{Name}] Not enough {skill.Values().Cost.Resource.Name} for {skill.Skill.Name}.");
                return false;
            }

            res.Cast(amount);

            Free = false;

            CurrentlyCasting = new SkillCastData(skill, target, this, Engine.GetTimer().GetNow());
            long time = CurrentlyCasting.CastFinishTime - Engine.GetTimer().GetNow();
            if (time >= GameConstants.TickTime * 3)
            {
                string type = skill.Skill.Type == SkillType.Cast ? "casting" : "channeling";
                string castedFinish = Key.Equals(CurrentlyCasting.Target.Key)
                    ? ""
                    : $" on  {CurrentlyCasting.Target.Name}";          
                Engine.Log()
                    .Log(
                        $"[{Name}] Started {type} {CurrentlyCasting.Skill.Skill.Name}{castedFinish}.");
            }
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
            int stackCount = 0;
            foreach(AppliedStatus sts in Statuses)
            {
                if (sts.Template.Key.Equals(newStatus.Template.Key))
                    ++stackCount;
            }
            Engine.Log().Log($"[{Name}] Affected by {status.Name}[{stackCount}].");
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
                        {
                            refresh.RemovalTime = Engine.GetTimer().GetNow() + (long) duration * 1000;
                        }
                        else
                        {
                            AddStatusFromTemplate(status, source, duration, values);
                        }
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
                Engine.Log().Log($"[{Name}] {status.Template.Name}[1] wore off.");
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
                    string castedFinish = Key.Equals(CurrentlyCasting.Target.Key)
                        ? ""
                        : $" on  {CurrentlyCasting.Target.Name}";
                    Engine.Log().Log($"[{Name}] Casted {CurrentlyCasting.Skill.Skill.Name}{castedFinish}.");
                    //casting has finished so resolve the formula
                    MeNode[] toResolve = CurrentlyCasting.Skill.Formulas;
                    foreach (MeNode node in toResolve)
                    {
                        MeNode sanitized = Sanitizer.ReplaceTargetAndSource(node, this, CurrentlyCasting.Target);
                        sanitized.Resolve();
                    }         
                    FinishCasting();
                }
            }
            else if (CurrentlyCasting.Skill.Skill.Type == SkillType.Channel)
            {

                if (CurrentlyCasting.CastFinishTime <= now)
                {
                    Engine.Log().Log($"[{Name}] Finished channeling {CurrentlyCasting.Skill.Skill.Name}.");
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
            CancelCurrentCast();
            StopAutoCasting();
            if (!Free)
            {
                Engine.Log().Log($"[{Name}] You are busy.");
                return;
            }
            if (IsDead)
            {
                return;
            }
            CurrentTarget = target;
            if(target != null)
            Engine.Log().Log($"[{Name}] Targeting {target.Name}.");
        }

        public void StopAutoCasting()
        {
            IsAutoCasting = false;
            AutoCastSkill = null;
            
        }

        private void CancelCurrentCast()
        {
            CurrentlyCasting = null;
        }

        private void CheckAutoCast()
        {
            if (!IsAutoCasting)
                return;
            long now = Engine.GetTimer().GetNow();
            if (CurrentlyCasting == null && (NextAutoCast == 0 || NextAutoCast <= now))
            {
                Cast(CurrentTarget, AutoCastSkill);
            }
        }
        public void StartAutoCasting(string skillKey, Entity target = null)
        {
            CancelCurrentCast();
            NextAutoCast = 0;
            AutoCastSkill = skillKey;
            if(target!=null)
                Target(target);
            IsAutoCasting = true;
        }

        public override void Update()
        {
            if (!IsDead)
            {
                CheckDeath();
                RemoveExpiredStatuses();
                RegenResources();
                ApplyHealAndHarm();
                TickCooldownds();

                CheckAutoCast();
                TickCurrentCast();
            }
            else
            {
                CheckRevive();
            }
        }

        public string GetSkillsString()
        {

            string result = "";
            foreach (SkillInstance skill in Skills.Values)
            {
                result += $"{skill.ToShortString(this)}\n";
            }

            return result;
        }

        public string GetStatusesString()
        {
            string result = "";
            if (Statuses.Count == 0)
            {
                return "No applied status effects.\n";
            }
            foreach (AppliedStatus status in Statuses)
            {
                result += $"{status.GetDisplayString(Engine)}\n";
            }

            return result;
        }
    }

}
