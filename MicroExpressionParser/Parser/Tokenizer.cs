namespace RPGEngine.Parser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    using RPGEngine.Language;

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
            int i = 0;
            while (i< sanitized.Length)
            {
                char c = sanitized[i];
                if (!Definer.Get().IsSpecialChar(c))
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
                    //eventually consider -=
                }
                else
                {
                    if (current.Length != 0)
                    {
                        result.Add(new Token(current));
                        current = "";                       
                    }
                    if (i < sanitized.Length - 1 &&Definer.Get().IsOperatorChar(c) &&Definer.Get().IsOperatorChar(sanitized[i + 1]))
                    {
                        string possibleOp = char.ToString(c) + sanitized[i + 1];
                        if (Definer.Get().IsOperator(possibleOp))
                        {
                            result.Add(new Token(possibleOp));
                            ++i;
                        }

                       
                    }
                    else
                    {
                        result.Add(new Token(char.ToString(c)));
                    }
                   
                }

                ++i;
            }
            if (current.Length != 0)
                result.Add(new Token(current));
            return result.ToArray();
        }
    }
}
