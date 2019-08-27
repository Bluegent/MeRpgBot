
using Newtonsoft.Json.Linq;
using RPGEngine.Core;
using RPGEngine.GameConfigReader;

namespace RPGEngine.Utils
{
    public class JsonUtils
    {
        public static JToken ValidateJsonEntry(string key, JObject json, JTokenType type, string failureMessage = null)
        {
            string msg = failureMessage ?? $"Entry {key} is missing or is the wrong type( expected: {type})";
            if (json.ContainsKey(key) &&( json[key].Type == JTokenType.String || json[key].Type == type))
            {
                return json[key];
            }
           
            throw new MeException(msg);
        }

        public static T GetValueOrDefault<T>(JObject json, string key, T defaultValue)
        {
            return json.ContainsKey(key) ? json[key].ToObject<T>() :defaultValue;
        }
    }
}
