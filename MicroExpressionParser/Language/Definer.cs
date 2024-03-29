﻿using System;
using System.Collections.Generic;
using System.Linq;

using RPGEngine.Entities;
using RPGEngine.Core;
using RPGEngine.Utils;

namespace RPGEngine.Language
{
    using RPGEngine.Language.DefinerUtils;
    using RPGEngine.Language.Operators.Mathematical;
    using RPGEngine.Language.Operators.Object;
    using RPGEngine.Templates;

    public class Definer
    {
        private static readonly Definer _instance = new Definer();
        public Dictionary<string, Operator> Operators;
        public Dictionary<string, Function> Functions;

        public static readonly Validator TwoDoubles = new Validator((values, op) =>
            {
                values[0].ToDouble();
                values[1].ToDouble();
                return true;
            });

        private List<char> operatorChars;
        private bool _initialized;
        public IGameEngine Engine { get; set; }

        public static Definer Instance()
        {
            return _instance;
        }

        private Definer()
        {
            _initialized = false;
            operatorChars = new List<char>();

            Operators = new Dictionary<string, Operator>();
            Functions = new Dictionary<string, Function>();
        }

        public bool IsOperatorChar(char c)
        {
            return operatorChars.Contains(c);
        }

        public bool IsSpecialChar(char c)
        {
            return c == LConstants.PARAM_SEPARATOR || c == LConstants.LEFT_PAREN || c == LConstants.RIGHT_PAREN || IsOperatorChar(c);
        }

        public bool IsSeparator(string str)
        {
            if (str.Length != 1)
                return false;
            if (str[0] == LConstants.PARAM_SEPARATOR)
                return true;
            return false;
        }

        public bool IsFunction(string str)
        {
            return Functions.ContainsKey(str);
        }

        public bool IsOperator(string str)
        {
            return Operators.ContainsKey(str);
        }

        public bool IsLeftParen(string str)
        {
            if (str.Length != 1)
                return false;
            if (str[0] == LConstants.LEFT_PAREN)
                return true;
            return false;
        }

        public bool IsRightParen(string str)
        {
            if (str.Length != 1)
                return false;
            if (str[0] == LConstants.RIGHT_PAREN)
                return true;
            return false;
        }

        private Operator AddOperator(
            string character,
            int precedence,
            bool leftAssoc,
            Func<MeVariable[], Operation, MeVariable> operation,
            Validator valid,
            int opCount = 2)
        {
            Operator op = new Operator(character, precedence, leftAssoc, opCount);
            op.OpFunc = operation;
            op.Validator = valid;
            Operators.Add(op.Key, op);
            foreach (char c in character)
            {
                if (!operatorChars.Contains(c))
                    operatorChars.Add(c);
            }

            return op;
        }

        private void AddOperator(Operator op)
        {
            Operators.Add(op.Key, op);
            foreach (char c in op.Key)
            {
                if (!operatorChars.Contains(c))
                    operatorChars.Add(c);
            }
        }

        private Function AddFunction(string name, Func<MeVariable[], Operation, MeVariable> operation, int parameterCount = -1, bool[] executeInPlace = null)
        {
            Function func = new Function(name, parameterCount, executeInPlace);
            func.OpFunc = operation;
            Functions.Add(func.Key, func);
            return func;
        }

        public bool Ignore(char c)
        {
            return LConstants.IgnoreChars.Contains(c);
        }
        public void Deinit()
        {
            Operators.Clear();
            Functions.Clear();
            operatorChars.Clear();
            _initialized = false;
        }

        public void Init(IGameEngine engine)
        {
            if (_initialized)
                return;
            _initialized = true;


            IOperatorDefiner[] opDefiners = {
                    new PlusOperator(),
                    new MinusOperator(),
                    new PropertyOperator(),
                    new MultiplyOperator(),
                    new PowerOperator(),
                    new DivideOperator(),
                    new NotOperator(),
                    new GreaterOperator(),
                    new LesserOperator(),
                    new AssignOperator(),
                    new NumEqualsOperator()



                };

            foreach (IOperatorDefiner def in opDefiners)
            {
                AddOperator(def.DefineOperator());
            }

            Engine = engine;




            AddFunction(LConstants.MAX_F,
                (values, func) =>
                {
                    func.CheckParamCount(values.Length);
                    double[] parameters = MeVariable.ToDoubleArray(values);
                    return parameters.Max();
                });

            AddFunction(LConstants.FLOOR_F,
                (values, func) =>
                {
                    func.CheckParamCount(values.Length);
                    return Math.Floor(values[0].ToDouble());
                }, 1);


            AddFunction(LConstants.MIN_F,
                (values, func) =>
                {
                    func.CheckParamCount(values.Length);
                    double[] parameters = MeVariable.ToDoubleArray(values);
                    return parameters.Min();
                });


            AddFunction(LConstants.ABS_F,
                (values, func) =>
                {
                    func.CheckParamCount(values.Length);
                    return Math.Abs(values[0].ToDouble());
                }, 1);

            AddFunction(LConstants.NON_NEG_F,
                (values, func) =>
                {
                    func.CheckParamCount(values.Length);
                    double value = values[0].ToDouble();
                    return value > 0 ? value : 0;
                }, 1);

            AddFunction(LConstants.RANDOM_F,
                (values, func) =>
                {
                    func.CheckParamCount(values.Length);
                    return new Random().Next((int)values[0].ToDouble(), (int)values[1].ToDouble());
                }, 2);


            AddFunction(LConstants.HARM_F,
                (values, func) =>
                {
                    //func.CheckParamCount(values.Length);
                    MeVariable[] targets = values[0].ToArray();
                    Entity source = values[1].ToEntity();
                    DamageTypeTemplate damageTypeTemplate = values[2].ToDamageType();
                    double amount = values[3].ToDouble();
                    bool periodic = false;

                    if (values.Length > func.ParameterCount)
                        periodic = values[4].ToBoolean();
                    double totalAmt = 0;
                    foreach (MeVariable variable in targets)
                    {
                        totalAmt += variable.ToEntity().TakeDamage(amount, damageTypeTemplate, source, periodic);
                    }
                    return totalAmt;
                }, 4);

            AddFunction(LConstants.HEAL_F,
                (values, func) =>
                {
                    func.CheckParamCount(values.Length);
                    MeVariable[] targets = values[0].ToArray();
                    Entity source = values[1].ToEntity();
                    double amount = values[2].ToDouble();
                    double totalAmt = 0;
                    foreach (MeVariable variable in targets)
                    {
                        totalAmt += variable.ToEntity().GetHealed(amount, source);
                    }
                    return totalAmt;
                }, 3);

            AddFunction(LConstants.ARRAY_F,
                (values, func) =>
                {
                    func.CheckParamCount(values.Length);
                    return new MeVariable() { Type = VariableType.Array, Value = values };
                });

            AddFunction(LConstants.GET_PLAYERS_F,
                (values, func) =>
                {
                    func.CheckParamCount(values.Length);
                    Entity[] players = Definer.Instance().Engine.GetAllPlayers();
                    List<MeVariable> playerList = new List<MeVariable>();
                    foreach (Entity entity in players)
                    {
                        playerList.Add(entity);
                    }
                    return new MeVariable() { Value = playerList.ToArray(), Type = VariableType.Array };
                }, 0);

            AddFunction(LConstants.GET_ACTIVE_PLAYERS_F,
                (values, func) =>
                {
                    func.CheckParamCount(values.Length);
                    //TODO: Implement retrieving ONLY active players
                    Entity[] players = Definer.Instance().Engine.GetAllPlayers();
                    List<MeVariable> playerList = new List<MeVariable>();
                    foreach (Entity entity in players)
                    {
                        playerList.Add(entity);
                    }
                    return new MeVariable() { Value = playerList.ToArray(), Type = VariableType.Array };
                }, 0);

            AddFunction(LConstants.GET_PROP_F,
                (values, func) =>
                {
                    func.CheckParamCount(values.Length);
                    Entity entity = values[0].ToEntity();
                    string prop = values[1].ToMeString();
                    return new MeVariable() { Value = new Property(entity, prop), Type = VariableType.Property };
                    ;
                }, 2);

            AddFunction(LConstants.IF_F,
                (values, func) =>
                {
                    //IF(CONDITION,THEN,ELSE)
                    func.CheckParamCount(values.Length);
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
            AddFunction(LConstants.ARR_RANDOM_F,
                (values, func) =>
                {
                    func.CheckParamCount(values.Length);
                    MeVariable[] input = values[0].ToArray();
                    int index = new Random().Next(0, input.Length);
                    return input[index];

                }, 1);

            AddFunction(LConstants.CHANCE_F,
                (values, func) =>
                {
                    func.CheckParamCount(values.Length);
                    double chance = values[0].ToDouble() * 10;
                    return RPGEngine.Utils.Utility.Chance(chance);

                }, 1);

            AddFunction(LConstants.CAST_F,
                (values, func) =>
                {
                    //CAST(CASTER,TARGET,SKILL)
                    func.CheckParamCount(values.Length);
                    Entity caster = values[0].ToEntity();
                    Entity target = values[1].ToEntity();
                    string skillKey = values[2].ToMeString();
                    caster.Cast(target, skillKey);
                    return null;

                }, 3);
            AddFunction(LConstants.ADD_MOD_F,
               (values, func) =>
               {
                   //MOD_VALUE(stat,Amount)
                   func.CheckParamCount(values.Length);
                   string stat = values[0].ToMeString();
                   double amount = values[1].ToDouble();
                   StatModifier mod = new StatModifier() { Amount = amount, StatKey = stat };
                   return new MeVariable { Type = VariableType.StatModifier, Value = mod };
               }, 2);
            AddFunction(LConstants.APPLY_F,
               (values, func) =>
               {
                   //APPLYSTATUS(target,Source,status_key,duration,amounts)
                   func.CheckParamCount(values.Length);
                   MeVariable[] targets = values[0].ToArray();
                   Entity source = values[1].ToEntity();
                   StatusTemplate effect = Definer.Instance().Engine.GetStatusByKey(values[2].ToMeString());
                   double duration = values[3].ToDouble();
                   double[] amounts = MeVariable.ToDoubleArray(values[4].ToArray());
                   func.CheckParamCount(values.Length);

                   foreach (MeVariable target in targets)
                   {
                       target.ToEntity().ApplyStatus(effect, source, duration, amounts);
                   }
                   return null;
               }, 5);
            AddFunction(LConstants.GET_F,
                (values, func) =>
            {
                func.CheckParamCount(values.Length);
                string key = values[0].ToMeString();
                return Definer.Instance().Engine.GetVariable(key); ;
            }, 1);

            AddFunction(LConstants.SAY_F,
                (values, func) =>
                {
                    func.CheckParamCount(values.Length);
                    Entity entity = values[0].ToEntity();
                    string msg = values[1].ToMeString();
                    Engine.Log().LogSay(entity, msg);
                    return null;
                }, 2);

            AddFunction(LConstants.PUSH_BACK_F,
                (values, func) =>
                    {
                        func.CheckParamCount(values.Length);
                        MeVariable[] entity = values[0].ToArray();
                        long amount = values[1].ToLong();
                        foreach (MeVariable var in entity)
                        {
                            var.ToEntity().AddPushback(amount);
                        }
                        return null;
                    }, 2);
            AddFunction(LConstants.REVIVE_F,
                (values, func) =>
                {
                    func.CheckParamCount(values.Length);
                    MeVariable[] entity = values[0].ToArray();
                    foreach (MeVariable var in entity)
                    {
                        var.ToEntity().Revive();
                    }
                    return null;
                }, 1);
            AddFunction(LConstants.ADD_TO_RESOURCE_F,
                (values, func) =>
                {
                    func.CheckParamCount(values.Length);
                    MeVariable[] entity = values[0].ToArray();
                    string resourceKey = values[1].ToString();
                    double amount = values[2].ToDouble();
                    foreach (MeVariable var in entity)
                    {
                        var.ToEntity().AddToResource(resourceKey, amount);
                    }
                    return null;
                }, 3);
        }
    }
}
