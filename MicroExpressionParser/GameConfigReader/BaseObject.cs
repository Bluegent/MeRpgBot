
using Newtonsoft.Json.Linq;

namespace RPGEngine.GameConfigReader
{
    public class BaseObject
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public void LoadBase(JObject json)
        {
            Key = json[GcConstants.General.KEY].ToString();
            Name = json.ContainsKey(GcConstants.General.NAME) ? json[GcConstants.General.NAME].ToString() : Key;
            Description = json.ContainsKey(GcConstants.General.DESC) ? json[GcConstants.General.DESC].ToString() : GcConstants.Defaults.DESC;
        }
    }
}
