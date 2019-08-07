using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGEngine.Core
{
    using MicroExpressionParser;

    using RPGEngine.Parser;

    public class MeNode
    {
        public MeVariable Value { get; set; }
        public List<MeNode> Leaves { get; }
        public MeNode Parent { get; set; }

        public MeNode(MeVariable value)
        {
            Leaves = new List<MeNode>();
            Value = value;
        }

        public MeNode Resolve()
        {
            return TreeResolver.Resolve(this);
        }
    }
}
