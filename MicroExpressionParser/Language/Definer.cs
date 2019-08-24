using System;
using System.Collections.Generic;
using System.Linq;

using RPGEngine.Entities;
using RPGEngine.Core;
using RPGEngine.Game;
using RPGEngine.Utils;

namespace RPGEngine.Language
{
    

    public class Definer
    {
        private static readonly  Definer _instance = new Definer();
        public Dictionary<string, Operator> Operators;
        public Dictionary<string, Function> Functions;

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

        private void AddOperator(
            string character,
            int precedence,
            bool leftAssoc,
            Func<MeVariable[], Operator, MeVariable> operation,
            int opCount = 2)
        {
            Operator op = new Operator(character, precedence, leftAssoc, opCount);
            op.Operation = operation;
            Operators.Add(op.Key, op);
            foreach (char c in character)
            {
                if(!operatorChars.Contains(c))
                    operatorChars.Add(c);
            }
        }

        private void AddFunction(string name, Func<MeVariable[], Function, MeVariable> operation, int parameterCount = -1, bool[] executeInPlace = null)
        {
            Function func = new Function(name, parameterCount, executeInPlace);
            func.Operation = operation;
            Functions.Add(func.Key, func);
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

            Engine = engine;
            AddOperator(LConstants.PLUS_OP, 1, true,
            (values, op) =>
            {
                op.ValidateParameters(values.Length);
                return values[0].ToDouble() + values[1].ToDouble();
            });

            AddOperator(LConstants.MINUS_OP, 1, true,
            (values, op) =>
            {
                op.ValidateParameters(values.Length);
                return values[0].ToDouble() - values[1].ToDouble();
            });


            AddOperator(LConstants.MULITPLY_OP, 2, true,
                (values, op) =>
                {
                    op.ValidateParameters(values.Length);
                    return values[0].ToDouble() * values[1].ToDouble();
                });

            AddOperator(LConstants.POWER_OP, 3, true,
                (values, op) =>
                {
                    op.ValidateParameters(values.Length);
                    return Math.Pow(values[0].ToDouble(), values[1].ToDouble());
                });

            AddOperator(LConstants.DIVIDE_OP, 2, true,
                (values, op) =>
                {
                    op.ValidateParameters(values.Length);
                    return values[0].ToDouble() / values[1].ToDouble();
                });

            AddOperator(LConstants.NOT_OP, 2, true,
                (values, op) =>
                {
                    op.ValidateParameters(values.Length);
                    return !values[0].ToBoolean();
                }, 1);

            AddOperator(LConstants.GREATER_OP, 0, true,
                (values, op) =>
                {
                    op.ValidateParameters(values.Length);
                    return values[0].ToDouble() > values[1].ToDouble();
                });
            AddOperator(LConstants.LESSER_OP, 0, true,
                (values, op) =>
                {
                    op.ValidateParameters(values.Length);
                    return values[0].ToDouble() < values[1].ToDouble();
                });

            AddOperator(LConstants.EQUAL_OP, 0, true,
                (values, op) =>
                {
                    op.ValidateParameters(values.Length);
                    return Utility.DoubleEq(values[0].ToDouble(), values[1].ToDouble());
                });

            AddOperator(LConstants.ASSIGN_OP, -1, true, (values, op) =>
               {
                   op.ValidateParameters(values.Length);
                   string key = values[0].ToString();
                   MeVariable leftSide = Definer.Instance().Engine.GetVariable(key);
                   MeVariable rightSide = values[1];
                   if (rightSide.Type == VariableType.String)
                   {
                       rightSide = Definer.Instance().Engine.GetVariable(rightSide.Value.ToString());
                   }
                   if (leftSide == null)
                   {
                       Definer.Instance().Engine.AddVariable(key, rightSide);
                   }
                   else
                   {
                       Definer.Instance().Engine.SetVariable(key, rightSide);
                   }
                   return null;
               },2);

            AddOperator(LConstants.PROP_OP, 20, true, (values, op) =>
                  {
                  op.ValidateParameters(values.Length);
                  string key = values[1].ToString();
                  MeVariable var = values[0];
                  switch (var.Type)
                  {
                      case VariableType.Array:
                      {
                          if (key.Equals(LConstants.ARR_LENGTH))
                              {
                                  return var.ToArray().Length;
                              }
                          throw new MeException($"Attempting to retrieve undefined property \"{key}\" from array.");
                      }
                      case VariableType.Entity:
                      {

                          return  new MeVariable() { Value = new Property(var.ToEntity(),key), Type = VariableType.Property };
                        }
                          
                             
                    }
                      throw new MeException($"Attempting to retrieve undefined property \"{key}\" from variable \"{var}\"");
                  }
            ,2);

            AddFunction(LConstants.MAX_F,
                (values, func) =>
                {
                    func.ValidateParameters(values.Length);
                    double[] parameters = MeVariable.ToDoubleArray(values);
                    return parameters.Max();
                });

            AddFunction(LConstants.MIN_F,
                (values, func) =>
                {
                    func.ValidateParameters(values.Length);
                    double[] parameters = MeVariable.ToDoubleArray(values);
                    return parameters.Min();
                });


            AddFunction(LConstants.ABS_F,
                (values, func) =>
                {
                    func.ValidateParameters(values.Length);
                    return Math.Abs(values[0].ToDouble());
                }, 1);

            AddFunction(LConstants.NON_NEG_F,
                (values, func) =>
                {
                    func.ValidateParameters(values.Length);
                    double value = values[0].ToDouble();
                    return value > 0 ? value : 0;
                }, 1);

            AddFunction(LConstants.RANDOM_F,
                (values, func) =>
                {
                    func.ValidateParameters(values.Length);
                    return new Random().Next((int)values[0].ToDouble(), (int)values[1].ToDouble());
                }, 2);


            AddFunction(LConstants.HARM_F,
                (values, func) =>
                {
                    //func.ValidateParameters(values.Length);
                    MeVariable[] targets = values[0].ToArray();
                    Entity source = values[1].ToEntity();
                    DamageType damageType = values[2].ToDamageType();
                    double amount = values[3].ToDouble();
                    bool periodic = false;

                    if (values.Length > func.ParameterCount)
                        periodic = values[4].ToBoolean();
                    double totalAmt = 0;
                    foreach (MeVariable variable in targets)
                    {
                        totalAmt += variable.ToEntity().TakeDamage(amount, damageType, source, periodic);
                    }
                    return totalAmt;
                }, 4);

            AddFunction(LConstants.HEAL_F,
                (values, func) =>
                {
                    func.ValidateParameters(values.Length);
                    MeVariable[] targets = values[0].ToArray();
                    Entity source = values[1].ToEntity();
                    double amount = values[2].ToDouble();
                    double totalAmt = 0;
                    foreach (MeVariable variable in targets)
                    {
                        totalAmt+= variable.ToEntity().GetHealed(amount, source);
                    }
                    return totalAmt;
                }, 3);

            AddFunction(LConstants.ARRAY_F,
                (values, func) =>
                {
                    func.ValidateParameters(values.Length);
                    return new MeVariable() { Type = VariableType.Array, Value = values };
                });

            AddFunction(LConstants.GET_PLAYERS_F,
                (values, func) =>
                {
                    func.ValidateParameters(values.Length);
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
                    func.ValidateParameters(values.Length);
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
                    func.ValidateParameters(values.Length);
                    Entity entity = values[0].ToEntity();
                    string prop = values[1].ToString();
                    return new MeVariable() {Value = new Property(entity, prop), Type = VariableType.Property};
                    ;
                }, 2);

            AddFunction(LConstants.IF_F,
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
            AddFunction(LConstants.ARR_RANDOM_F,
                (values, func) =>
                {
                    func.ValidateParameters(values.Length);
                    MeVariable[] input = values[0].ToArray();
                    int index = new Random().Next(0, input.Length);
                    return input[index];

                }, 1);

            AddFunction(LConstants.CHANCE_F,
                (values, func) =>
                {
                    func.ValidateParameters(values.Length);
                    double chance = values[0].ToDouble() * 10;
                    return Utils.Utility.Chance(chance);

                }, 1);

            AddFunction(LConstants.CAST_F,
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
            AddFunction(LConstants.ADD_MOD_F,
               (values, func) =>
               {
                   //MOD_VALUE(stat,Amount)
                   func.ValidateParameters(values.Length);
                   string stat = values[0].ToString();
                   double amount = values[1].ToDouble();
                   StatModifier mod = new StatModifier() { Amount = amount, StatKey = stat };
                   return new MeVariable { Type = VariableType.StatModifier, Value = mod };
               }, 2);
            AddFunction(LConstants.APPLY_F,
               (values, func) =>
               {
                   //APPLYSTATUS(target,Source,status_key,duration,amounts)
                   func.ValidateParameters(values.Length);
                   MeVariable[] targets = values[0].ToArray();
                   Entity source = values[1].ToEntity();
                   StatusTemplate effect = Definer.Instance().Engine.GetStatusByKey(values[2].ToString());
                   double duration = values[3].ToDouble();
                   double[] amounts = MeVariable.ToDoubleArray(values[4].ToArray());
                   func.ValidateParameters(values.Length);

                   //TODO:construct a statusEffect
                   return null;
               }, 5);
            AddFunction(LConstants.GET_F, 
                (values, func) =>
            {
                func.ValidateParameters(values.Length);
                string key = values[0].ToString();
                return Definer.Instance().Engine.GetVariable(key); ;
            },1);

            AddFunction(LConstants.SAY_F,
                (values, func) =>
                {
                    func.ValidateParameters(values.Length);
                    Entity entity = values[0].ToEntity();
                    string msg = values[1].ToString();
                    Engine.Log().LogSay(entity,msg);
                    return null;
                }, 2);

            AddFunction(LConstants.PUSH_BACK_F,
                (values, func) =>
                    {
                        func.ValidateParameters(values.Length);
                        MeVariable[] entity = values[0].ToArray();
                        long amount = values[1].ToLong();
                        foreach (MeVariable var in entity)
                        {
                            var.ToEntity().AddPushback(amount);
                        }
                        return null;
                    }, 2);

        }
    }
}
