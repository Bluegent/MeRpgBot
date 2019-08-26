
using Newtonsoft.Json.Linq;
using RPGEngine.Core;
using RPGEngine.GameConfigReader;

namespace RPGEngine.Utils
{
    public class JsonUtils
    {
        public static JToken ValidateJsonEntry(string key, JObject json, JTokenType type, string failureMessage = null)
        {
            string msg = failureMessage ?? $"Entry {key} is missing or is the wrong tpye( expected: {type})";
            if (!json.ContainsKey(key) || json[key].Type != type)
            {
                throw new MeException(msg);
            }
            return json[key];
        }

        public static T GetValueOrDefault<T>(JObject json, string key, T defaultValue)
        {
            return json.ContainsKey(key) ? json[key].ToObject<T>() :defaultValue;
        }
    }
}
