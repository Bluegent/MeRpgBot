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

    }
    public class MockEntity : Entity
    {

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
    }
}
