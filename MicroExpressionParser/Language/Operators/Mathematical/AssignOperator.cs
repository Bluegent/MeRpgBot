namespace RPGEngine.Language.Operators.Mathematical
{
    using RPGEngine.Core;
    using RPGEngine.Language.DefinerUtils;

    public class AssignOperator : IOperatorDefiner
    {
        public Operator DefineOperator()
        {
            return Utils.MakeOperator(LConstants.ASSIGN_OP, -1, true, (values, op) =>
                {
                    op.CheckParamCount(values.Length);
                    string key = values[0].ToMeString();
                    MeVariable leftSide = Definer.Instance().Engine.GetVariable(key);
                    MeVariable rightSide = values[1];
                    if (rightSide.Type == VariableType.String)
                    {
                        rightSide = Definer.Instance().Engine.GetVariable(rightSide.Value.ToString());
                    }
                    if (leftSide == null)
                    {
                        Definer.Instance().Engine.AddVariable(key, rightSide);
                    }
                    else
                    {
                        Definer.Instance().Engine.SetVariable(key, rightSide);
                    }
                    return null;
                }, new Validator(
                (variables, operation) => true), 2);
        }
    }
}