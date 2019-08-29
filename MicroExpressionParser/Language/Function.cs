using System;
using RPGEngine.Core;

namespace RPGEngine.Language
{
   

    public class Function : Operation
    {
        private readonly bool[] _executeInPlace;

        public bool ExecuteSubNode(int index)
        {
            if (_executeInPlace == null)
            {
                return true;
            }
            else
            {
                return _executeInPlace[index];
            }
        }
        public Function(string key, int parameterCount = -1, bool[] executeInPlace = null) :base(key, parameterCount)
        {
            ParameterCount = parameterCount;
            _executeInPlace = executeInPlace;
        }
    }
}
