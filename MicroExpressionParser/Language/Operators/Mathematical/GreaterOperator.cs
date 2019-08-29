namespace RPGEngine.Language.Operators.Mathematical
{
    using RPGEngine.Language.DefinerUtils;

    public class GreaterOperator : IOperatorDefiner
    {
        public Operator DefineOperator()
        {
            return Utils.MakeOperator(LConstants.GREATER_OP, 0, true,
                (values, op) =>
                    {
                        op.CheckParamCount(values.Length);
                        return values[0].ToDouble() > values[1].ToDouble();
                    }, Definer.TwoDoubles);
        }
    }
}