namespace RPGEngine.Parser
{
    using System;
    using System.Collections.Generic;

    using MicroExpressionParser;
    using MicroExpressionParser.Parser;

    using RPGEngine.Core;

    public static class TreeConverter
    {

        public static MeNode Convert(MeNode parent, TokenNode node, IGameEngine engine)
        {
            MeNode newNode = null;
            switch (node.Token.Type)
            {
                case TokenType.Function:
                    {
                        newNode = new MeNode(new MeVariable()
                        {
                            Value = ParserConstants.Functions[node.Token.Value],
                            Type = VariableType.Function
                        });
                        break;
                    }

                case TokenType.Operator:
                    {
                        newNode = new MeNode(new MeVariable()
                        {
                            Value = ParserConstants.Operators[node.Token.Value],
                            Type = VariableType.Operator
                        });
                        break;
                    }
                case TokenType.Variable:
                    {
                        double result = 0;
                        bool success = double.TryParse(node.Token.Value, out result);
                        if (success)
                        {
                            newNode = new MeNode(
                                new MeVariable() { Value = result, Type = VariableType.NumericValue });
                            break;
                        }

                        if (node.Token.Value.StartsWith("$"))
                        {
                            newNode = new MeNode(
                              new MeVariable() { Value = node.Token.Value, Type = VariableType.PlaceHolder });
                            break;
                        }
                        Entity tryEntity = engine.GetEntityByKey(node.Token.Value);
                        if (tryEntity != null)
                        {
                            newNode = new MeNode(
                                new MeVariable() { Value = tryEntity, Type = VariableType.Entity });
                            break;
                        }

                        DamageType tryDamageType = engine.GeDamageType(node.Token.Value);
                        if (tryDamageType != null)
                        {
                            newNode = new MeNode(
                                new MeVariable() { Value = tryDamageType, Type = VariableType.DamageType });
                            break;
                        }

                        newNode = new MeNode(
                                new MeVariable() { Value = node.Token.Value, Type = VariableType.String });
                        break;
                    }
            }

            foreach (TokenNode subNode in node.Parameters)
            {
                newNode.Leaves.Add(Convert(newNode, subNode, engine));
            }

            newNode.Parent = parent;
            return newNode;
        }

        public static MeNode ResolveNode(MeNode node, int index)
        {
            for(int i=0;i<node.Leaves.Count;++i)
            {
                node.Leaves[i] = ResolveNode(node.Leaves[i],i);
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

        public static MeNode BuildTree(Token[] tokens, IGameEngine engine)
        {
            TokenNode tree = TreeBuilder.MakeTree(InfixToPostfix.ToPostfix(tokens));
            return Convert(null, tree, engine);
        }

        public static MeNode BuildTree(string expression, IGameEngine engine)
        {
            return BuildTree(Tokenizer.Tokenize(expression), engine);
        }

        public static MeNode ResolveTree(Token[] tokens, IGameEngine engine)
        {
            return ResolveNode(BuildTree(tokens, engine), 0);
        }

        public static MeNode ResolveTree(string expression, IGameEngine engine)
        {
            return ResolveTree(Tokenizer.Tokenize(expression),engine);
        }
    }
}
