namespace RPGEngine.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MicroExpressionParser.Parser;

    public class Tokenizer
    {
        private static string Sanitize(string expression)
        {
            return expression.Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim();
        }

        public static Token[] Tokenize(string expression)
        {
            List<Token> result = new List<Token>();
            string sanitized = Sanitize(expression);            
            string current = "";
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
                        result.Add(new Token(char.ToString(c)));
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
                    result.Add(new Token(char.ToString(c)));
                }
            }
            if (current.Length != 0)
                result.Add(new Token(current));
            return result.ToArray();
        }
    }
}
