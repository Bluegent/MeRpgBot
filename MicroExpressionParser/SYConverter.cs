using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroExpressionParser
{
    public class SYConverter
    {
        public class ConverterException : Exception
        {
            public ConverterException(String msg) : base(msg) { }
        }

        private static void shuntOperators(List<Token> postfix, Stack<Token> opStack, Operator op)
        {
            Token nextTok = opStack.Count == 0 ? null : opStack.Peek();
            while (nextTok != null &&
                nextTok.type == TokenType.OPERATOR &&
                ((op.leftAsoc && op.precedence <= ParserConstants.operators[nextTok.value].precedence)
                || (op.precedence < ParserConstants.operators[nextTok.value].precedence))
                )
            {
                postfix.Add(opStack.Pop());
                nextTok = opStack.Count == 0 ? null : opStack.Peek();
            }
        }

        public static void debug(List<Token> postfix, Stack<Token> stack, Token lf, Token pt, Token toke)
        {
            System.Console.Write("Expr: ");
            foreach(Token tok in postfix)
            {
                System.Console.Write(tok.value + " ");
            }
            System.Console.Write("\nStack:");
            foreach (Token tok in stack)
            {
                System.Console.Write(tok.value + " ");
            }
            System.Console.WriteLine("\nCT= "+toke.value+" LF=" + (lf == null? "-" : lf.value) + " PT = " + (pt == null? "-": pt.value)+"\n");
        }

        public static Token[] toPostfix(Token[] infix)
        {
            List<Token> postFix = new List<Token>();
            Stack<Token> opStack = new Stack<Token>();
            Token lastFunction = null;
            Token prevTok = null;

            foreach (Token tok in infix)
            {
                switch (tok.type)
                {
                    case TokenType.VARIABLE:
                        {
                            postFix.Add(tok);
                            break;
                        }
                    case TokenType.FUNCTION:
                        {
                            opStack.Push(tok);
                            lastFunction = tok;
                            break;
                        }

                    case TokenType.SEPARATOR:
                        {
                            if (prevTok != null && prevTok.type == TokenType.OPERATOR)
                                throw new ConverterException("Missing parameter(s) for operator " + prevTok.value + " .");
                            while (opStack.Count != 0 && opStack.Peek().type != TokenType.LEFT_PAREN)
                            {
                                postFix.Add(opStack.Pop());
                            }
                            if (opStack.Count == 0)
                            {
                                if (lastFunction == null)
                                {
                                    throw new ConverterException("Unexpected separator character.");
                                }
                                else
                                {
                                    throw new ConverterException("Error while parsing function " + lastFunction.value + " .");
                                }
                            }
                            break;
                        }

                    case TokenType.OPERATOR:
                        {
                            if (prevTok != null && (prevTok.type == TokenType.SEPARATOR || prevTok.type == TokenType.LEFT_PAREN))
                            {
                                throw new ConverterException("Missing parameter(s) for operator " + tok.value + " .");
                            }

                            Operator op = ParserConstants.operators[tok.value];
                            shuntOperators(postFix, opStack, op);
                            opStack.Push(tok);
                            break;
                        }
                    case TokenType.LEFT_PAREN:
                        {
                            if(prevTok != null)
                            {
                                if (prevTok.type == TokenType.FUNCTION)
                                    postFix.Add(tok);
                            }
                            opStack.Push(tok);
                            break;
                        }
                    case TokenType.RIGHT_PAREN:
                        {
                            if (prevTok != null && prevTok.type == TokenType.OPERATOR)
                            {
                                throw new ConverterException("Missing parameter(s) for operator " + prevTok.value + " .");
                            }
                            while(opStack.Count != 0 && opStack.Peek().type != TokenType.LEFT_PAREN)
                            {
                                postFix.Add(opStack.Pop());
                            }
                            if(opStack.Count == 0)
                            {
                                throw new ConverterException("Missmatched parenthesis after" + prevTok.value + " .");
                            }
                            opStack.Pop();
                            if(opStack.Count != 0  && opStack.Peek().type == TokenType.FUNCTION)
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
                if (tok.type == TokenType.LEFT_PAREN || tok.type  == TokenType.RIGHT_PAREN)
                   throw new ConverterException("Missmatched parenthesis after" + prevTok.value + " .");
                postFix.Add(tok);
            }
            return postFix.ToArray();
        }
    }
}
