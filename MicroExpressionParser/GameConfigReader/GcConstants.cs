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

        public static class Statuses
        {
            public const string INTERVAL = "interval";
            public const string MAX_STACK = "max_stacks";
            public const string STACK_TYPE = "stacking";
        }


        //skills
        public static class Skills
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
            public const string INTERVAL = "interval";

            public const string DEFAULT_TYPE = "cast";
            public const string DEFAULT_CAST_DURATION = "0";
            public const string DEFAULT_COOLDOWN = "1";
            public const string DEFAULT_COST_VALUE = "1";
            public const string DEFAULT_INTERVAL_VALUE = "0";

            public const long DEFAULT_NEEDED_LEVEL = 0;
            public const bool DEFAULT_INTERRUPT = true;
            public const bool DEFAULT_PUSHBACK = true;


        }


        public static class Resources
        {
            public const string REGEN = "regen";
            public const string INTERVAL = "interval";
            public const string MODIFIER = "start_percentage";

            public const long DEFAULT_INTERVAL = 1;
            public const double DEFAULT_MODIFIER = 1;

            public const double DEFAULT_REGEN = 0;
        }

        public static class Classes
        {
            public const string BASE_VALUES = "base_values";
            public const string BASIC_ATTRIBUTES = "basic_attributes";
            public const string SKILLS = "skills";
            public const string BASE_ATTACK = "base_attack";
            public const string RESOURCES = "resources";
        }

        public static class Defaults
        {
            public const string DESC = "";
        }
    }
}
