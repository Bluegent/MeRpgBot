using System;
using RPGEngine.Core;
namespace RPGEngine.Language.DefinerUtils
{
    public  interface IOperatorDefiner
    {
        Operator DefineOperator();
    }

    public interface IFunctionDefiner
    {
        Function DefineFunction();
    }

    public class Utils
    {
        public static Operator MakeOperator(
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
            return op;
        }

        public static Function MakeFunction(string name, Func<MeVariable[], Operation, MeVariable> operation, int parameterCount = -1, bool[] executeInPlace = null)
        {
            Function func = new Function(name, parameterCount, executeInPlace);
            func.OpFunc = operation;
            return func;
        }
    }
}