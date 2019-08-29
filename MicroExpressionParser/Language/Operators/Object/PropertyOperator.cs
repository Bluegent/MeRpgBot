namespace RPGEngine.Language.Operators.Object
{
    using RPGEngine.Core;
    using RPGEngine.Entities;
    using RPGEngine.Language.DefinerUtils;

    public class PropertyOperator :IOperatorDefiner
    {
        public Operator DefineOperator()
        {
            return
            Utils.MakeOperator(LConstants.PROP_OP, 20, true, (values, op) =>
            {
                op.CheckParamCount(values.Length);
                string key = values[1].ToMeString();
                MeVariable var = values[0];
                switch (var.Type)
                {
                    case VariableType.Array:
                        {
                            if (key.Equals(LConstants.ARR_LENGTH))
                            {
                                return var.ToArray().Length;
                            }
                            throw new MeException($"Attempting to retrieve undefined property \"{key}\" from array.");
                        }
                    case VariableType.Entity:
                        {

                            return new MeVariable() { Value = new Property(var.ToEntity(), key), Type = VariableType.Property };
                        }

                }
                throw new MeException($"Attempting to retrieve undefined property \"{key}\" from variable \"{var}\"");
            }
            , new Validator(
                (variables, operation) =>
                {
                    MeVariable var = variables[0];
                    string key = variables[1].ToMeString();
                    switch (var.Type)
                    {
                        case VariableType.Array:
                            {
                                return key.Equals(LConstants.ARR_LENGTH);
                            }
                        case VariableType.Entity:
                            {
                                Entity ent = var.ToEntity();
                                return ent.HasProperty(key);
                            }
                        default:
                            {
                                return false;
                            }
                    }
                }), 2);
        }
    }
}