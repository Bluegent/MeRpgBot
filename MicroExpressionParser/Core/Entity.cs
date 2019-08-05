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

        public abstract void ApplyStatus(StatusTemplate status, double duration);

    }

    public class StatusTemplate
    {
        public FunctionalNode formula { get; set; }
    }

    public class StatModifier
    {
        public string key { get; set; }
        public double amount { get; set; }
    }

    public class AppliedStatus
    {
        public StatusTemplate status { get; set; }
        public double[] values { get; set; }
        public long removeTime { get; set; }
    }

    public class MockEntity : Entity
    {

        public Dictionary<string, double> FinalStats { get; set; }

        public MockEntity()
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

        public override void ApplyStatus(StatusTemplate status, double duration)
        {
            throw new NotImplementedException();
        }
    }
}
