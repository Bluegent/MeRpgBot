
using RPGEngine.GameConfigReader;
using RPGEngine.Parser;
using RPGEngine.Entities;
using RPGEngine.Cleanup;

namespace RPGEngine.Templates
{
    using RPGEngine.Core;

    public class DamageTypeTemplate : BaseObject
    {
        public MeNode Mitigation { get; set; }
        public MeNode Dodge { get; set; }
        public MeNode CriticalChance { get; set; }
        public MeNode CriticalModifier { get; set; }

        public DamageTypeTemplate(IGameEngine engine, string mitigation, string dodge, string crit, string critmod)
        {
            Mitigation = mitigation != null ? TreeConverter.Build(mitigation, engine) : null;
            Dodge = dodge != null ? TreeConverter.Build(dodge, engine) : null;
            CriticalChance = crit != null ? TreeConverter.Build(crit, engine) : null;
            CriticalModifier = critmod != null ? TreeConverter.Build(critmod, engine) : null;
        }

        public bool GetDodge(Entity source, Entity target)
        {
            return Dodge != null && Utils.Utility.Chance(Sanitizer.ReplaceTargetAndSource(Dodge, source, target).Resolve().Value.ToDouble());
        }

        public double GetMitigatedAmount(double amount, Entity source, Entity target)
        {
            bool crited;
            if (CriticalChance != null)
            {
                MeNode resolvedCritChance = Sanitizer.ReplaceTargetAndSource(CriticalChance, source, target)
                    .Resolve();

                crited = Utils.Utility.Chance(resolvedCritChance.Value.ToDouble());
            }
            else
            {
                crited = false;
            }

            double mutliplier = 1.0;
            if (crited)
            {
                if(CriticalModifier!=null)
                    mutliplier = Sanitizer.ReplaceTargetAndSource(CriticalModifier, source, target)
                    .Resolve().Value.ToDouble();
            }

            double finalAmount = mutliplier * amount;
            if (Mitigation != null)
            {
                MeNode mitigation = Sanitizer.SanitizeMitigation(Mitigation, target, source, finalAmount)
                    .Resolve();
                return mitigation.Value.ToDouble();
            }
            else
            {
                return finalAmount;
            }

        }
    }
}
