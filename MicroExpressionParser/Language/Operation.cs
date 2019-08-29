using System;
using RPGEngine.Core;

namespace RPGEngine.Language
{
    public class Validator
    {
        private Func<MeVariable[], Operation, bool> Func { get; set; }

        public static implicit operator Validator(Func<MeVariable[], Operation, bool> func)
        {
            return new Validator(func);
        }

        public Validator(Func<MeVariable[], Operation, bool> func)
        {
            Func = func;
        }

        public bool Validate(MeVariable[] values, Operation op)
        {
            try
            {
                return Func.Invoke(values, op);
            }
            catch (MeException)
            {
                return false;
            }
        }
    }

    public class Operation
    {
        public string Key { get; }
        public Validator Validator { get; set; }
        public Func<MeVariable[], Operation, MeVariable> OpFunc { get; set; }

        public int ParameterCount { get; set; }

        public Operation(string key, int parameterCount = -1)
        {
            Key = key;
            ParameterCount = parameterCount;
        }

        public bool CanExecute(MeVariable[] parameters)
        {
            return Validator != null && Validator.Validate(parameters, this);
        }
        public MeVariable Execute(MeVariable[] parameters)
        {
            return OpFunc.Invoke(parameters, this);
        }

        public void CheckParamCount(int parameterCount)
        {
            if (ParameterCount != -1 && parameterCount != ParameterCount)
                throw new MeException($"Invalid argument count for operation {Key}, got: {parameterCount} expected:  {ParameterCount} .");
        }
    }
}