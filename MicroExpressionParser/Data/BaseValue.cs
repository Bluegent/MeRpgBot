using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MicroExpressionParser
{
    public partial class BaseValue
    {
        [JsonProperty("key")]
        [DefaultValue(null)]
        public string Key { get; set; }

        [JsonProperty("value")]
        [DefaultValue(0)]
        public long Value { get; set; }

        public static BaseValue FromJson(string json)
        {
            if (json == null || json == "")
            {
                return null;
            }
            BaseValue result;
            try
            {
                result = JsonConvert.DeserializeObject<BaseValue>(json);
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
