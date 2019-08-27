namespace RPGEngine.Core
{
    using System.Collections.Generic;

    using RPGEngine.GameConfigReader;

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

    }
}
