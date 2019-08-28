using RPGEngine.Core;
using RPGEngine.Game;
using System.Collections.Generic;
using RPGEngine.GameConfigReader;

namespace RPGEngine.Entities
{
    using System;
    using System.Dynamic;

    using RPGEngine.Cleanup;
    using RPGEngine.Templates;

    public abstract class Entity
    {
        public const string HP_KEY = "HP";
        public IGameEngine Engine { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public Dictionary<string, EntityAttribute> Attributes { get; protected set; }
        public Dictionary<string, ResourceInstance> ResourceMap { get; protected set; }
        public Dictionary<string, BaseProperty> BaseValueMap { get; protected set; }
        public Dictionary<string,StatInstance> Stats { get; protected set; }

        public MeNode ReviveDuration { get; set; }
        public long ResolvedReviveDuration { get; set; }
        public long ReviveTime { get; set; }

        public bool IsDead { get; set; }


        public Entity(IGameEngine engine)
        {
            Engine = engine;
            Attributes = new Dictionary<string, EntityAttribute>();
            ResourceMap = new Dictionary<string, ResourceInstance>();
            Stats = new Dictionary<string, StatInstance>();
            BaseValueMap = new Dictionary<string, BaseProperty>();
            ReviveDuration = new MeNode(0);
        }

        public void Kill()
        {
            ResourceMap[HP_KEY].Value = 0;
        }
        public abstract void Die();
        public abstract void Revive();


        public void CheckRevive()
        {
            if ( IsDead && Engine.GetTimer().GetNow() >= ReviveTime)
            {
                Revive();
                IsDead = false;
            }
        }

        public void CheckDeath()
        {
            if (!IsDead && ResourceMap[HP_KEY].Value <= 0)
            {
                Die();
                IsDead = true;
                ReviveTime = Engine.GetTimer().GetNow() + ResolvedReviveDuration * GameConstants.TickTime;
            }
        }

        public bool Free { get; protected set; }



        public abstract double TakeDamage(double amount, DamageTypeTemplate typeTemplate, Entity source, bool periodic = true);

        public abstract double GetHealed(double amount, Entity source, bool log = true);

        public abstract bool Cast(Entity target, string skillKey, bool autocast = false);

        public abstract BaseProperty GetProperty(string key);

        public abstract void ApplyStatus(StatusTemplate status, Entity source, double duration, double[] values);

        public abstract void Update();

        public abstract void Cleanse();

        public abstract void AddPushback(long delay);

        public abstract void InterruptCasting();

        public abstract bool HasProperty(string propertyKey);

        public abstract void RegenResources();


        public ResourceInstance GetResource(string key)
        {
            return ResourceMap.ContainsKey(key) ? ResourceMap[key] : null;
        }


        public StatInstance GetStat(string key)
        {
            return Stats.ContainsKey(key) ? Stats[key] : null;
        }

        public void AddBaseValue(string key, double value)
        {
            if (BaseValueMap.ContainsKey(key))
                return;
            BaseValueMap.Add(key, new BaseProperty() {Value =  value});
        }

        public void AddStat(StatTemplate stat)
        {
            if (Stats.ContainsKey(stat.Key))
                return;
            Stats.Add(stat.Key,new StatInstance(this,stat));
            RefreshProperties();
        }

        public void AddAttribute(string key, double value)
        {
            if (Attributes.ContainsKey(key))
                return;
            Attributes.Add(key, new EntityAttribute(value){Key = key});
            RefreshProperties();
        }

        public void AddResource(ResourceTemplate resource)
        {
            if (ResourceMap.ContainsKey(resource.Key))
                return;
            ResourceInstance resIn = new ResourceInstance(resource,this);
            ResourceMap.Add(resource.Key, resIn);
            RefreshProperties();
        }

        public void RefreshResources()
        {
            foreach (ResourceInstance res in ResourceMap.Values)
            {
                res.Refresh();
            }
        }

        public void RefreshAttributes()
        {
            foreach (EntityAttribute atr in Attributes.Values)
            {
                atr.Refresh();
            }
        }

        public void RefreshStats()
        {
            foreach (StatInstance stat in Stats.Values)
            {
                stat.Refresh();
            }
        }

        public void RefreshProperties()
        {
            RefreshAttributes();
            RefreshResources();
            RefreshStats();

            ResolvedReviveDuration = Sanitizer.ReplacePropeties(ReviveDuration, this).Resolve().Value.ToLong();
        }

        public string GetResourcesString()
        {
            string result = "";
            foreach (ResourceInstance resource in ResourceMap.Values)
            {
                result +=
                    $"{Utils.Utility.getBar(resource.Value, resource.MaxAmount)} {Utils.Utility.TruncateAndAlign(resource.Resource.Name,10),-15}\n";
            }

            return result;
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
