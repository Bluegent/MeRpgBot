namespace RPGEngine.Language.Operators.Mathematical
{
    using RPGEngine.Language.DefinerUtils;

    public class DivideOperator : IOperatorDefiner
    {
        public Operator DefineOperator()
        {
            return Utils.MakeOperator(LConstants.DIVIDE_OP, 2, true,
                (values, op) =>
                {
                    op.CheckParamCount(values.Length);
                    return values[0].ToDouble() / values[1].ToDouble();
                }, Definer.TwoDoubles);
        }
    }
}