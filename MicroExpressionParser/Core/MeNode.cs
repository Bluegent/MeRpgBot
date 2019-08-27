using System.Collections.Generic;
using System.Text;


namespace RPGEngine.Core
{
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

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(Value.ToString());
            if (Leaves.Count != 0)
            {
                builder.Append("(");
                foreach (MeNode node in Leaves)
                {
                    builder.Append(node.ToString());
                    builder.Append(", ");
                }

                builder.Remove(builder.Length - 2, 2);
                builder.Append(")");
            }
            return builder.ToString();
        }
    }
}
