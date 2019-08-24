using RPGEngine.Core;
using RPGEngine.Game;
using System.Collections.Generic;

namespace RPGEngine.Entities
{

    public abstract class Entity
    {
        public const string HP_KEY = "HP";

        public string Name { get; set; }
        public string Key { get; set; }
        public Dictionary<string, EntityAttribute> Attributes { get; protected set; }
        public Dictionary<string, ResourceInstance> ResourceMap { get; protected set; }


        public Entity()
        {
            Attributes = new Dictionary<string, EntityAttribute>();
            ResourceMap = new Dictionary<string, ResourceInstance>();
        }

        public bool Free { get; protected set; }

        public abstract double TakeDamage(double amount, DamageType type, Entity source, bool periodic = true);

        public abstract double GetHealed(double amount, Entity source, bool log = true);

        public abstract bool Cast(Entity target, string skillKey);

        public abstract BaseProperty GetProperty(string key);

        public abstract void ApplyStatus(StatusTemplate status, Entity source, double duration, double[] values);

        public abstract void Update();

        public abstract void Cleanse();

        public abstract void AddPushback(long delay);

        public abstract void InterruptCasting();

        public abstract bool HasProperty(string propertyKey);

        public abstract void RegenResources(long deltaTMs);


        public ResourceInstance GetResource(string key)
        {
            return ResourceMap.ContainsKey(key) ? ResourceMap[key] : null;
        }

        public void AddAttribute(string key, double value)
        {
            if (Attributes.ContainsKey(key))
                return;
            Attributes.Add(key, new EntityAttribute(value));
            ResetAttributes();
        }

        public void AddResource(ResourceTemplate resource)
        {
            if (ResourceMap.ContainsKey(resource.Key))
                return;
            ResourceInstance resIn = new ResourceInstance(resource,this);
            ResourceMap.Add(resource.Key,resIn); 
        }

        protected void ResetAttributes()
        {
            foreach (KeyValuePair<string, EntityAttribute> pair in Attributes)
            {
                pair.Value.Refresh();
            }
        }


    }

    public class Property
    {
        private readonly Entity _reference;
        private readonly string _key;

        public double Value => _reference.GetProperty(_key).Value;

        public Property(Entity entity, string key)
        {
            _reference = entity;
            _key = key;
        }
    }
}
