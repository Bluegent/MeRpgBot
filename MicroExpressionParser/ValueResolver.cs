using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroExpressionParser
{
    using System.Data;
    using System.Net.Configuration;
    using System.Runtime.InteropServices;

    public class ValueResolverException : Exception
    {
        public ValueResolverException(String msg) : base(msg) { }
    }

    public class FunctionalNode
    {
        public MeVariable Value { get; set; }
        public List<FunctionalNode> Leaves { get; }

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

    public static class ValueResolver
    {

        public static FunctionalNode Convert(SyntacticNode node, Dictionary<String, double> variableMap)
        {
            FunctionalNode newNode = null;
            switch (node.Token.Type)
            {
                case TokenType.Function:
                    {
                        newNode = new FunctionalNode(new MeVariable()
                                                         {
                                                             Value= ParserConstants.Functions[node.Token.Value],
                                                             Type = VariableType.Function,
                                                         });
                        break;
                    }

                case TokenType.Operator:
                    {
                        newNode = new FunctionalNode(new MeVariable()
                                                         {
                                                             Value = ParserConstants.Operators[node.Token.Value],
                                                             Type = VariableType.Operator,
                                                         });
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
                        newNode = new FunctionalNode(new MeVariable()
                                                         {
                                                             Value = result,
                                                             Type = VariableType.NumericValue,
                                                         });
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

            switch (node.Value.Type)
            {
                case VariableType.Function:

                        List<MeVariable> parameters = new List<MeVariable>();
                        foreach (FunctionalNode subNode in node.Leaves)
                        {
                            parameters.Add(subNode.ToValue());
                        }

                        node.Value = node.Value.ToFunction().Execute(parameters.ToArray());
                        node.Leaves.Clear();
                        break;
                case VariableType.Entity:
                    //not implemented yet
                    break;
                case VariableType.Operator:
                    {
                        node.Value = node.Value.ToOperator().Operation(node.Leaves[0].ToValue(), node.Leaves[1].ToValue());
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
