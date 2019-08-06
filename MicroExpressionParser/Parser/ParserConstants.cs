using MicroExpressionParser.Core;
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

        public static bool IsSpecialChar(char c)
        {
            return c == StringConstants.PARAM_SEPARATOR || c == StringConstants.LEFT_PAREN || c == StringConstants.RIGHT_PAREN || IsOperator(char.ToString(c));
        }

        public static bool IsSeparator(string str)
        {
            if (str.Length != 1)
                return false;
            if (str[0] == StringConstants.PARAM_SEPARATOR)
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
            if (str[0] == StringConstants.LEFT_PAREN)
                return true;
            return false;
        }

        public static bool IsRightParen(string str)
        {
            if (str.Length != 1)
                return false;
            if (str[0] == StringConstants.RIGHT_PAREN)
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

        private static void AddFunction(string name, Func<MeVariable[], AbstractFunction, MeVariable> operation, int parameterCount = -1, bool[] executeInPlace = null)
        {
            AbstractFunction func = new AbstractFunction(name, parameterCount, executeInPlace);
            func.Operation = operation;
            Functions.Add(func.Name, func);
        }
        public static void Init(IGameEngine engine)
        {
            Operators = new Dictionary<string, Operator>();
            Functions = new Dictionary<string, AbstractFunction>();

            AddOperator(StringConstants.PLUS_OP, 1, true,
            (values, op) =>
                {
                    op.ValidateParameters(values.Length);
                    return values[0].ToDouble() + values[1].ToDouble();
                });

            AddOperator(StringConstants.MINUS_OP, 1, true,
            (values, op) =>
                {
                    op.ValidateParameters(values.Length);
                    return values[0].ToDouble() - values[1].ToDouble();
                });


            AddOperator(StringConstants.MULITPLY_OP, 2, true,
                (values, op) =>
                    {
                        op.ValidateParameters(values.Length);
                        return values[0].ToDouble() * values[1].ToDouble();
                    });

            AddOperator(StringConstants.POWER_OP, 3, true,
                (values, op) =>
                    {
                        op.ValidateParameters(values.Length);
                        return Math.Pow(values[0].ToDouble(), values[1].ToDouble());
                    });

            AddOperator(StringConstants.DIVIDE_OP, 2, true,
                (values, op) =>
                    {
                        op.ValidateParameters(values.Length);
                        return values[0].ToDouble() / values[1].ToDouble();
                    });

            AddOperator(StringConstants.NOT_OP, 2, true,
                (values, op) =>
                    {
                        op.ValidateParameters(values.Length);
                        return !values[0].ToBoolean();
                    }, 1);

            AddOperator(StringConstants.GREATER_OP, 0, true,
                (values, op) =>
                    {
                        op.ValidateParameters(values.Length);
                        return values[0].ToDouble() > values[1].ToDouble();
                    });
            AddOperator(StringConstants.LESSER_OP, 0, true,
                (values, op) =>
                    {
                        op.ValidateParameters(values.Length);
                        return values[0].ToDouble() < values[1].ToDouble();
                    });

            AddOperator(StringConstants.EQUAL_OP, 0, true,
                (values, op) =>
                    {
                        op.ValidateParameters(values.Length);
                        return values[0].ToDouble() == values[1].ToDouble();
                    });

            AddFunction(StringConstants.MAX_F,
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        double[] parameters = MeVariable.ToDoubleArray(values);
                        return parameters.Max();
                    });

            AddFunction(StringConstants.MIN_F,
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        double[] parameters = MeVariable.ToDoubleArray(values);
                        return parameters.Min();
                    });


            AddFunction(StringConstants.ABS_F,
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        return Math.Abs(values[0].ToDouble());
                    }, 1);

            AddFunction(StringConstants.NON_NEG_F,
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        double value = values[0].ToDouble();
                        return value > 0 ? value : 0;
                    }, 1);

            AddFunction(StringConstants.RANDOM_F,
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        return new Random().Next((int)values[0].ToDouble(), (int)values[1].ToDouble());
                    }, 2);


            AddFunction(StringConstants.HARM_F,
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        Entity target = values[0].ToEntity();
                        Entity source = values[1].ToEntity();
                        DamageType damageType = values[2].ToDamageType();
                        double amount = values[3].ToDouble();
                        target.TakeDamage(amount, damageType, source);
                        return null;
                    }, 4);

            AddFunction(StringConstants.HEAL_F,
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        Entity target = values[0].ToEntity();
                        Entity source = values[1].ToEntity();
                        double amount = values[2].ToDouble();
                        target.GetHealed(amount, source);
                        return null;
                    }, 3);

            AddFunction(StringConstants.ARRAY_F,
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        return new MeVariable() { Type = VariableType.Array, Value = values };
                    });

            AddFunction(StringConstants.GET_PLAYERS_F,
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
                    }, 0);

            AddFunction(StringConstants.GET_ACTIVE_PLAYERS_F,
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        //TODO: Implement retrieving ONLY active players
                        Entity[] players = engine.GetAllPlayers();
                        List<MeVariable> playerList = new List<MeVariable>();
                        foreach (Entity entity in players)
                        {
                            playerList.Add(entity);
                        }
                        return new MeVariable() { Value = playerList.ToArray(), Type = VariableType.Array };
                    }, 0);

            AddFunction(StringConstants.GET_PROP_F,
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        Entity entity = values[0].ToEntity();
                        string prop = values[1].ToString();
                        return entity.GetProperty(prop).Value;
                    }, 2);

            AddFunction(StringConstants.IF_F,
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
                    }, 3, new bool[] { true, false, false });
            AddFunction(StringConstants.ARR_RANDOM_F,
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        MeVariable[] input = values[0].ToArray();
                        int index = new Random().Next(0, input.Length);
                        return input[index];

                    }, 1);

            AddFunction(StringConstants.CHANCE_F,
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        double chance = values[0].ToDouble() * 10;
                        int dice = new Random().Next(0, 1000);

                        return dice < chance;

                    }, 1);

            AddFunction(StringConstants.CAST_F,
                (values, func) =>
                    {
                        //CAST(CASTER,TARGET,SKILL)
                        func.ValidateParameters(values.Length);
                        Entity caster = values[0].ToEntity();
                        Entity target = values[1].ToEntity();
                        string skillKey = values[2].ToString();
                        caster.Cast(target, skillKey);
                        return null;

                    }, 3);
            AddFunction(StringConstants.MOD_VALUE_F,
               (values, func) =>
               {
                   //MOD_VALUE(stat,amount)
                   func.ValidateParameters(values.Length);
                   string stat = values[0].ToString();
                   double amount = values[1].ToDouble();
                   StatModifier mod = new StatModifier() { amount = amount, key = stat };
                   return new MeVariable { Type = VariableType.StatModifier, Value = mod };
               }, 2);
            AddFunction(StringConstants.APPLY_F,
               (values, func) =>
               {
                   //APPLYSTATUS(target,source,status_key,duration,amounts)
                   Entity target = values[0].ToEntity();
                   Entity source = values[1].ToEntity();
                   StatusTemplate effect = engine.GetStatusByKey(values[2].ToString());
                   double duration = values[3].ToDouble();
                   double[] amounts = MeVariable.ToDoubleArray(values[4].ToArray());
                   func.ValidateParameters(values.Length);

                   //TODO:construct a statusEffect
                   return null;
               }, 5);

        }
    }
}
