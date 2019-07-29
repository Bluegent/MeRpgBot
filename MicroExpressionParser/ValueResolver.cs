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

    public class FunctionalNode
    {
        public Object Value { get; set; }
        public VariableType Type { get; set; }
        public List<FunctionalNode> Leaves { get; }

        public FunctionalNode(Object value, VariableType type)
        {
            Leaves = new List<FunctionalNode>();
            this.Type = type;
            this.Value = value;
        }

        public double ToValue()
        {
            if (Type != VariableType.NumericValue && Type != VariableType.Variable)
                throw new ValueResolverException("Attempted to retrieve invalid value from node "+this.Type+" .");
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
                        newNode = new FunctionalNode(ParserConstants.Functions[node.Token.Value], VariableType.Function);
                        break;
                    }

                case TokenType.Operator:
                    {
                        newNode = new FunctionalNode(ParserConstants.Operators[node.Token.Value], VariableType.Operator);
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
                        newNode = new FunctionalNode(result, VariableType.Variable);
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
                case VariableType.Function:
                    if (((BaseFunction)node.Value).IsMathematical())
                    {
                        List<double> parameters = new List<Double>();
                        foreach (FunctionalNode subNode in node.Leaves)
                        {
                            parameters.Add(subNode.ToValue());
                        }
                        node.Value = ((MathFunction)node.Value).Execute(parameters.ToArray());
                        node.Type = VariableType.Variable;
                        node.Leaves.Clear();
                    }
                    else
                    {
                        //not implemented yet
                    }
                    break;
                case VariableType.Entity:
                    //not implemented yet
                    break;
                case VariableType.Operator:
                    {
                        node.Value = ((Operator)node.Value).Operation(node.Leaves[0].ToValue(), node.Leaves[1].ToValue());
                        node.Type = VariableType.Variable;
                        node.Leaves.Clear();
                        break;
                    }
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
