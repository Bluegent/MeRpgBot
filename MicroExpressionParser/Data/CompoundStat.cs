using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace RPGEngine
{
    public partial class CompoundStat
    {
        [JsonProperty("StatKey")]
        [DefaultValue(null)]
        public string Key { get; set; }

        [JsonProperty("formula")]
        [DefaultValue(null)]
        public string Formula { get; set; }

        public static CompoundStat FromJson(string json)
        {
            if (json == null || json == "")
            {
                return null;
            }
            CompoundStat result;
            try
            {
                result = JsonConvert.DeserializeObject<CompoundStat>(json);
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
