using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGEngine.Core
{
    public class ResourceTemplate
    {
        public string Key { get; }
        public MeNode Formula { get; }

        public ResourceTemplate(string key, MeNode formula)
        {
            Key = key;
            Formula = formula;
        }

        public long ResolveMaxAmount(Entity target)
        {
            long amount = 0;
            return amount;
        }

    }
}
