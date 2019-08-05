using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroExpressionParser
{
    using System.Data;
    using System.Net.Configuration;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    public class ValueResolverException : Exception
    {
        public ValueResolverException(string msg) : base(msg) { }
    }

    public class FunctionalNode
    {
        public MeVariable Value { get; set; }
        public List<FunctionalNode> Leaves { get; }
        public FunctionalNode Parent { get; set; }

        public FunctionalNode(MeVariable value)
        {
            Leaves = new List<FunctionalNode>();
            this.Value = value;
        }

        public double ToValue()
        {
            return Value.ToDouble();
        }
    }

    public static class FunctionalTreeConverter
    {

        public static FunctionalNode Convert(FunctionalNode parent, SyntacticNode node, IGameEngine engine)
        {
            FunctionalNode newNode = null;
            switch (node.Token.Type)
            {
                case TokenType.Function:
                    {
                        newNode = new FunctionalNode(new MeVariable()
                        {
                            Value = ParserConstants.Functions[node.Token.Value],
                            Type = VariableType.Function
                        });
                        break;
                    }

                case TokenType.Operator:
                    {
                        newNode = new FunctionalNode(new MeVariable()
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
                            newNode = new FunctionalNode(
                                new MeVariable() { Value = result, Type = VariableType.NumericValue });
                            break;
                        }

                        Entity tryEntity = engine.GetEntityByKey(node.Token.Value);
                        if (tryEntity != null)
                        {
                            newNode = new FunctionalNode(
                                new MeVariable() { Value = tryEntity, Type = VariableType.Entity });
                            break;
                        }

                        DamageType tryDamageType = engine.GeDamageType(node.Token.Value);
                        if (tryDamageType != null)
                        {
                            newNode = new FunctionalNode(
                                new MeVariable() { Value = tryDamageType, Type = VariableType.DamageType });
                            break;
                        }

                        newNode = new FunctionalNode(
                                new MeVariable() { Value = node.Token.Value, Type = VariableType.String });
                        break;
                    }
            }

            foreach (SyntacticNode subNode in node.Parameters)
            {
                newNode.Leaves.Add(Convert(newNode, subNode, engine));
            }

            newNode.Parent = parent;
            return newNode;
        }

        public static FunctionalNode ResolveNode(FunctionalNode node, int index)
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
                    foreach (FunctionalNode subNode in node.Leaves)
                    {
                        parameters.Add(subNode.Value);
                    }
                    return new FunctionalNode(new MeFunction(node.Value, parameters.ToArray())); ;
                }
            }
            switch (node.Value.Type)
            {
                case VariableType.Function:

                    List<MeVariable> parameters = new List<MeVariable>();
                    foreach (FunctionalNode subNode in node.Leaves)
                    {
                        parameters.Add(subNode.Value);
                    }
                    return new FunctionalNode(node.Value.ToFunction().Execute(parameters.ToArray()));
                case VariableType.Operator:
                    {

                        List<MeVariable> opParameters = new List<MeVariable>();
                        foreach (FunctionalNode subNode in node.Leaves)
                        {
                            opParameters.Add(subNode.Value);
                        }
                        return new FunctionalNode(node.Value.ToOperator().Execute(opParameters.ToArray()));
                    }
                default:
                    {
                        return node;
                    }
            }
        }

        public static FunctionalNode BuildTree(Token[] tokens, IGameEngine engine)
        {
            SyntacticNode tree = TreeBuilder.MakeTree(SYConverter.ToPostfix(tokens));
            return Convert(null, tree, engine);
        }

        public static FunctionalNode BuildTree(string expression, IGameEngine engine)
        {
            return BuildTree(Tokenizer.Tokenize(expression), engine);
        }

        public static FunctionalNode ResolveTree(Token[] tokens, IGameEngine engine)
        {
            return ResolveNode(BuildTree(tokens, engine), 0); ;
        }

        public static FunctionalNode ResolveTree(string expression, IGameEngine engine)
        {
            return ResolveTree(Tokenizer.Tokenize(expression),engine);
        }
    }
}
