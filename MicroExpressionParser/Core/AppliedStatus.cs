using System;
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
        public double[] NumericValues { get; set; }
        public long RemovalTime { get; set; }
        public long LastTick { get; set; }
        public Entity Source { get; set; }
        public long Interval { get; set; }
    }
}
