namespace RPGEngine.Language
{
    using System;

    using RPGEngine;

    using RPGEngine.Core;

    public class Function
    {
        public string Key { get; set; }
        public Func<MeVariable[], Function, MeVariable> Operation { get; set; }

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
        public Function(string key, int parameterCount = -1, bool[] executeInPlace = null)
        {
            Key = key;
            ParameterCount = parameterCount;
            ExecuteInPlace = executeInPlace;
        }
        public void ValidateParameters(int variableCount)
        {
            if (ParameterCount == -1)
                return;
            if (variableCount != ParameterCount)
                throw new MeException($"Invalid argument count for function {Key}, got: {variableCount} expected:  {ParameterCount} .");
        }

        public MeVariable Execute(MeVariable[] variables)
        {
            return Operation.Invoke(variables, this);
        }
    }
}
