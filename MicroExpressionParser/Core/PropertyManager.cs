namespace RPGEngine.Core
{
    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    using RPGEngine.GameConfigReader;
    using RPGEngine.Utils;

    public class PropertyManager
    {
        public Dictionary<string, BaseObject> Attributes { get; set; }
        public Dictionary<string, BaseObject> BaseValues { get; set; }
        public Dictionary<string,ResourceTemplate> Resources { get; set; }

        public IGameEngine Engine { get; set; }

        public PropertyManager()
        {
            Attributes = new Dictionary<string, BaseObject>();
            BaseValues = new Dictionary<string, BaseObject>();
            Resources = new Dictionary<string, ResourceTemplate>();
        }

        public bool HasAttribute(string key)
        {
            return Attributes.ContainsKey(key);
        }

        public void AddAttribute(BaseObject attribute)
        {
            if (Attributes.ContainsKey(attribute.Key))
                throw new MeException($"Duplicate attribute {attribute.Name}.");
            Attributes.Add(attribute.Key, attribute);
        }

        public bool HasBaseValue(string key)
        {
            return BaseValues.ContainsKey(key);
        }

        public void AddBaseValue(BaseObject baseValue)
        {
            if (BaseValues.ContainsKey(baseValue.Key))
                throw new MeException($"Duplicate base value {baseValue.Name}.");
            BaseValues.Add(baseValue.Key, baseValue);
        }

        public bool HasResource(string key)
        {
            return Resources.ContainsKey(key);
        }

        public void AddResource(ResourceTemplate resource)
        {
            if (Resources.ContainsKey(resource.Key))
                throw new MeException($"Duplicate resource {resource.Name}.");
            Resources.Add(resource.Key,resource);
        }

        public ResourceTemplate GetResource(string key)
        {
            return Resources.ContainsKey(key) ? Resources[key] : null;
        }

        public void LoadAttributesFromPath(string path)
        {
            JArray attributeJson = FileReader.FromPath<JArray>(path);
            foreach(JToken attribute in attributeJson)
            {
                if (attribute.Type != JTokenType.Object)
                {
                    throw new MeException($"Expected a json object \"{path}\"at  \"{attribute}\".");
                }
                BaseObject newAttribute = new BaseObject();
                newAttribute.LoadBase(attribute.ToObject<JObject>());
                AddAttribute(newAttribute);
            }
        }

        public void LoadBaseValuesFromPath(string path)
        {
            JArray baseValueArray = FileReader.FromPath<JArray>(path);
            foreach (JToken baseValue in baseValueArray)
            {
                if (baseValue.Type != JTokenType.Object)
                {
                    throw new MeException($"Expected a json object \"{path}\"at  \"{baseValue}\".");
                }
                BaseObject newBaseValue = new BaseObject();
                newBaseValue.LoadBase(baseValue.ToObject<JObject>());
                AddBaseValue(newBaseValue);
            }
        }


        public void LoadResourcesFromPath(string path)
        {
            JArray resourceJson = FileReader.FromPath<JArray>(path);
            ResourceReader reader = new ResourceReader(Engine);

            foreach (JToken resourceValue in resourceJson)
            {
                if (resourceValue.Type != JTokenType.Object)
                {
                    throw new MeException($"Expected a json object \"{path}\"at  \"{resourceValue}\".");
                }

                ResourceTemplate newResource = reader.FromJson(resourceValue.ToObject<JObject>());
                AddResource(newResource);
            }
        }


    }

    
}
