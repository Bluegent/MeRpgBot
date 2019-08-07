using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGEngine.Core
{
    using MicroExpressionParser;

    public class MeNode
    {
        public MeVariable Value { get; set; }
        public List<MeNode> Leaves { get; }
        public MeNode Parent { get; set; }

        public MeNode(MeVariable value)
        {
            Leaves = new List<MeNode>();
            this.Value = value;
        }

        public double ToValue()
        {
            return Value.ToDouble();
        }
    }
}
