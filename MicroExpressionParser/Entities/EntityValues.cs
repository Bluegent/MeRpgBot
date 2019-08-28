

using System.Collections.Generic;
using RPGEngine.Game;

namespace RPGEngine.Entities
{
    using Newtonsoft.Json.Linq;

    using RPGEngine.Core;
    using RPGEngine.GameConfigReader;
    using RPGEngine.GameInterface;
    using RPGEngine.Managers;
    using RPGEngine.Templates;

    public class BaseProperty
    {
        public double Value { get; set; }
    }

    public class EntityAttribute : BaseProperty, IJsonSerializable
    {
        public string Key { get; set; }
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

        public JObject ToJObject()
        {
            JObject result=new JObject();
            result.Add(GcConstants.General.KEY,Key);
            result.Add(GcConstants.General.FROM_POINTS,FromPoints);
            return result;
        }

        public bool FromJObject(JObject obj, IGameEngine engine)
        {
            string key = obj[GcConstants.General.KEY].ToObject<string>();
            if (!engine.GetPropertyManager().HasAttribute(key))
                return false;
            double fromPoints = obj[GcConstants.General.FROM_POINTS].ToObject<double>();
            Key = key;
            FromPoints = fromPoints;
            return true;
        }
    }
}
