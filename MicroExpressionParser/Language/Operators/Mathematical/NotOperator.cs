namespace RPGEngine.Language.Operators.Mathematical
{
    using RPGEngine.Language.DefinerUtils;

    public class NotOperator : IOperatorDefiner
    {
        public Operator DefineOperator()
        {
            return Utils.MakeOperator(LConstants.NOT_OP, 2, true,
                (values, op) =>
                    {
                        op.CheckParamCount(values.Length);
                        return !values[0].ToBoolean();
                    }, new Validator((variables, operation) =>
                    {
                        variables[0].ToBoolean();
                        return true;
                    })
                , 1);
        }
    }
}