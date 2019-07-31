using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MicroExpressionParser
{
    public partial class DataCollection
    {
        [JsonProperty("base_value_keys")]
        [DefaultValue(null)]
        public List<string> BaseValueKeys { get; set; }

        [JsonProperty("attributes_keys")]
        [DefaultValue(null)]
        public List<string> AttributesKeys { get; set; }

        [JsonProperty("damage_types")]
        [DefaultValue(null)]
        public List<DamageType> DamageTypes { get; set; }

        [JsonProperty("compound_stats")]
        [DefaultValue(null)]
        public List<CompoundStat> CompundStats { get; set; }

        [JsonProperty("entity_resource")]
        [DefaultValue(null)]
        public List<CompoundStat> EntityResource { get; set; }

        [JsonProperty("status_effects")]
        [DefaultValue(null)]
        public List<StatusEffect> StatusEffects { get; set; }

        [JsonProperty("skills")]
        [DefaultValue(null)]
        public List<Skill> Skills { get; set; }

        [JsonProperty("classes")]
        [DefaultValue(null)]
        public List<Class> Classes { get; set; }

        public static DataCollection FromJson(string json)
        {
            if (json == null || json == "")
            {
                return null;
            }
            DataCollection result;
            try
            {
                result = JsonConvert.DeserializeObject<DataCollection>(json);
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
