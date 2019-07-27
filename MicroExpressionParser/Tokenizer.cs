using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroExpressionParser
{
    public enum TokenType
    {
        VARIABLE, LEFT_PAREN, RIGHT_PAREN, SEPARATOR, FUNCTION, OPERATOR
    }

    public class Token
    {
        public TokenType type { get; set; }
        public String value { get; set; }
        public static TokenType getType(String str)
        {
            if (ParserConstants.isFunction(str))
                return TokenType.FUNCTION;
            if (ParserConstants.isOperator(str))
                return TokenType.OPERATOR;
            if (ParserConstants.isLeftParen(str))
                return TokenType.LEFT_PAREN;
            if (ParserConstants.isRightParen(str))
                return TokenType.RIGHT_PAREN;
            if (ParserConstants.isSeparator(str))
                return TokenType.SEPARATOR;
            return TokenType.VARIABLE;
        }
        public Token(String value)
        {
            this.value = value;
            this.type = getType(value);
        }
    }

    public class Tokenizer
    {

        public static String sanitize(String expression)
        {
            return expression.Replace(" ", "").Replace("\n", "").Replace("\r", "").Replace("\t", "").Trim(); ;
        }

        public static Token[] tokenize(String expression)
        {
            List<Token> result = new List<Token>();
            String sanitized = sanitize(expression);            
            String current = "";
            foreach (char c in sanitized)
            {
                if (Char.IsLetterOrDigit(c))
                {
                    current += c;
                }
                else if(c == '-')
                {
                    Token prev = result.Last();
                    if (prev.type == TokenType.FUNCTION)
                        throw new Exception("Found minus after function with no (, function: " + prev.value);
                    else if (prev.type == TokenType.VARIABLE || prev.type == TokenType.RIGHT_PAREN)
                        result.Add(new Token(Char.ToString(c)));
                    else
                        current = "-";
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
