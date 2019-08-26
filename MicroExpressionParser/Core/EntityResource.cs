using RPGEngine.Cleanup;
using RPGEngine.Entities;
using RPGEngine.Game;

namespace RPGEngine.Core
{
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

    public class ResourceInstance : BaseProperty
    {
        public ResourceTemplate Resource { get; }
        public Entity Parent { get; }
        public double MaxAmount { get; private set; }
        public double RegenAmount { get; private set; }
        public long LastRegenTick { get; set; }
        public long RegenInterval { get; set; }
        public double Modifier { get; set; }

        public ResourceInstance(ResourceTemplate template, Entity parent)
        {
            Resource = template;
            Parent = parent;
            Refresh();
            Value = MaxAmount * Modifier;
            LastRegenTick = 0;
        }

        public void Replentish()
        {
            Value = MaxAmount * Modifier;
        }

        public void Refresh()
        {
            MaxAmount = Resource.ResolveMaxAmount(Parent);
            RegenAmount = Resource.ResolveRegen(Parent);
            RegenInterval = Resource.ResolveInterval(Parent);
            Modifier = Resource.ResolveModifier(Parent);

        }

        private void Clamp()
        {
            Value = Utils.Utility.Clamp(Value, 0, MaxAmount);
        }

        public void Regen(long now)
        {
            if (now - LastRegenTick * GameConstants.TickTime >= RegenInterval)
            {
                Value += RegenAmount;
                Clamp();
                LastRegenTick = now;
            }
        }

        public bool CanCast(double amount)
        {
            return Value >= amount;
        }

        public void Cast(double amount)
        {
            Value -= amount;
        } 
    }
}
