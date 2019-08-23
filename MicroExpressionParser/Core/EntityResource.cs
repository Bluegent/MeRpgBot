using RPGEngine.Cleanup;
using RPGEngine.Entities;

namespace RPGEngine.Core
{
    

    public class ResourceTemplate
    {
        public string Key { get; }
        public MeNode Formula { get; }
        public MeNode RegenFormula { get; }

        public ResourceTemplate(string key, MeNode formula, MeNode regenFormula)
        {
            Key = key;
            Formula = formula;
            RegenFormula = regenFormula;
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
        public Entity Parent { get; }
        public double CurrentAmount { get; set; }
        public double MaxAmount { get; private set; }
        public double RegenAmount { get; private set; }

        public ResourceInstance(ResourceTemplate template, Entity parent)
        {
            Resource = template;
            Parent = parent;
            Refresh();
            CurrentAmount = MaxAmount;
        }

        public void Refresh()
        {
            MaxAmount = Resource.ResolveMaxAmount(Parent);
            RegenAmount = Resource.ResolveRegen(Parent);
        }

    }
}
