﻿using System;
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
            public const string LEVEL = "level";
            public const string FROM_POINTS = "from_points";
            public const string ID = "id";
            public const string CURRENT_EXP = "current_exp";
            public const string ATTRIBUTE_POINTS = "attribute_points";
            public const string CLASS = "class";
            public const string ENTITY = "entity";
            public const string BASE_SKILL = "base_skill";
            public const string DEAD = "dead";
            public const string REVIVE_TIME = "revive_time";
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
            public const string DEFAULT_COST_VALUE = "0";
            public const string DEFAULT_INTERVAL_VALUE = "0";

            public const long DEFAULT_NEEDED_LEVEL = 0;
            public const bool DEFAULT_INTERRUPT = true;
            public const bool DEFAULT_PUSHBACK = true;


        }

        public static class DamageType
        {
            public const string MITIGATION = "mitigation";
            public const string DODGE = "dodge";
            public const string CRIT = "crit";
            public const string CRIT_MULT = "crit_multiplier";
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
            public const string ATTRIBUTES = "attributes";
        }


        public static class Core
        {
            public const string REVIVE_TIME = "revive_time";
            public const string DEFAULT_SKILL_THREAT = "skill_threat";
            public const string ATTRIBUTES_PER_LEVEL = "attribute_points_per_level";
            public const string LEVEL_ONE_EXP = "start_exp";
            public const string EXP_FORMULA = "exp_formula";

            public const string MAX_LEVEL = "max_level";
            public const long DEFAULT_THREAT = 1;
            public const long DEFAULT_ATTRIBUTE_POINTS = 1;
            public const long DEFAULT_MAX_LEVEL = 1;
        }

        public static class Defaults
        {
            public const string DESC = "";
        }
    }
}
