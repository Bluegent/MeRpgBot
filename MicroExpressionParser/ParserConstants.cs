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
        public String character { get; }
        public int precedence { get; }
        public Func<double, double, double> operation { get; }
        public bool leftAsoc { get; }

        public bool precedes(Operator other)
        {
            return this.precedence > other.precedence;
        }

        public Operator(String character, int precedence, bool leftAsoc, Func<double, double, double> operation)
        {
            this.character = character;
            this.precedence = precedence;
            this.operation = operation;
            this.leftAsoc = leftAsoc;
        }
    }

    public class Function
    {
        public String name { get; }
        public Func<double[], double> operation { get; }
        public Function(String name, Func<double[], double> operation)
        {
            this.name = name;
            this.operation = operation;
        }
        public static void validateParameters(int expected, double[] parameters)
        {
            if (expected == 0)
                return;
            if (parameters.Length != expected)
                throw new ArgumentException("Argument count does not match, got " + parameters.Length + " expected: " + expected);
        }
    }

    public class ParserConstants
    {
        public static Dictionary<String, Operator> operators;
        public static Dictionary<String, Function> functions;
        public const char paramSeparator = ',';
        public const char leftParen = '(';
        public const char rightParen = ')';
        public static bool isSeparator(String str)
        {
            if (str.Length != 1)
                return false;
            if (str[0] == paramSeparator)
                return true;
            return false;
        }

        public static bool isFunction(String str)
        {
            return functions.ContainsKey(str);
        }

        public static bool isOperator(String str)
        {
            return operators.ContainsKey(str);
        }

        public static bool isLeftParen(String str)
        {
            if (str.Length != 1)
                return false;
            if (str[0] == leftParen)
                return true;
            return false;
        }

        public static bool isRightParen(String str)
        {
            if (str.Length != 1)
                return false;
            if (str[0] == rightParen)
                return true;
            return false;
        }

        public static void addOp(Operator op)
        {
            operators.Add(op.character, op);
        }
        public static void addFunc(Function func)
        {
            functions.Add(func.name, func);
        }
        public static void init()
        {
            operators = new Dictionary<string, Operator>();
            functions = new Dictionary<String, Function>();

            Operator plus = new Operator("+", 1, true, (left, right) => left + right);
            addOp(plus);
            Operator minus = new Operator("-", 1, true, (left, right) => left - right);
            addOp(minus);
            Operator multiply = new Operator("*", 2, true, (left, right) => left * right);
            addOp(multiply);
            Operator divide = new Operator("/", 2, true, (left, right) => left / right);
            addOp(divide);

            Function max = new Function("MAX", values => values.Max());
            addFunc(max);

            Function min = new Function("MIN", values => values.Min());
            addFunc(min);

            Function abs = new Function("ABS", values =>
            {
                Function.validateParameters(1, values);
                return Math.Abs(values[0]);
            });
            addFunc(abs);

            Function noNegative = new Function("NONNEG", values =>
            {
                Function.validateParameters(1, values);
                return values[0] >= 0 ? values[0] : 0;
            });
            addFunc(noNegative);

            Function random = new Function("RANDOM", values =>
            {
                Function.validateParameters(2, values);
                return new Random().Next((int)values[0], (int)values[1]);
            });

            addFunc(random);
        }
    }
}
