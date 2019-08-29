namespace RPGEngine.Language.Operators.Mathematical
{
    using RPGEngine.Language.DefinerUtils;

    public class MinusOperator :IOperatorDefiner
    {
        public Operator DefineOperator()
        {

            return Utils.MakeOperator(LConstants.MINUS_OP, 1, true,
                (values, op) =>
                    {
                        op.CheckParamCount(values.Length);
                        return values[0].ToDouble() - values[1].ToDouble();
                    }, Definer.TwoDoubles);
        }
    }
}