using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MicroExpressionParser
{
    public partial class ValueByLevel
    {
        [JsonProperty("cooldown")]
        [DefaultValue(null)]
        public string Cooldown { get; set; }

        [JsonProperty("formula")]
        [DefaultValue(null)]
        public string Formula { get; set; }

        [JsonProperty("needed_level")]
        [DefaultValue(0)]
        public long NeededLevel { get; set; }

        [JsonProperty("duration", NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public string Duration { get; set; }

        [JsonProperty("interval", NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public string Interval { get; set; }

        public static ValueByLevel FromJson(string json)
        {
            if (json == null || json == "")
            {
                return null;
            }
            ValueByLevel result;
            try
            {
                result = JsonConvert.DeserializeObject<ValueByLevel>(json);
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

