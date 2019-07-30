using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroExpressionParser
{
    public enum TokenType
    {
        Variable, LeftParen, RightParen, Separator, Function, Operator
    }

    public class Token
    {
        public TokenType Type { get; set; }
        public String Value { get; set; }
        public static TokenType GetType(String str)
        {
            if (ParserConstants.IsFunction(str))
                return TokenType.Function;
            if (ParserConstants.IsOperator(str))
                return TokenType.Operator;
            if (ParserConstants.IsLeftParen(str))
                return TokenType.LeftParen;
            if (ParserConstants.IsRightParen(str))
                return TokenType.RightParen;
            if (ParserConstants.IsSeparator(str))
                return TokenType.Separator;
            return TokenType.Variable;
        }
        public Token(String value)
        {
            this.Value = value;
            this.Type = GetType(value);
        }
    }

    public class Tokenizer
    {


        private static String Sanitize(String expression)
        {
            return expression.Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim();
        }

        public static Token[] Tokenize(String expression)
        {
            List<Token> result = new List<Token>();
            String sanitized = Sanitize(expression);            
            String current = "";
            foreach (char c in sanitized)
            {
                if (!ParserConstants.IsSpecialChar(c))
                {
                    current += c;
                }
                else if(c == '-')
                {
                    if (current.Length != 0)
                    {
                        Token testToken = new Token(current);
                        if (testToken.Type == TokenType.Variable)
                        {
                            result.Add(testToken);
                            current = "";
                        }
                    }
                    Token prev = result.Last();
                    if (prev.Type == TokenType.Function)
                        throw new Exception("Found minus after function with no (, function: " + prev.Value);
                    else if (prev.Type == TokenType.Variable || prev.Type == TokenType.RightParen)
                    {
                        result.Add(new Token(Char.ToString(c)));
                    }
                    else
                    {
                        
                        current = "-";
                    }
                }
                else
                {
                    if (current.Length != 0)
                    {
                        result.Add(new Token(current));
                        current = "";                       
                    }
                    result.Add(new Token(Char.ToString(c)));
                }
            }
            if (current.Length != 0)
                result.Add(new Token(current));
            return result.ToArray();
        }
    }
}
