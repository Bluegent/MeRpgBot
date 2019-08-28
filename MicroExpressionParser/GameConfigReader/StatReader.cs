using Newtonsoft.Json.Linq;
using RPGEngine.Core;
using RPGEngine.Game;
using RPGEngine.Parser;
using RPGEngine.Utils;

namespace RPGEngine.GameConfigReader
{
    using RPGEngine.Templates;

    public class StatReader
    {
        private IGameEngine _engine;

        public StatReader(IGameEngine engine)
        {
            _engine = engine;
        }

        public StatTemplate FromJson(JObject json)
        {
            StatTemplate result = new StatTemplate();

            result.LoadBase(json);

            string formulaString =
                JsonUtils.ValidateJsonEntry(GcConstants.General.FORMULA, json, JTokenType.String, $"Missing formula for stat {result.Name}.").ToString();

            result.Formula = TreeConverter.Build(formulaString, _engine);

            return result;
        }

    }
}