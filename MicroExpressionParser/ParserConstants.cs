using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroExpressionParser
{
    public class ArgumentException : Exception
    {
        public ArgumentException(String message) : base(message) { }
    }

    public class Operator
    {
        public String Character { get; }
        public int Precedence { get; }
        public Func<double, double, double> Operation { get; }
        public bool LeftAsoc { get; }

        public bool Precedes(Operator other)
        {
            return this.Precedence > other.Precedence;
        }

        public Operator(String character, int precedence, bool leftAsoc, Func<double, double, double> operation)
        {
            this.Character = character;
            this.Precedence = precedence;
            this.Operation = operation;
            this.LeftAsoc = leftAsoc;
        }
    }

    public class Function
    {
        public String Name { get; }
        public Func<double[], double> Operation { get; }
        public Function(String name, Func<double[], double> operation)
        {
            this.Name = name;
            this.Operation = operation;
        }
        public static void ValidateParameters(int expected, double[] parameters)
        {
            if (expected == 0)
                return;
            if (parameters.Length != expected)
                throw new ArgumentException("Argument count does not match, got " + parameters.Length + " expected: " + expected);
        }
    }

    public class ParserConstants
    {
        public static Dictionary<String, Operator> Operators;
        public static Dictionary<String, Function> Functions;
        public const char PARAM_SEPARATOR = ',';
        public const char LEFT_PAREN = '(';
        public const char RIGHT_PAREN = ')';
        public static bool IsSeparator(String str)
        {
            if (str.Length != 1)
                return false;
            if (str[0] == PARAM_SEPARATOR)
                return true;
            return false;
        }

        public static bool IsFunction(String str)
        {
            return Functions.ContainsKey(str);
        }

        public static bool IsOperator(String str)
        {
            return Operators.ContainsKey(str);
        }

        public static bool IsLeftParen(String str)
        {
            if (str.Length != 1)
                return false;
            if (str[0] == LEFT_PAREN)
                return true;
            return false;
        }

        public static bool IsRightParen(String str)
        {
            if (str.Length != 1)
                return false;
            if (str[0] == RIGHT_PAREN)
                return true;
            return false;
        }

        public static void AddOp(Operator op)
        {
            Operators.Add(op.Character, op);
        }
        public static void AddFunc(Function func)
        {
            Functions.Add(func.Name, func);
        }
        public static void Init()
        {
            Operators = new Dictionary<string, Operator>();
            Functions = new Dictionary<String, Function>();

            Operator plus = new Operator("+", 1, true, (left, right) => left + right);
            AddOp(plus);
            Operator minus = new Operator("-", 1, true, (left, right) => left - right);
            AddOp(minus);
            Operator multiply = new Operator("*", 2, true, (left, right) => left * right);
            AddOp(multiply);
            Operator divide = new Operator("/", 2, true, (left, right) => left / right);
            AddOp(divide);

            Function max = new Function("MAX", values => values.Max());
            AddFunc(max);

            Function min = new Function("MIN", values => values.Min());
            AddFunc(min);

            Function abs = new Function("ABS", values =>
            {
                Function.ValidateParameters(1, values);
                return Math.Abs(values[0]);
            });
            AddFunc(abs);

            Function noNegative = new Function("NONNEG", values =>
            {
                Function.ValidateParameters(1, values);
                return values[0] >= 0 ? values[0] : 0;
            });
            AddFunc(noNegative);

            Function random = new Function("RANDOM", values =>
            {
                Function.ValidateParameters(2, values);
                return new Random().Next((int)values[0], (int)values[1]);
            });

            AddFunc(random);
        }
    }
}
