using RPGEngine.Language.DefinerUtils;
namespace RPGEngine.Language.Operators.Mathematical
{

    public class PlusOperator : IOperatorDefiner
    {

        public Operator DefineOperator()
        {
            return DefinerUtils.Utils.MakeOperator(
                LConstants.PLUS_OP,
                1,
                true,
                (values, op) =>
                    {
                        op.CheckParamCount(values.Length);
                        return values[0].ToDouble() + values[1].ToDouble();
                    }, Definer.TwoDoubles);
        }
    }
}