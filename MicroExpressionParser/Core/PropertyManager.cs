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
            JObject attributeJson = FileReader.FromPath(path);
            if (attributeJson.Type != JTokenType.Array)
            {
                throw new MeException($"Expeted attribute array in \"{path}\".");
            }
            JArray attributeArray = attributeJson.ToObject<JArray>();
            foreach(JToken attribute in attributeArray)
            {
                if (attribute.Type != JTokenType.Object)
                {

                }
            }
        }

    }
}
