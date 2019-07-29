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

        public int getOperandCount()
        {
            return 2;
        }

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

    public abstract class BaseFunction
    {
        public String Name { get; }
        protected Object innerFunction;
        public BaseFunction(String name, Object operation)
        {
            this.Name = name;
            this.innerFunction = operation;
        }
        public static void ValidateParameters(int expected, double[] parameters)
        {
            if (expected == 0)
                return;
            if (parameters.Length != expected)
                throw new ArgumentException("Argument count does not match, got " + parameters.Length + " expected: " + expected);
        }

        public abstract bool IsMathematical();
    }

    public class MathFunction : BaseFunction
    {
        public MathFunction(String name, Func<double[], double> operation) : base(name, operation)
        {
        }

        public double Execute(double[] values)
        {
            return ((Func<double[], double>)innerFunction).Invoke(values);
        }

        public override bool IsMathematical()
        {
            return true;
        }
    }

    public class NonMathFunction : BaseFunction
    {
        public NonMathFunction(string name, Func<MeVariable[], MeVariable> operation) : base(name, operation)
        {
        }

        public static void ValidateParameters(int expected, MeVariable[] parameters)
        {
            if (expected == 0)
                return;
            if (parameters.Length != expected)
                throw new ArgumentException("Argument count does not match, got " + parameters.Length + " expected: " + expected);
        }

        public static void ValidateParameter(MeVariable parameter, int count, VariableType expected)
        {
            if (parameter.Type != expected)
                throw new ArgumentException("Argument " + count + " is the wrong type (" + parameter.Type.ToString() + "). Expected: " + expected.ToString());
        }

        public void Execute(MeVariable[] variables)
        {
            ((Func<MeVariable[], MeVariable>)innerFunction).Invoke(variables);
        }

        public override bool IsMathematical()
        {
            return false;
        }
    }

    public class ParserConstants
    {
        public static Dictionary<String, Operator> Operators;
        public static Dictionary<String, BaseFunction> Functions;
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
        public static void AddFunc(BaseFunction func)
        {
            Functions.Add(func.Name, func);
        }
        public static void Init()
        {
            Operators = new Dictionary<string, Operator>();
            Functions = new Dictionary<String, BaseFunction>();

            Operator plus = new Operator("+", 1, true, (left, right) => left + right);
            AddOp(plus);
            Operator minus = new Operator("-", 1, true, (left, right) => left - right);
            AddOp(minus);
            Operator multiply = new Operator("*", 2, true, (left, right) => left * right);
            AddOp(multiply);
            Operator divide = new Operator("/", 2, true, (left, right) => left / right);
            AddOp(divide);

            BaseFunction max = new MathFunction("MAX", values => values.Max());
            AddFunc(max);

            BaseFunction min = new MathFunction("MIN", values => values.Min());
            AddFunc(min);

            BaseFunction abs = new MathFunction("ABS", values =>
            {
                BaseFunction.ValidateParameters(1, values);
                return Math.Abs(values[0]);
            });
            AddFunc(abs);

            BaseFunction noNegative = new MathFunction("NONNEG", values =>
            {
                BaseFunction.ValidateParameters(1, values);
                return values[0] >= 0 ? values[0] : 0;
            });
            AddFunc(noNegative);

            BaseFunction random = new MathFunction("RANDOM", values =>
            {
                BaseFunction.ValidateParameters(2, values);
                return new Random().Next((int)values[0], (int)values[1]);
            });

            BaseFunction ModifyValue = new NonMathFunction("MOD_VALUE", variables =>
            {
                //Usage: MOD_VALUE(Entity Caster, Entity TARGET,Stat TO_MODIFY ,Numeric AMOUNT)
                NonMathFunction.ValidateParameters(4, variables);
                NonMathFunction.ValidateParameter(variables[0], 0, VariableType.Entity);
                NonMathFunction.ValidateParameter(variables[1], 1, VariableType.Entity);
                NonMathFunction.ValidateParameter(variables[2], 2, VariableType.Stat);
                NonMathFunction.ValidateParameter(variables[3], 3, VariableType.NumericValue);

                Entity target = (Entity)(variables[1].Value);
                String statName = (String)(variables[2].Value);
                double value = (double)(variables[3].Value);
                target.ModifyValue(statName, value);
                return null;
            });

            AddFunc(random);
        }
    }
}
