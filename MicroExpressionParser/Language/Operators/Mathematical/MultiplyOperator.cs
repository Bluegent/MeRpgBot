namespace RPGEngine.Language.Operators.Mathematical
{
    using RPGEngine.Language.DefinerUtils;

    public class MultiplyOperator : IOperatorDefiner
    {
        public Operator DefineOperator()
        {
            return Utils.MakeOperator(LConstants.MULITPLY_OP, 2, true,
                (values, op) =>
                    {
                        op.CheckParamCount(values.Length);
                        return values[0].ToDouble() * values[1].ToDouble();
                    }, Definer.TwoDoubles);
        }
    }
}