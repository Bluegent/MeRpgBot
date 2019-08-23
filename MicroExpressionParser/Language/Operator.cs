namespace RPGEngine.Language
{
    using System;

    using RPGEngine;

    using RPGEngine.Core;

    public class Operator
    {
        public string Key { get; }
        public int Precedence { get; }
        public Func<MeVariable[], Operator, MeVariable> Operation { get; set; }
        public bool LeftAsoc { get; }
        public int OperatorCount { get; }

        public void ValidateParameters(int parameterCount)
        {
            if (parameterCount != OperatorCount)
                throw new MeException($"Invalid argument count for operator {Key}, got: {parameterCount} expected:  {OperatorCount} .");
        }

        public bool Precedes(Operator other)
        {
            return Precedence > other.Precedence;
        }

        public bool IsUnary()
        {
            return OperatorCount == 1;
        }
        public Operator(string key, int precedence, bool leftAsoc, int operatorCount = 2)
        {
            Key = key;
            Precedence = precedence;
            LeftAsoc = leftAsoc;
            OperatorCount = operatorCount;
        }
        public MeVariable Execute(MeVariable[] variables)
        {
            return Operation.Invoke(variables, this);
        }
    }
}
