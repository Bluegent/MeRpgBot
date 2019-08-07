using MicroExpressionParser.Core;
using System.Collections.Generic;

namespace MicroExpressionParser
{
    using MicroExpressionParser.Parser;

    using RPGEngine.Core;
    using RPGEngine.Language;
    using RPGEngine.Parser;

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

        public abstract void TakeDamage(double amount, DamageType type, Entity source);

        public abstract void GetHealed(double amount, Entity source);

        public abstract void Cast(Entity target, string skillKey);

        public abstract EntityProperty GetProperty(string key);

        public abstract void ApplyStatus(StatusTemplate status, Entity source, double duration, double[] values);

        public abstract void Update();

    }
    public enum StackingType
    {
        None, //The buff does not stack, so when it is applied while another stack of it exists on the target, nothing happens.
        Refresh, //The duration of existing stacks is refreshed.
        Independent, //Status stacks have independent durations, so applying a new one will not affect the others.
    }
    public class StatusTemplate
    {
        public MeNode[] ComponentFormulas { get; set; }
        public MeNode Interval { get; set; }
        public MeNode MaxStacks { get; set; }
        public StackingType Type { get; set; }
    }

    public class StatModifier
    {
        public string StatKey { get; set; }
        public double Amount { get; set; }
    }

    public class AppliedStatus
    {
        public StatusTemplate Template { get; set; }
        public double[] NumericValues { get; set; }
        public long RemovalTime { get; set; }
        public long LastTick { get; set; }
        public Entity Source { get; set; }
        public long Interval { get; set; }
    }

    public class MockEntity : Entity
    {
        public List<AppliedStatus> Statuses;
        public Dictionary<string, double> FinalStats { get; set; }
        public IGameEngine Engine { get; set; }

        public MockEntity(IGameEngine engine)
        {
            StatMap = new Dictionary<string, double>
                          {
                              { "CHP", 100 },
                              { "MHP", 100 },
                              { "STR", 5 },
                              { "INT", 5 },
                              { "AGI", 5 },
                              { "DEF",10},
                              { "MDEF",0 }
                          };
            FinalStats = new Dictionary<string, double>(StatMap);
            Engine = engine;
            Statuses = new List<AppliedStatus>();


        }

        public override void TakeDamage(double amount, DamageType type, Entity source)
        {
            FinalStats["CHP"] -= amount;
            StatMap["CHP"] -= amount;
        }

        public override void GetHealed(double amount, Entity source)
        {
            FinalStats["CHP"] += amount;
        }

        public override void Cast(Entity target, string skillKey)
        {
            //do stuff...
        }

        public override EntityProperty GetProperty(string key)
        {
            if (FinalStats.ContainsKey(key))
                return new EntityProperty() { Key = key, Value = FinalStats[key] };
            return null;
        }

        public override void ApplyStatus(StatusTemplate status, Entity source, double duration, double[] values)
        {
            long removeTime = Engine.GetTimer().GetNow() + (long)duration * 1000;
            
            AppliedStatus newStatus = new AppliedStatus() { Source = source, LastTick = 0, RemovalTime = removeTime, Template = status, NumericValues = values };
            MeNode intervalTree = Engine.GetSanitizer().SanitizeSkillEntities(status.Interval, source, this);
            newStatus.Interval = intervalTree.Resolve().Value.ToLong();
            Statuses.Add(newStatus);
        }

        private void RemoveExpiredStatuses()
        {
            long now = Engine.GetTimer().GetNow();
            List<AppliedStatus> remove = new List<AppliedStatus>();
            foreach (AppliedStatus status in Statuses)
            {
                if (status.RemovalTime < now)
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
                            && tree.Value.Value == ParserConstants.Functions[Constants.MOD_VALUE_F])
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
                            && (tree.Value.Value == ParserConstants.Functions[Constants.HARM_F]
                                || tree.Value.Value == ParserConstants.Functions[Constants.HEAL_F]))
                        {
                            MeNode newTree = Engine.GetSanitizer().SanitizeSkillEntities(tree, status.Source, this);
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

        public override void Update()
        {
            //Template handling
            RemoveExpiredStatuses();
            ResetFinalStatMap();
            ApplyModifiers();
            ApplyHealAndHarm();
            SetLastTicks();
            //tick cooldowns
            //tick casting/channeling timers
            //set isFree flag

        }
    }
}
