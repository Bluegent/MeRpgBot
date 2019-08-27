namespace RPGEngine.Game
{
    using System.Collections.Generic;

    using RPGEngine.Cleanup;
    using RPGEngine.Core;
    using RPGEngine.Entities;
    using RPGEngine.GameConfigReader;

    public class StatTemplate : BaseObject
    {
        public MeNode Formula { get; set; }

        public StatTemplate()
            : base()
        {

        }

        public double ResolveValue(Entity parent)
        {
            return Sanitizer.ReplacePropeties(Formula, parent).Resolve().Value.ToDouble();
        }
    }

    public class StatInstance :BaseProperty
    {
        public StatTemplate Stat { get; set; }
        public double Base { get; private set; }

        public List<StatModifier> Modifiers { get; set; }
        private Entity _parent;

        public StatInstance(Entity parent, StatTemplate stat)
        {
            _parent = parent;
            Stat = stat;
            Modifiers = new List<StatModifier>();
            Refresh();
        }

        public void Refresh()
        {
            Base = Stat.ResolveValue(_parent);
            Value = Base;
            foreach (StatModifier mod in Modifiers)
            {
                Value += mod.Amount;
            }
        }


    }
}