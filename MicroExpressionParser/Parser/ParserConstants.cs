using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroExpressionParser
{
    public class Operator
    {
        public string Character { get; }
        public int Precedence { get; }
        public Func<MeVariable[], Operator, MeVariable> Operation { get; set; }
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

        public bool IsUnary()
        {
            return OperatorCount == 1;
        }
        public Operator(string character, int precedence, bool leftAsoc, int operatorCount = 2)
        {
            Character = character;
            Precedence = precedence;
            LeftAsoc = leftAsoc;
            OperatorCount = operatorCount;
        }
        public MeVariable Execute(MeVariable[] variables)
        {
            return Operation.Invoke(variables, this);
        }
    }

    public class AbstractFunction
    {
        public string Name { get; set; }
        public Func<MeVariable[], AbstractFunction, MeVariable> Operation { get; set; }

        private bool[] ExecuteInPlace;
        public int ParameterCount { get; }

        public bool ExecuteSubNode(int index)
        {
            if (ExecuteInPlace == null)
            {
                return true;
            }
            else
            {
                return ExecuteInPlace[index];
            }
        }
        public AbstractFunction(string name, int parameterCount = -1, bool[] executeInPlace = null)
        {
            Name = name;
            ParameterCount = parameterCount;
            ExecuteInPlace = executeInPlace;
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
            return Operation.Invoke(variables, this);
        }
    }

    public static class ParserConstants
    {
        public static Dictionary<string, Operator> Operators;
        public static Dictionary<string, AbstractFunction> Functions;
        public const char PARAM_SEPARATOR = ',';
        public const char LEFT_PAREN = '(';
        public const char RIGHT_PAREN = ')';
        public const char SPECIAL_CHAR = '$';

        public static readonly string TargetKeyword = SPECIAL_CHAR+"TARGET";

        public static readonly string CasterKeyword = SPECIAL_CHAR+"CASTER";

        public const char FUNCTION_SEPARATOR = '~';

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

        private static void AddOperator(
            string character,
            int precedence,
            bool leftAssoc,
            Func<MeVariable[], Operator, MeVariable> operation,
            int opCount = 2)
        {
            Operator op = new Operator(character, precedence, leftAssoc, opCount);
            op.Operation = operation;
            Operators.Add(op.Character, op);
        }

        private static void AddFunction(string name, Func<MeVariable[], AbstractFunction, MeVariable> operation, int parameterCount = -1,bool[] executeInPlace = null)
        {
            AbstractFunction func = new AbstractFunction(name,parameterCount, executeInPlace);
            func.Operation = operation;
            Functions.Add(func.Name, func);
        }
        public static void Init(IGameEngine engine)
        {
            Operators = new Dictionary<string, Operator>();
            Functions = new Dictionary<string, AbstractFunction>();

            AddOperator("+", 1, true,
            (values, op) =>
                {
                    op.ValidateParameters(values.Length);
                    return values[0].ToDouble() + values[1].ToDouble();
                });

            AddOperator("-", 1, true,
            (values, op) =>
                {
                    op.ValidateParameters(values.Length);
                    return values[0].ToDouble() - values[1].ToDouble();
                });


            AddOperator("*", 2, true,
                (values, op) =>
                    {
                        op.ValidateParameters(values.Length);
                        return values[0].ToDouble() * values[1].ToDouble();
                    });

            AddOperator("/", 2, true,
                (values, op) =>
                    {
                        op.ValidateParameters(values.Length);
                        return values[0].ToDouble() / values[1].ToDouble();
                    });

            AddOperator("!", 2, true,
                (values, op) =>
                    {
                        op.ValidateParameters(values.Length);
                        return !values[0].ToBoolean();
                    },1);

            AddOperator(">", 0, true,
                (values, op) =>
                    {
                        op.ValidateParameters(values.Length);
                        return values[0].ToDouble()>values[1].ToDouble();
                    });


            AddFunction("MAX",
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        double[] parameters = MeVariable.ToDoubleArray(values);
                        return parameters.Max();
                    });

            AddFunction("MIN",
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        double[] parameters = MeVariable.ToDoubleArray(values);
                        return parameters.Min();
                    });


            AddFunction("ABS",
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        return Math.Abs(values[0].ToDouble());
                    },1);

            AddFunction("NON_NEG",
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        double value = values[0].ToDouble();
                        return value > 0 ? value : 0;
                    }, 1);

            AddFunction("RANDOM",
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        return new Random().Next((int)values[0].ToDouble(), (int)values[1].ToDouble());
                    },2);


            AddFunction("HARM",
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        Entity target = values[0].ToEntity();
                        DamageType damageType = values[1].ToDamageType();
                        double amount = values[2].ToDouble();
                        target.TakeDamage(amount, damageType);
                        return null;
                    }, 3);

            AddFunction("ARRAY",
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        return new MeVariable() { Type = VariableType.Array, Value = values };
                    });

            AddFunction("GET_PLAYERS",
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        Entity[] players = engine.GetAllPlayers();
                        List<MeVariable> playerList = new List<MeVariable>();
                        foreach (Entity entity in players)
                        {
                            playerList.Add(entity);
                        }
                        return new MeVariable() { Value = playerList.ToArray(), Type = VariableType.Array };
                    },0);

            AddFunction("GET_PROP",
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        Entity entity = values[0].ToEntity();
                        string prop = values[1].ToString();
                        return entity.GetProperty(prop).Value;
                    },2);

            AddFunction("IF",
                (values, func) =>
                    {
                        //IF(CONDITION,THEN,ELSE)
                        func.ValidateParameters(values.Length);
                        bool condition = values[0].ToBoolean();
                        if (condition)
                        {
                            return values[1].Execute();
                        }
                        else
                        {
                            return values[2].Execute();
                        }
                    }, 3,new bool[]{true,false,false});


        }
    }
}
