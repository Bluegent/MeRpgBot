using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Key = json[GcConstants.KEY].ToString();
            Name = json.ContainsKey(GcConstants.NAME) ? json[GcConstants.NAME].ToString() : Key;
            Description = json.ContainsKey(GcConstants.DESC) ? json[GcConstants.DESC].ToString() : GcConstants.Defaults.DESC;
        }
    }
}
