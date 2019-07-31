using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroExpressionParser
{
    public class Operator
    {
        public string Character { get; }
        public int Precedence { get; }
        public Func<MeVariable[], MeVariable> Operation { get; set; }
        public bool LeftAsoc { get; }
        public int OperatorCount { get; }

        public void ValidateParameters(int parameterCount)
        {
            if (parameterCount != OperatorCount)
                throw new MeException($"Invalid argument count for operator {Character}, got: {parameterCount} expected:  {OperatorCount} .");
        }

        public bool Precedes(Operator other)
        {
            return this.Precedence > other.Precedence;
        }

        public Operator(string character, int precedence, bool leftAsoc,  int operatorCount = 2)
        {
            Character = character;
            Precedence = precedence;
            LeftAsoc = leftAsoc;
            OperatorCount = operatorCount;
        }
    }

    public class AbstractFunction
    {
        public string Name { get; set; }
        public object Operation { get; set; }
        public int ParameterCount { get; }
        public AbstractFunction(string name, int parameterCount = -1)
        {
            Name = name;
            ParameterCount = parameterCount;
        }
        public void ValidateParameters(int variableCount)
        {
            if (ParameterCount == -1)
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
        public static void Init(IGameEngine engine)
        {
            Operators = new Dictionary<string, Operator>();
            Functions = new Dictionary<string, AbstractFunction>();

            Operator plus = new Operator("+", 1, true);
            MeVariable PlusFunction(MeVariable[] values)
            {
                plus.ValidateParameters(values.Length);
                return values[0].ToDouble() + values[1].ToDouble();
            }
            plus.Operation = PlusFunction;
            AddOp(plus);

            Operator minus = new Operator("-", 1, true);
            MeVariable MinusFunc(MeVariable[] values)
            {
                minus.ValidateParameters(values.Length);
                return values[0].ToDouble() - values[1].ToDouble();
            }
            minus.Operation = MinusFunc;
            AddOp(minus);

            Operator multiply = new Operator("*", 2, true);
            MeVariable MultiplyFunc(MeVariable[] values)
            {
                multiply.ValidateParameters(values.Length);
                return values[0].ToDouble() * values[1].ToDouble();
            }
            multiply.Operation = MultiplyFunc;
            AddOp(multiply);

            Operator divide = new Operator("/", 2, true);
            MeVariable DivideFunc(MeVariable[] values)
            {
                divide.ValidateParameters(values.Length);
                return values[0].ToDouble() - values[1].ToDouble();
            }
            divide.Operation = DivideFunc;
            AddOp(divide);

            Operator not = new Operator("!", 3, false, 1);
            MeVariable NotFunc(MeVariable[] values)
            {
                not.ValidateParameters(values.Length);
                return !values[0].ToBoolean();
            }
            not.Operation = NotFunc;
            AddOp(not);

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
                target.TakeDamage(amount, damageType);
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


            AbstractFunction getAllPlayers = new AbstractFunction("GET_PLAYERS",0);
            MeVariable GetAllPlayersFunction(MeVariable[] values)
            {
                getAllPlayers.ValidateParameters(values.Length);
                Entity[] players = engine.GetAllPlayers();
                List<MeVariable> playerList = new List<MeVariable>();
                foreach(Entity entity in players)
                {
                    playerList.Add(entity);
                }
                return new MeVariable(){Value = playerList.ToArray(), Type = VariableType.Array};
            }
            getAllPlayers.Operation = (Func<MeVariable[], MeVariable>)GetAllPlayersFunction;
            AddFunc(getAllPlayers);

            AbstractFunction getProperty = new AbstractFunction("GET_PROP", 2);
            MeVariable GetPropertyFunction(MeVariable[] values)
            {
                getProperty.ValidateParameters(values.Length);
                Entity entity = values[0].ToEntity();
                string prop = values[1].ToString();
                return entity.GetProperty(prop).Value;
            }
            getProperty.Operation = (Func<MeVariable[], MeVariable>)GetPropertyFunction;
            AddFunc(getProperty);
        }
    }
}
