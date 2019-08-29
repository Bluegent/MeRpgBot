namespace RPGEngine.Language.Operators.Mathematical
{
    using RPGEngine.Language.DefinerUtils;
    using RPGEngine.Utils;

    public class NumEqualsOperator : IOperatorDefiner
    {
        public Operator DefineOperator()
        {

            return Utils.MakeOperator(LConstants.EQUAL_OP, 0, true,
                (values, op) =>
                    {
                        op.CheckParamCount(values.Length);
                        return Utility.DoubleEq(values[0].ToDouble(), values[1].ToDouble());
                    }, Definer.TwoDoubles);
        }
    }
}