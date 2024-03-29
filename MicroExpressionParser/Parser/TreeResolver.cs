﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGEngine.Parser
{
    using RPGEngine;

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


        public static MeNode ResolveGivenOperations(MeNode node, string[] operationNames, int index = 0)
        {
            for (int i = 0; i < node.Leaves.Count; ++i)
            {
                node.Leaves[i] = ResolveGivenOperations(node.Leaves[i], operationNames, i);
            }

            if (node.Value.Type == VariableType.Function || node.Value.Type == VariableType.Operator)
            {
                foreach (string key in operationNames)
                {
                    if (node.Value.GetString().Equals(key))
                        return node.Resolve();
                }
            }
            return node;

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
                        MeNode amt = new MeNode(node.Value.ToOperator().Execute(opParameters.ToArray()));
                        return amt;
                    }
                default:
                    {
                        return node;
                    }
            }
        }
    }
}
