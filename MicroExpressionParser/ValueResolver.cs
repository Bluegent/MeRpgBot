using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroExpressionParser
{
    using System.Data;
    using System.Net.Configuration;

    public class ValueResolverException : Exception
    {
        public ValueResolverException(String msg) : base(msg) { }
    }
    public enum NodeType
    {
        Function,
        Value,
        Entity,
        Operator
    }

    public class FunctionalNode
    {
        public Object Value { get; set; }
        public NodeType Type { get; set; }
        public List<FunctionalNode> Leaves { get; }

        public FunctionalNode(Object value, NodeType type)
        {
            Leaves = new List<FunctionalNode>();
            this.Type = type;
            this.Value = value;
        }

        public double ToValue()
        {
            if (Type != NodeType.Value)
                throw new ValueResolverException("Attempted to retrieve invalid value from a node.");
            return (double)Value;
        }
    }

    public static class ValueResolver
    {

        public static FunctionalNode Convert(SyntacticNode node, Dictionary<String, double> variableMap)
        {
            FunctionalNode newNode = null;
            switch (node.Token.Type)
            {
                case TokenType.Function:
                    {
                        newNode = new FunctionalNode(ParserConstants.Functions[node.Token.Value], NodeType.Function);
                        break;
                    }

                case TokenType.Operator:
                    {
                        newNode = new FunctionalNode(ParserConstants.Operators[node.Token.Value], NodeType.Operator);
                        break;
                    }

                case TokenType.Variable:
                    {
                        double result;
                        bool success = Double.TryParse(node.Token.Value, out result);
                        if (!success)
                        {
                            if (variableMap.ContainsKey(node.Token.Value))
                                result = variableMap[node.Token.Value];
                            else
                                throw new ValueResolverException(
                                    "Attempted to convert unknown value: " + node.Token.Value + " .");
                        }
                        newNode = new FunctionalNode(result, NodeType.Value);
                        break;
                    }
            }

            foreach (SyntacticNode subNode in node.Parameters)
            {
                newNode.Leaves.Add(Convert(subNode, variableMap));
            }

            return newNode;
        }

        public static void ResolveNode(FunctionalNode node)
        {
            foreach (FunctionalNode subNode in node.Leaves)
            {
                ResolveNode(subNode);
            }

            switch (node.Type)
            {
                case NodeType.Function:
                    if (((Function)node.Value).IsMathematical())
                    {
                        List<double> parameters = new List<Double>();
                        foreach (FunctionalNode subNode in node.Leaves)
                        {
                            parameters.Add(subNode.ToValue());
                        }
                        node.Value = ((Function)node.Value).Operation(parameters.ToArray());
                        node.Type = NodeType.Value;
                        node.Leaves.Clear();
                    }
                    else
                    {
                        //not implemented yet
                    }
                    break;
                case NodeType.Entity:
                    //not implemented yet
                    break;
                case NodeType.Operator:
                    node.Value = ((Operator)node.Value).Operation(node.Leaves[0].ToValue(),node.Leaves[1].ToValue());
                    node.Type = NodeType.Value;
                    node.Leaves.Clear();
                    break;
            }
        }

        public static double Resolve(string expression, Dictionary<String, double> variableMap)
        {
            double result = 0;
            SyntacticNode tree = TreeBuilder.ExprToTree(expression);
            FunctionalNode resultNode = Convert(tree, variableMap);
            ResolveNode(resultNode);
            return resultNode.ToValue();
        }
    }
}
