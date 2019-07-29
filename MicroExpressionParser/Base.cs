using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroExpressionParser
{
    public enum VariableType
    {
        Function,
        NumericValue,
        Variable,
        Entity,
        Stat,
        Operator
    }

    public class MeVariable
    {
        public VariableType Type { get; set; }
        public Object Value { get; set; }
    }
    class Base
    {
    }
}
