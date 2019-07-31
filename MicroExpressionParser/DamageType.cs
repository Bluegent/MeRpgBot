using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MicroExpressionParser
{
    public partial class DamageType
    {
        [JsonProperty("key")]
        [DefaultValue(null)]
        public string Key { get; set; }

        [JsonProperty("name")]
        [DefaultValue(null)]
        public string Name { get; set; }

        [JsonProperty("mitigation")]
        [DefaultValue(null)]
        public string MitigationFormula { get; set; }

        public static DamageType FromJson(string json)
        {
            if (json == null || json == "")
            {
                return null;
            }
            DamageType result;
            try
            {
                result = JsonConvert.DeserializeObject<DamageType>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                result = null;
            }
            return result;
        }
    }
}
