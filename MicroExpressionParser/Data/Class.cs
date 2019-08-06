using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
namespace MicroExpressionParser
{
    public partial class Class
    {
        [JsonProperty("StatKey")]
        [DefaultValue(null)]
        public string Key { get; set; }

        [JsonProperty("base_values")]
        [DefaultValue(null)]
        public List<BaseValue> BaseValues { get; set; }

        [JsonProperty("basic_attributes")]
        [DefaultValue(null)]
        public List<BaseValue> BasicAttributes { get; set; }

        [JsonProperty("skills")]
        [DefaultValue(null)]
        public List<string> Skills { get; set; }

        public static Class FromJson(string json)
        {
            if (json == null || json == "")
            {
                return null;
            }
            Class result;
            try
            {
                result = JsonConvert.DeserializeObject<Class>(json);
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
