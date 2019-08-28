

using System.Collections.Generic;
using RPGEngine.Game;

namespace RPGEngine.Entities
{
    using RPGEngine.Templates;

    public class BaseProperty
    {
        public double Value { get; set; }
    }

    public class EntityAttribute : BaseProperty
    {
        public double Base { get; set; }
        public double FromPoints { get; set; }
        public List<StatModifier> Modifiers { get; }

        public EntityAttribute(double value = 0)
        {
            Base = value;
            Modifiers = new List<StatModifier>();
            Refresh();
        }

        public void Refresh()
        {
            Value = Base + FromPoints;
            foreach (StatModifier mod in Modifiers)
            {
                Value += mod.Amount;
            }
        }
    }
}
