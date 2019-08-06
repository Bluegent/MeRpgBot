using MicroExpressionParser.Core;
using System.Collections.Generic;

namespace MicroExpressionParser
{
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

        public abstract void TakeDamage(double amount, DamageType type,Entity source);

        public abstract void GetHealed(double amount,Entity source);

        public abstract void Cast(Entity target, string skillKey);

        public abstract EntityProperty GetProperty(string key);

        public abstract void ApplyStatus(StatusTemplate status, Entity source, double duration, double[] values);

        public abstract void Update();

    }

    public class StatusTemplate
    {
        public FunctionalNode[] formulas { get; set; }
        public int interval { get; set; }
    }

    public class StatModifier
    {
        public string key { get; set; }
        public double amount { get; set; }
    }

    public class AppliedStatus
    {
        public StatusTemplate status { get; set; }
        public MeVariable[] resolved { get; set; }
        public double[] values { get; set; }
        public long removeTime { get; set; }
        public long lastTick { get; set; }
        public Entity source { get; set; }
    }

    public class MockEntity : Entity
    {
        public List<AppliedStatus> statuses;
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
            statuses = new List<AppliedStatus>();


        }

        public override void TakeDamage(double amount, DamageType type, Entity source)
        {
            FinalStats["CHP"] -= amount;
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
            if(FinalStats.ContainsKey(key))
                return new EntityProperty(){Key = key, Value = FinalStats[key]};
            return null;
        }

        public override void ApplyStatus(StatusTemplate status,Entity source, double duration, double[] values)
        {
            long now = Engine.GetTimer().GetNow();
            AppliedStatus newStatus = new AppliedStatus() { source= source, lastTick = 0,removeTime = now+(long)duration,status=status,values = values };
            statuses.Add(newStatus);
        }

        private void RemoveExpiredStatuses()
        {
            long now = Engine.GetTimer().GetNow();
            List<AppliedStatus> remove = new List<AppliedStatus>();
            foreach(AppliedStatus status in statuses)
            {
                if (status.removeTime < now)
                    remove.Add(status);
            }
        
            foreach (AppliedStatus status in remove)
                statuses.Remove(status);
        }

        private void ResetFinalStatMap()
        {
            foreach(KeyValuePair<string,double> pair in StatMap)
            {

                FinalStats[pair.Key] = StatMap[pair.Key];
            }
        }

        private bool IsTime(AppliedStatus status)
        {
            if (status.lastTick == 0)
                return true;
            if (Engine.GetTimer().GetNow() - status.lastTick > status.status.interval)
                return true;
            return false;
        }

        private void ApplyModifiers()
        {
            //first apply modifiers
            foreach (AppliedStatus status in statuses)
            {
                if (IsTime(status))
                {
                    foreach (FunctionalNode tree in status.status.formulas)
                    {
                        if (tree.Value.Type == VariableType.Function
                            && tree.Value.Value == ParserConstants.Functions[StringConstants.MOD_VALUE_F])
                        {
                            StatModifier mod = Engine.GetSanitizer().ResolveStatus(tree, status.values).ToModifier();
                            FinalStats[mod.key] += mod.amount;
                        }
                    }
                }
            }
        }
        private void ApplyHealAndHarm()
        {
            foreach (AppliedStatus status in statuses)
            {
                if (IsTime(status))
                {
                    foreach (FunctionalNode tree in status.status.formulas)
                    {
                        if (tree.Value.Type == VariableType.Function
                            && (tree.Value.Value == ParserConstants.Functions[StringConstants.HARM_F]
                                || tree.Value.Value == ParserConstants.Functions[StringConstants.HEAL_F]))
                        {
                            Engine.GetSanitizer().SanitizeSkillEntities(tree, status.source, this);
                            FunctionalTreeConverter.ResolveNode(tree, 0);
                        }
                    }
                   
                }
            }
        }


        private void SetLastTicks()
        {
            foreach (AppliedStatus status in statuses)
            {
                if (IsTime(status))
                {
                    status.lastTick = Engine.GetTimer().GetNow();
                }
            }
        }

        public override void Update()
        {
            RemoveExpiredStatuses();
            //tick cooldowns
            ResetFinalStatMap();
            ApplyModifiers();
            ApplyHealAndHarm();
            SetLastTicks();
        }
    }
}
