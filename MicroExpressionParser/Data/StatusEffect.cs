using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MicroExpressionParser
{
    public partial class StatusEffect
    {
        [JsonProperty("StatKey")]
        [DefaultValue(null)]
        public string Key { get; set; }

        [JsonProperty("name")]
        [DefaultValue(null)]
        public string Name { get; set; }

        [JsonProperty("description")]
        [DefaultValue(null)]
        public string Description { get; set; }

        [JsonProperty("max_stack")]
        [DefaultValue(0)]
        public long MaxStack { get; set; }

        [JsonProperty("formula")]
        [DefaultValue(null)]
        public string Formula { get; set; }

        public static StatusEffect FromJson(string json)
        {
            if (json == null || json == "")
            {
                return null;
            }
            StatusEffect result;
            try
            {
                result = JsonConvert.DeserializeObject<StatusEffect>(json);
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
