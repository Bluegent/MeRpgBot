namespace RPGEngine.Language.Operators.Mathematical
{
    using System;

    using RPGEngine.Language.DefinerUtils;
    public class PowerOperator : IOperatorDefiner
    {
        public Operator DefineOperator()
        {
            return
                Utils.MakeOperator(LConstants.POWER_OP, 3, true,
                    (values, op) =>
                        {
                            op.CheckParamCount(values.Length);
                            return Math.Pow(values[0].ToDouble(), values[1].ToDouble());
                        }, Definer.TwoDoubles);
        }
    }
}