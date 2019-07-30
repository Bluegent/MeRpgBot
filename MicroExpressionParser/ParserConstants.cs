using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroExpressionParser
{
    public class Operator
    {
        public string Character { get; }
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

        public Operator(string character, int precedence, bool leftAsoc, Func<double, double, double> operation)
        {
            this.Character = character;
            this.Precedence = precedence;
            this.Operation = operation;
            this.LeftAsoc = leftAsoc;
        }
    }

    public class AbstractFunction
    {
        public string Name { get; set; }
        public object Operation { get; set; }
        public int ParameterCount { get; }
        public AbstractFunction(String name, int parameterCount = 0)
        {
            Name = name;
            ParameterCount = parameterCount;
        }
        public void ValidateParameters(int variableCount)
        {
            if (ParameterCount == 0)
                return;
            if (variableCount != ParameterCount)
                throw new MeException($"Invalid argument count for function {Name}, got: {variableCount} expected:  {ParameterCount} .");
        }

        public MeVariable Execute(MeVariable[] variables)
        {
            return ((Func<MeVariable[], MeVariable>)(Operation)).Invoke(variables);
        }
    }

    public static class ParserConstants
    {
        public static Dictionary<string, Operator> Operators;
        public static Dictionary<string, AbstractFunction> Functions;
        public const char PARAM_SEPARATOR = ',';
        public const char LEFT_PAREN = '(';
        public const char RIGHT_PAREN = ')';

        public static bool IsSpecialChar(char c)
        {
            return c == PARAM_SEPARATOR || c == LEFT_PAREN || c == RIGHT_PAREN || IsOperator($"{c}");
        }

        public static bool IsSeparator(string str)
        {
            if (str.Length != 1)
                return false;
            if (str[0] == PARAM_SEPARATOR)
                return true;
            return false;
        }

        public static bool IsFunction(string str)
        {
            return Functions.ContainsKey(str);
        }

        public static bool IsOperator(string str)
        {
            return Operators.ContainsKey(str);
        }

        public static bool IsLeftParen(string str)
        {
            if (str.Length != 1)
                return false;
            if (str[0] == LEFT_PAREN)
                return true;
            return false;
        }

        public static bool IsRightParen(string str)
        {
            if (str.Length != 1)
                return false;
            if (str[0] == RIGHT_PAREN)
                return true;
            return false;
        }

        private static void AddOp(Operator op)
        {
            Operators.Add(op.Character, op);
        }
        private static void AddFunc(AbstractFunction func)
        {
            Functions.Add(func.Name, func);
        }
        public static void Init()
        {
            Operators = new Dictionary<string, Operator>();
            Functions = new Dictionary<string, AbstractFunction>();

            Operator plus = new Operator("+", 1, true, (left, right) => left + right);
            AddOp(plus);
            Operator minus = new Operator("-", 1, true, (left, right) => left - right);
            AddOp(minus);
            Operator multiply = new Operator("*", 2, true, (left, right) => left * right);
            AddOp(multiply);
            Operator divide = new Operator("/", 2, true, (left, right) => left / right);
            AddOp(divide);

            AbstractFunction max = new AbstractFunction("MAX");

            MeVariable MaxFunction(MeVariable[] values)
            {
                max.ValidateParameters(values.Length);
                double[] parameters = MeVariable.ToDoubleArray(values);
                return parameters.Max();
            }

            max.Operation = (Func<MeVariable[], MeVariable>)MaxFunction;
            AddFunc(max);

            AbstractFunction min = new AbstractFunction("MIN");

            MeVariable MinFunction(MeVariable[] values)
            {
                max.ValidateParameters(values.Length);
                double[] parameters = MeVariable.ToDoubleArray(values);
                return parameters.Min();
            }

            min.Operation = (Func<MeVariable[], MeVariable>)MinFunction;
            AddFunc(min);


            AbstractFunction abs = new AbstractFunction("ABS", 1);

            MeVariable AbsFunction(MeVariable[] values)
            {
                abs.ValidateParameters(values.Length);
                return Math.Abs(values[0].ToDouble());
            }

            abs.Operation = (Func<MeVariable[], MeVariable>)AbsFunction;
            AddFunc(abs);


            AbstractFunction noNegative = new AbstractFunction("NON_NEG", 1);

            MeVariable NoNegativeFunction(MeVariable[] values)
            {
                noNegative.ValidateParameters(values.Length);
                double value = values[0].ToDouble();
                return value > 0 ? value : 0;
            }

            noNegative.Operation = (Func<MeVariable[], MeVariable>)NoNegativeFunction;
            AddFunc(noNegative);


            AbstractFunction random = new AbstractFunction("RANDOM", 2);

            MeVariable RandomFunction(MeVariable[] values)
            {
                random.ValidateParameters(values.Length);
                return new Random().Next((int)values[0].ToDouble(), (int)values[1].ToDouble());
            }

            random.Operation = (Func<MeVariable[], MeVariable>)RandomFunction;
            AddFunc(random);


            AbstractFunction harm = new AbstractFunction("HARM", 3);

            MeVariable HarmFunction(MeVariable[] values)
            {
                //HARM(TARGET,TYPE,AMOUNT)
                harm.ValidateParameters(values.Length);
                Entity target = values[0].ToEntity();
                DamageType damageType = values[1].ToDamageType();
                double amount = values[2].ToDouble();
                //not implemented yet
                //target.TakeDamage(amount,type);
                return null;
            }

            harm.Operation = (Func<MeVariable[], MeVariable>)HarmFunction;
            AddFunc(harm);

            AbstractFunction array = new AbstractFunction("ARRAY");

            MeVariable ArrayFunction(MeVariable[] values)
            {
                array.ValidateParameters(values.Length);
                return new MeVariable() {Type = VariableType.Array, Value = values};
            }

            array.Operation = (Func<MeVariable[], MeVariable>)ArrayFunction;
            AddFunc(array);
        }
    }
}
