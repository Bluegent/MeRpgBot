namespace RPGEngine.Parser
{
    using System;
    using System.Collections.Generic;

    using MicroExpressionParser;
    using MicroExpressionParser.Parser;

    public class TokenNode
    {
        public Token Token { get; }
        public List<TokenNode> Parameters { get; }

        public TokenNode(Token token)
        {
            Token = token;
            Parameters = new List<TokenNode>();
        }
    }

    public static class TreeBuilder
    {

        public static TokenNode ExprToTree(string expression)
        {
            Token[] postFix = InfixToPostfix.ExprToPostifx(expression);
            return MakeTree(postFix);
        }

        public static TokenNode MakeTree(Token[] postfix)
        {
            Stack<TokenNode> nodeStack = new Stack<TokenNode>();
            foreach(Token tok in postfix)
            {
                switch (tok.Type)
                {
                    case TokenType.LeftParen:
                        {
                            nodeStack.Push(new TokenNode(tok));
                            break;
                        }

                    case TokenType.Operator:
                        {
                            TokenNode node = new TokenNode(tok);
                            for (int i = 0; i < ParserConstants.Operators[tok.Value].OperatorCount;++i)
                            {
                                if(nodeStack.Count == 0 || nodeStack.Peek().Token.Type == TokenType.LeftParen)
                                    throw new MeException("Parameter(s) missing for operator " + tok.Value + " .");
                                node.Parameters.Add(nodeStack.Pop());
                            }
                            nodeStack.Push(node);
                            node.Parameters.Reverse();
                            break;
                        }
                    case TokenType.Function:
                        {
                            TokenNode node = new TokenNode(tok);
                            while (nodeStack.Count != 0 && nodeStack.Peek().Token.Type != TokenType.LeftParen)
                            {
                                node.Parameters.Add(nodeStack.Pop());
                            }

                            if (nodeStack.Count == 0)
                            {
                                throw new MeException("No parenthesis found for function "+tok.Value+" .");
                            }
                            nodeStack.Pop();
                            node.Parameters.Reverse();
                            nodeStack.Push(node);
                            break;
                        }

                    case TokenType.Variable:
                        {
                            nodeStack.Push(new TokenNode(tok));
                            break;
                        }
                }
            }
            return nodeStack.Pop();
        }
    }
}
