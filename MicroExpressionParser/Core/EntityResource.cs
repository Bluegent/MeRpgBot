using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGEngine.Core
{
    using System.ComponentModel;

    using MicroExpressionParser;

    public class ResourceTemplate
    {
        public string Key { get; }
        public MeNode Formula { get; }
        public MeNode RegenFormula { get; }

        public ResourceTemplate(string key, MeNode formula)
        {
            Key = key;
            Formula = formula;
        }

        public double ResolveRegen(Entity target)
        {
            MeNode entityNode = Sanitizer.ReplacePropeties(RegenFormula, target);
            return entityNode.Resolve().Value.ToDouble();
        }

        public double ResolveMaxAmount(Entity target)
        {
            MeNode entityNode = Sanitizer.ReplacePropeties(Formula, target);
            return entityNode.Resolve().Value.ToDouble();
        }

    }

    public class ResourceInstance
    {
        public ResourceTemplate Resource { get; }
        public double CurrentAmount { get; set; }
        public double MaxAmount { get; private set; }
        public double RegenAmount { get; private set; }

        public void Refresh(Entity target)
        {
            MaxAmount = Resource.ResolveMaxAmount(target);
            RegenAmount = Resource.ResolveRegen(target);
        }

    }
}
