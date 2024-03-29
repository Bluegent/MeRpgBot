﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGEngine.Parser
{
    using RPGEngine.Language;

    public enum TokenType
    {
        Variable, LeftParen, RightParen, Separator, Function, Operator
    }

    public class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }
        public static TokenType GetType(string str)
        {
            if (Definer.Instance().IsFunction(str))
                return TokenType.Function;
            if (Definer.Instance().IsOperator(str))
                return TokenType.Operator;
            if (Definer.Instance().IsLeftParen(str))
                return TokenType.LeftParen;
            if (Definer.Instance().IsRightParen(str))
                return TokenType.RightParen;
            if (Definer.Instance().IsSeparator(str))
                return TokenType.Separator;
            return TokenType.Variable;
        }
        public Token(string value)
        {
            Value = value;
            Type = GetType(value);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
