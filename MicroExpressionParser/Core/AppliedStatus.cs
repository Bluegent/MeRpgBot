using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGEngine.Core
{
    using MicroExpressionParser;

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
