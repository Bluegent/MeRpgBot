namespace RPGEngine.Language
{
    using System;

    using MicroExpressionParser;

    using RPGEngine.Core;

    public class Function
    {
        public string Name { get; set; }
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
        public Function(string name, int parameterCount = -1, bool[] executeInPlace = null)
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
}
