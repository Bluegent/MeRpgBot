using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGEngine.GameConfigReader
{
    public class GcConstants
    {
        //general

        public class General
        {
            public const string KEY = "key";
            public const string NAME = "name";
            public const string DESC = "description";
            public const string FORMULA = "formula";
            public const string VALUE = "value";
        }

        //statuses

        public class Statuses
        {
            public const string INTERVAL = "interval";
            public const string MAX_STACK = "max_stacks";
            public const string STACK_TYPE = "stacking";
        }


        //skills
        public class Skills
        {
            public const string ALIASES = "aliases";
            public const string SKILL_TYPE = "type";
            public const string VALUES_BY_LEVEL = "values_by_level";
            public const string COOLDOWN = "cooldown";
            public const string NEEDED_LEVEL = "needed_level";
            public const string CAST_DURATION = "duration";
            public const string PUSH_BACK = "pushback";
            public const string INTERRUPT = "interrupt";
            public const string THREAT = "threat";
            public const string COST = "cost";

            public const string DEFAULT_TYPE = "cast";
        }



        public class Defaults
        {
            public const string DESC = "";
        }
    }
}
