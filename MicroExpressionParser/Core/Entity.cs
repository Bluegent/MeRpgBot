using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public abstract void ApplyStatus(StatusTemplate status, double duration, double[] values);

        public abstract void Update();

    }

    public class StatusTemplate
    {
        internal FunctionalNode[] formula;

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
        public FunctionalNode[] resolved { get; set; }
        public double[] values { get; set; }
        public long removeTime { get; set; }
        public long lastTick { get; set; }
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
            FinalStats = new Dictionary<string, double>();
            Engine = engine;


        }

        public override void TakeDamage(double amount, DamageType type, Entity source)
        {
            StatMap["CHP"] -= amount;
        }

        public override void GetHealed(double amount, Entity source)
        {
            StatMap["CHP"] += amount;
        }

        public override void Cast(Entity target, string skillKey)
        {
            //do stuff...
        }

        public override EntityProperty GetProperty(string key)
        {
            if(StatMap.ContainsKey(key))
                return new EntityProperty(){Key = key, Value = StatMap[key]};
            return null;
        }

        public override void ApplyStatus(StatusTemplate status, double duration, double[] values)
        {
            long now = Engine.getTimer().GetNow();
            AppliedStatus newStatus = new AppliedStatus() { lastTick = 0,removeTime = now+(long)duration,status=status,values = values };
            statuses.Add(newStatus);
        }

        private void RemoveExpiredStatuses()
        {
            long now = Engine.getTimer().GetNow();
            List<AppliedStatus> remove = new List<AppliedStatus>();
            foreach(AppliedStatus status in statuses)
            {
                if (status.removeTime < now)
                    remove.Add(status);
            }
        
            foreach (AppliedStatus status in remove)
                statuses.Remove(status);
        }

        private void ApplyModifiers()
        {

        }

        public override void Update()
        {
            RemoveExpiredStatuses();
            //tick cooldowns
            //refresh new stat map
            //calculate stats by applying modifiers
            //apply harmful ticks
        }
    }
}
