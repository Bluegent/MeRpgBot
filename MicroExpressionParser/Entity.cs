using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroExpressionParser
{

    class Entity
    {
        public String Name { get; set; }
        public Dictionary<string, double> StatMap { get; set; }

        public Entity()
        {
            // static 
            StatMap.Add("CHP", 100);
            StatMap.Add("MHP", 100);
            
            //dynamic
            StatMap.Add("STR", 5);
            StatMap.Add("INT", 5);
            StatMap.Add("AGI", 5);

        }

        public void ModifyValue(String stat, double amount)
        {
            StatMap[stat] += amount;
        }
    }
}
