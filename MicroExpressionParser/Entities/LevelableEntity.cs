
using RPGEngine.Core;

namespace RPGEngine.Entities
{
    using System.Linq;

    using Newtonsoft.Json.Linq;

    using RPGEngine.GameConfigReader;
    using RPGEngine.GameInterface;

    public class LevelableEntity : BaseEntity, IJsonSerializable
    {
        public int Level { get; private set; }
        public int AttributePoints { get; private set; }
        public double CurrentExp { get; set; }
        public double CurrentLevelMaxExp { get; set; }

        public LevelableEntity(IGameEngine engine) : base(engine)
        {
            Level = 0;
            AttributePoints = 0;
            CurrentExp = 0;
            CurrentLevelMaxExp = Engine.GetMaxExp(Level);
        }

        public void AddExp(long amount)
        {
            CurrentExp += amount;
            if(CurrentExp >= CurrentLevelMaxExp)
                LevelUp();
        }

        private void LevelUp()
        {
            ++Level;
            ++AttributePoints;
            CurrentExp -= CurrentLevelMaxExp;
            CurrentLevelMaxExp = Engine.GetMaxExp(Level);
        }

        public bool AssignAttributePoint(string key)
        {
            if (AttributePoints == 0)
            {
                //log that you don't have attribute points
                return false;
            }

            if (!Attributes.ContainsKey(key))
            {
                //log that you don't have that attribute
                return false;
            }

            ++Attributes[key].FromPoints;
            RefreshProperties();

            return true;
        }

        public JObject ToJObject()
        {
            JObject result = new JObject();
            result.Add(GcConstants.General.NAME,Name);
            result.Add(GcConstants.General.LEVEL,Level);
            result.Add(GcConstants.General.CURRENT_EXP,CurrentExp);
            result.Add(GcConstants.General.ATTRIBUTE_POINTS,AttributePoints);
            JArray attributes=new JArray();
            foreach (EntityAttribute attr in Attributes.Values)
            {
                attributes.Add(attr.ToJObject());
            }
            result.Add(GcConstants.Classes.ATTRIBUTES,attributes);
            JArray resources = new JArray();
            foreach (var resource in ResourceMap.Values)
            {
                resources.Add(resource.ToJObject());
            }
            result.Add(GcConstants.Classes.RESOURCES,resources);
            JArray skills = new JArray();
            foreach (var skill in Skills.Values)
            {
                skills.Add(skill.ToJObject());
            }
            result.Add(GcConstants.Classes.SKILLS, skills);
            return result;
        }

        public bool FromJObject(JObject obj, IGameEngine engine)
        {
            string name = obj[GcConstants.General.NAME].ToObject<string>();
            int level = obj[GcConstants.General.LEVEL].ToObject<int>();
            double currentExp = obj[GcConstants.General.CURRENT_EXP].ToObject<double>();
            int attributePoints = obj[GcConstants.General.ATTRIBUTE_POINTS].ToObject<int>();
            Name = name;
            Level = level;
            CurrentExp = currentExp;
            AttributePoints = attributePoints;
            JToken[] attributesArray = obj[GcConstants.Classes.ATTRIBUTES].ToArray();
            foreach (JToken attribute in attributesArray)
            {
                string key = attribute[GcConstants.General.KEY].ToObject<string>();
                if (Attributes.ContainsKey(key))
                {
                    Attributes[key].FromJObject(attribute.ToObject<JObject>(), Engine);
                }
            }
            JToken[] resourcesArray = obj[GcConstants.Classes.RESOURCES].ToArray();
            foreach (JToken resource in resourcesArray)
            {
                string key = resource[GcConstants.General.KEY].ToObject<string>();
                if (ResourceMap.ContainsKey(key))
                {
                    ResourceMap[key].FromJObject(resource.ToObject<JObject>(), Engine);
                }
            }
            JToken[] skillsArray = obj[GcConstants.Classes.SKILLS].ToArray();
            foreach (JToken skill in skillsArray)
            {
                string key = skill[GcConstants.General.KEY].ToObject<string>();
                if (Skills.ContainsKey(key))
                {
                    Skills[key].FromJObject(skill.ToObject<JObject>(), Engine);
                }
            }
            return true;
        }
    }
}
