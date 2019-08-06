namespace RPGEngine.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MicroExpressionParser.Parser;

    public static class Tokenizer
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
                    switch (prev.Type)
                    {
                        case TokenType.Function:
                            throw new Exception($"Found minus after function with no (, function:\"{prev.Value}\".");
                        case TokenType.Variable:
                        case TokenType.RightParen:
                            result.Add(new Token(char.ToString(c)));
                            break;
                        default:
                            current = "-";
                            break;
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
