namespace RPGEngine.Parser
{
    using System;
    using System.Collections.Generic;

    using MicroExpressionParser;
    using RPGEngine.Core;
    using RPGEngine.Language;

    public static class TreeConverter
    {

        public static MeNode Build(MeNode parent, TokenNode node, IGameEngine engine)
        {
            MeNode newNode = null;
            switch (node.Token.Type)
            {
                case TokenType.Function:
                    {
                        newNode = new MeNode(new MeVariable()
                        {
                            Value = Definer.Get().Functions[node.Token.Value],
                            Type = VariableType.Function
                        });
                        break;
                    }

                case TokenType.Operator:
                    {
                        newNode = new MeNode(new MeVariable()
                        {
                            Value = Definer.Get().Operators[node.Token.Value],
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
                newNode.Leaves.Add(Build(newNode, subNode, engine));
            }

            newNode.Parent = parent;
            return newNode;
        }

        public static MeNode Build(Token[] tokens, IGameEngine engine)
        {
            TokenNode tree = TreeBuilder.MakeTree(InfixToPostfix.ToPostfix(tokens));
            return Build(null, tree, engine);
        }

        public static MeNode Build(string expression, IGameEngine engine)
        {
            return Build(Tokenizer.Tokenize(expression), engine);
        }
    }
}
