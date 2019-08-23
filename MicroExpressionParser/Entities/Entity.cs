using RPGEngine.Core;
using RPGEngine.Game;
using RPGEngine.Parser;
using System.Collections.Generic;
using RPGEngine.Cleanup;
using RPGEngine.Language;
namespace RPGEngine.Entities
{
    public class EntityProperty
    {
        public string Key { get; set; }
        public double Value { get; set; }
    }
    public abstract class Entity
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public Dictionary<string, double> InteralAttributes { get; protected set; }
        public Dictionary<string, double> Attributes { get; protected set; }
        public Dictionary<string, ResourceInstance> ResourceMap { get; protected set; }


        public Entity()
        {
            InteralAttributes = new Dictionary<string, double>();
            ResourceMap = new Dictionary<string, ResourceInstance>();
        }

        public bool Free { get; protected set; }

        public abstract void TakeDamage(double amount, DamageType type, Entity source, bool periodic = true);

        public abstract void GetHealed(double amount, Entity source, bool log = true);

        public abstract bool Cast(Entity target, string skillKey);

        public abstract EntityProperty GetProperty(string key);

        public abstract void ApplyStatus(StatusTemplate status, Entity source, double duration, double[] values);

        public abstract void Update();

        public abstract void Cleanse();

        public abstract void AddPushback(long delay);

        public abstract void InterruptCasting();

        public abstract bool HasProperty(string propertyKey);

        public abstract void RegenResources(long deltaTMs);

        public void AddAttribute(string key, double value)
        {
            if (InteralAttributes.ContainsKey(key))
                return;
            InteralAttributes.Add(key,value);
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
            foreach (KeyValuePair<string, double> pair in InteralAttributes)
            {
                Attributes[pair.Key] = InteralAttributes[pair.Key];
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
