using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroExpressionParser
{
    public class TreeBuilderException : Exception
    {
        public TreeBuilderException(String msg) : base(msg) { }
    }

    public class SyntacticNode
    {
        public Token Token { get; }
        public List<SyntacticNode> Parameters { get; }

        public SyntacticNode(Token token)
        {
            this.Token = token;
            this.Parameters = new List<SyntacticNode>();
        }
    }

    public static class TreeBuilder
    {

        public static SyntacticNode ExprToTree(String expression)
        {
            Token[] postFix = SYConverter.ExprToPostifx(expression);
            return MakeTree(postFix);
        }

        public static SyntacticNode MakeTree(Token[] postfix)
        {
            Stack<SyntacticNode> nodeStack = new Stack<SyntacticNode>();
            foreach(Token tok in postfix)
            {
                switch (tok.Type)
                {
                    case TokenType.LeftParen:
                        {
                            nodeStack.Push(new SyntacticNode(tok));
                            break;
                        }

                    case TokenType.Operator:
                        {
                            SyntacticNode node = new SyntacticNode(tok);
                            for (int i = 0; i < ParserConstants.Operators[tok.Value].OperatorCount;++i)
                            {
                                if(nodeStack.Count == 0 || nodeStack.Peek().Token.Type == TokenType.LeftParen)
                                    throw new TreeBuilderException("Paramether(s) missing for operator " + tok.Value + " .");
                                node.Parameters.Add(nodeStack.Pop());
                            }
                            nodeStack.Push(node);
                            node.Parameters.Reverse();
                            break;
                        }
                    case TokenType.Function:
                        {
                            SyntacticNode node = new SyntacticNode(tok);
                            while (nodeStack.Count != 0 && nodeStack.Peek().Token.Type != TokenType.LeftParen)
                            {
                                node.Parameters.Add(nodeStack.Pop());
                            }

                            if (nodeStack.Count == 0)
                            {
                                throw new TreeBuilderException("No parenthesis found for function "+tok.Value+" .");
                            }
                            nodeStack.Pop();
                            node.Parameters.Reverse();
                            nodeStack.Push(node);
                            break;
                        }

                    case TokenType.Variable:
                        {
                            nodeStack.Push(new SyntacticNode(tok));
                            break;
                        }
                }
            }
            return nodeStack.Pop();
        }
    }
}
