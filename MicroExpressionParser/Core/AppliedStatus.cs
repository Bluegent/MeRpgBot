using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RPGEngine.Game;
using RPGEngine.Entities;

namespace RPGEngine.Core
{


    public class AppliedStatus
    {
        public StatusTemplate Template { get; set; }
        public List<StatModifier> MyMods { get; private set; }
        public double[] NumericValues { get; set; }
        public long RemovalTime { get; set; }
        public long LastTick { get; set; }
        public Entity Source { get; set; }
        public long Interval { get; set; }

        public double TotalDamamge { get; set; }
        public double TotalHeal { get; set; }

        public AppliedStatus()
        {
            MyMods = new List<StatModifier>();
            TotalDamamge = 0;
            TotalHeal = 0;
        }

        public void Remove(Entity target)
        {
            foreach (StatModifier mod in MyMods)
            {
                target.Attributes[mod.StatKey].Modifiers.Remove(mod);
            }
        }
    }
}
