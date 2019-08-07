namespace MicroExpressionParser.Parser
{
    using System;
    using System.Collections.Generic;

    using RPGEngine.Parser;

    public static class InfixToPostfix
    { 
    public static Token[] ExprToPostifx(string expression)
        {
            Token[] list = Tokenizer.Tokenize((expression));
            return ToPostfix(list);
        }

        private static void ShuntOperators(List<Token> postfix, Stack<Token> opStack, Operator op)
        {
            Token nextTok = opStack.Count == 0 ? null : opStack.Peek();
            while (nextTok != null &&
                nextTok.Type == TokenType.Operator &&
                ((op.LeftAsoc && op.Precedence <= ParserConstants.Operators[nextTok.Value].Precedence)
                || (op.Precedence < ParserConstants.Operators[nextTok.Value].Precedence))
                )
            {
                postfix.Add(opStack.Pop());
                nextTok = opStack.Count == 0 ? null : opStack.Peek();
            }
        }

        public static Token[] ToPostfix(Token[] infix)
        {
            List<Token> postFix = new List<Token>();
            Stack<Token> opStack = new Stack<Token>();
            Token lastFunction = null;
            Token prevTok = null;

            foreach (Token tok in infix)
            {
                switch (tok.Type)
                {
                    case TokenType.Variable:
                        {
                            postFix.Add(tok);
                            break;
                        }
                    case TokenType.Function:
                        {
                            opStack.Push(tok);
                            lastFunction = tok;
                            break;
                        }

                    case TokenType.Separator:
                        {
                            if (prevTok != null && prevTok.Type == TokenType.Operator)
                                throw new MeException("Missing parameter(s) for operator " + prevTok.Value + " .");
                            while (opStack.Count != 0 && opStack.Peek().Type != TokenType.LeftParen)
                            {
                                postFix.Add(opStack.Pop());
                            }
                            if (opStack.Count == 0)
                            {
                                if (lastFunction == null)
                                {
                                    throw new MeException("Unexpected separator Character.");
                                }
                                else
                                {
                                    throw new MeException("Error while parsing function " + lastFunction.Value + " .");
                                }
                            }
                            break;
                        }

                    case TokenType.Operator:
                        {
                            if (prevTok != null && (prevTok.Type == TokenType.Separator || prevTok.Type == TokenType.LeftParen))
                            {
                                throw new MeException("Missing parameter(s) for operator " + tok.Value + " .");
                            }

                            Operator op = ParserConstants.Operators[tok.Value];
                            ShuntOperators(postFix, opStack, op);
                            opStack.Push(tok);
                            break;
                        }
                    case TokenType.LeftParen:
                        {
                            if(prevTok != null)
                            {
                                if (prevTok.Type == TokenType.Function)
                                    postFix.Add(tok);
                            }
                            opStack.Push(tok);
                            break;
                        }
                    case TokenType.RightParen:
                        {
                            if (prevTok != null && prevTok.Type == TokenType.Operator)
                            {
                                throw new MeException("Missing parameter(s) for operator " + prevTok.Value + " .");
                            }
                            while(opStack.Count != 0 && opStack.Peek().Type != TokenType.LeftParen)
                            {
                                postFix.Add(opStack.Pop());
                            }
                            if(opStack.Count == 0)
                            {
                                throw new MeException("Mismatched parenthesis after" + prevTok.Value + " .");
                            }
                            opStack.Pop();
                            if(opStack.Count != 0  && opStack.Peek().Type == TokenType.Function)
                            {
                                postFix.Add(opStack.Pop());
                            }
                            break;
                        }
                        
                }
                prevTok = tok;    
            }
            while(opStack.Count != 0 )
            {
                Token tok = opStack.Pop();
                if (tok.Type == TokenType.LeftParen || tok.Type  == TokenType.RightParen)
                   throw new MeException("Mismatched parenthesis after" + prevTok.Value + " .");
                postFix.Add(tok);
            }
            return postFix.ToArray();
        }
    }
}
