namespace RPGEngine.Language
{
    using System;

    using MicroExpressionParser;

    using RPGEngine.Core;

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
}
