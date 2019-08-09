using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGEngine.Parser
{
    using MicroExpressionParser;

    using RPGEngine.Core;

    public class TreeResolver
    {
        public static MeNode Resolve(Token[] tokens, IGameEngine engine)
        {
            return Resolve(TreeConverter.Build(tokens, engine), 0);
        }

        public static MeNode Resolve(string expression, IGameEngine engine)
        {
            return Resolve(Tokenizer.Tokenize(expression), engine);
        }


        public static MeNode ResolveGivenFunction(MeNode node, string functionName, int index = 0)
        {
            for (int i = 0; i < node.Leaves.Count; ++i)
            {
                node.Leaves[i] = ResolveGivenFunction(node.Leaves[i], functionName, i);
            }

            if (node.Value.Type == VariableType.Function && node.Value.ToFunction().Name.Equals(functionName))
            {
                return node.Resolve();
            }
            else
            {
                return node;
            }
        }

        public static MeNode Resolve(MeNode node, int index = 0)
        {
            for (int i = 0; i < node.Leaves.Count; ++i)
            {
                node.Leaves[i] = Resolve(node.Leaves[i], i);
            }

            if (node.Parent != null && node.Parent.Value.Type == VariableType.Function)
            {
                if (!node.Parent.Value.ToFunction().ExecuteSubNode(index))
                {
                    List<MeVariable> parameters = new List<MeVariable>();
                    foreach (MeNode subNode in node.Leaves)
                    {
                        parameters.Add(subNode.Value);
                    }
                    return new MeNode(new MeFunction(node.Value, parameters.ToArray()));
                }
            }
            switch (node.Value.Type)
            {
                case VariableType.Function:
                    List<MeVariable> parameters = new List<MeVariable>();
                    foreach (MeNode subNode in node.Leaves)
                    {
                        parameters.Add(subNode.Value);
                    }
                    return new MeNode(node.Value.ToFunction().Execute(parameters.ToArray()));
                case VariableType.Operator:
                    {

                        List<MeVariable> opParameters = new List<MeVariable>();
                        foreach (MeNode subNode in node.Leaves)
                        {
                            opParameters.Add(subNode.Value);
                        }
                        return new MeNode(node.Value.ToOperator().Execute(opParameters.ToArray()));
                    }
                default:
                    {
                        return node;
                    }
            }
        }
    }
}
