
namespace RPGEngine.GameInterface
{
    using Newtonsoft.Json.Linq;

    using RPGEngine.Core;
    using RPGEngine.Managers;

    interface IJsonSerializable
    {
        JObject ToJObject();

        bool FromJObject(JObject obj,IGameEngine engine);
    }
}
