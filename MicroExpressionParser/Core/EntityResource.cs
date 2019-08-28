using Discord;

using RPGEngine.Entities;
using RPGEngine.Game;

namespace RPGEngine.Core
{
    using RPGEngine.Templates;

    public class ResourceInstance : BaseProperty
    {
        public ResourceTemplate Resource { get; }
        public Entity Parent { get; }
        public double MaxAmount { get; private set; }
        public double RegenAmount { get; private set; }
        public long NextRegenTick { get; set; }
        public long RegenInterval { get; set; }
        public double Modifier { get; set; }

        public ResourceInstance(ResourceTemplate template, Entity parent)
        {
            Resource = template;
            Parent = parent;
            Refresh();
            Value = MaxAmount * Modifier;
            NextRegenTick = 0;
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
            if (NextRegenTick == 0 || now >= NextRegenTick)
            {
                Value += RegenAmount;
                Clamp();
                NextRegenTick = now + RegenInterval * GameConstants.TickTime;
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
