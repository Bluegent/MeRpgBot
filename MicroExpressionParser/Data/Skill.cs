using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
namespace MicroExpressionParser
{
    public partial class Skill
    {
        [JsonProperty("key")]
        [DefaultValue(null)]
        public string Key { get; set; }

        [JsonProperty("name")]
        [DefaultValue(null)]
        public string Name { get; set; }

        [JsonProperty("aliases")]
        [DefaultValue(null)]
        public List<string> Aliases { get; set; }

        [JsonProperty("description")]
        [DefaultValue(null)]
        public string Description { get; set; }

        [JsonProperty("type")]
        [DefaultValue(null)]
        public string Type { get; set; }

        [JsonProperty("values_by_level")]
        [DefaultValue(null)]
        public List<ValueByLevel> ValuesByLevel { get; set; }

        public static Skill FromJson(string json)
        {
            if (json == null || json == "")
            {
                return null;
            }
            Skill result;
            try
            {
                result = JsonConvert.DeserializeObject<Skill>(json);
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
