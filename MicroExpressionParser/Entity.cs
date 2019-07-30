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

        public abstract void TakeDamage(double amount, DamageType type);

        public abstract EntityProperty GetProperty(string key);

    }
    public class MockEntity : Entity { 
    
        public MockEntity()
        {
            StatMap = new Dictionary<string, double>
                          {
                              { "CHP", 100 },
                              { "MHP", 100 },
                              { "STR", 5 },
                              { "INT", 5 },
                              { "AGI", 5 }
                          };
                          

        }

        public override void TakeDamage(double amount, DamageType type)
        {
            //do things
        }

        public override EntityProperty GetProperty(string key)
        {
            return new EntityProperty(){Key = key, Value = StatMap[key]};
        }
    }
}
