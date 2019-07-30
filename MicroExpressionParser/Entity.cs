using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroExpressionParser
{
    public abstract class Entity
    {
        public string Name { get; set; }
        public Dictionary<string, double> StatMap { get; set; }

        public abstract void TakeDamage(double amount, DamageType type);

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
    }
}
