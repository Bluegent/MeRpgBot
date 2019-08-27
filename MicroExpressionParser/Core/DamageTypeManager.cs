namespace RPGEngine.Core
{
    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    using RPGEngine.Game;
    using RPGEngine.Utils;

    public class DamageTypeManager
    {
        public Dictionary<string, DamageTypeTemplate> DamageTypes { get; }

        public IGameEngine Engine { get; set; }

        public DamageTypeManager()
        {
            DamageTypes = new Dictionary<string, DamageTypeTemplate>();
        }

        public bool HasDamageType(string key)
        {
            return DamageTypes.ContainsKey(key);
        }

        public void AddDamageType(DamageTypeTemplate damageType)
        {
            if (DamageTypes.ContainsKey(damageType.Key))
            {
                throw new MeException($"Attempted to add a damage type with key {damageType.Key} but it already exists");
            }
            DamageTypes.Add(damageType.Key,damageType);
        }

        public DamageTypeTemplate GetDamageType(string key)
        {
            return DamageTypes.ContainsKey(key) ? DamageTypes[key] : null;
        }

        public void LoadDamageTypesFromfile(string path)
        {
            //DamageTypeReader reader = new DamageTypeReader(Engine);
            JArray damageTypeJson = FileReader.FromPath<JArray>(path);

            foreach (JToken damageValue in damageTypeJson)
            {
                if (damageValue.Type != JTokenType.Object)
                {
                    throw new MeException($"Expected a json object \"{path}\"at  \"{damageValue}\".");
                }

                //DamageTypeTemplate newSkill = reader.FromJson(damageValue.ToObject<JObject>());
                //AddDamageType(newSkill);
            }
        }
    }
}