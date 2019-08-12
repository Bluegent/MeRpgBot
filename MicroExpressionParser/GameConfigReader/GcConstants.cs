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
        public const string KEY = "key";
        public const string NAME = "name";
        public const string DESC = "description";
        public const string FORMULA = "formula";

        //statuses
        public const string INTERVAL = "interval";

        public const string MAX_STACK = "max_stacks";
        public const string STACK_TYPE = "stacking";


        public class Defaults
        {
            public const string DESC = "";
        }
    }
}
