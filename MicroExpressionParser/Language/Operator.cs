using System;
using RPGEngine.Core;

namespace RPGEngine.Language
{
    public class Operator : Operation
    {
        public int Precedence { get; }

        public bool LeftAsoc { get; }


        public bool Precedes(Operator other)
        {
            return Precedence > other.Precedence;
        }

        public bool IsUnary()
        {
            return ParameterCount == 1;
        }
        public Operator(string key, int precedence, bool leftAsoc, int operatorCount = 2) : base(key,operatorCount)
        {
            Precedence = precedence;
            LeftAsoc = leftAsoc;
        }
    }
}
