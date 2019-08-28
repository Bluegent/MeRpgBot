namespace RPGEngine.Templates
{
    using RPGEngine.Cleanup;
    using RPGEngine.Core;
    using RPGEngine.Entities;
    using RPGEngine.GameConfigReader;

    public class ResourceTemplate : BaseObject
    {
        public MeNode StartMod { get; set; }
        public MeNode Formula { get; set; }
        public MeNode RegenFormula { get; set; }
        public MeNode RegenInterval { get; set; }

        public ResourceTemplate()
        {
            StartMod = new MeNode(GcConstants.Resources.DEFAULT_MODIFIER);
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

        public long ResolveInterval(Entity target)
        {
            MeNode entityNode = Sanitizer.ReplacePropeties(RegenInterval, target);
            return entityNode.Resolve().Value.ToLong();
        }

        public double ResolveModifier(Entity target)
        {
            MeNode entityNode = Sanitizer.ReplacePropeties(StartMod, target);
            return entityNode.Resolve().Value.ToDouble();
        }

    }
}