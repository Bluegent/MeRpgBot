namespace RPGEngine.GameConfigReader
{
    using System.Globalization;
    using System.Runtime.InteropServices;

    using Newtonsoft.Json.Linq;

    using RPGEngine.Core;
    using RPGEngine.Parser;
    using RPGEngine.Templates;
    using RPGEngine.Utils;

    public class ResourceReader
    {

        public IGameEngine Engine { get; }

        public ResourceReader(IGameEngine engine)
        {
            Engine = engine;
        }

        public ResourceTemplate FromJson(JObject json)
        {
            ResourceTemplate result = new ResourceTemplate();
            result.LoadBase(json);

            //get formula for calculating max amount
            string maxValue = JsonUtils.ValidateJsonEntry(
                GcConstants.General.FORMULA,
                json,
                JTokenType.String,
                $"Unknown formula for resource {result.Name}").ToString();
            
            result.Formula = TreeConverter.Build(maxValue,Engine);

            //get formula for calculating regen
            string regenValue = JsonUtils.GetValueOrDefault(
                json,
                GcConstants.Resources.REGEN,
                GcConstants.Resources.DEFAULT_REGEN.ToString(CultureInfo.InvariantCulture));

            result.RegenFormula = TreeConverter.Build(regenValue, Engine);

            //get interval
            string intervalValue = JsonUtils.GetValueOrDefault<string>(json, GcConstants.Resources.INTERVAL, GcConstants.Resources.DEFAULT_INTERVAL.ToString());
            result.RegenInterval = TreeConverter.Build(intervalValue, Engine);

            //get modifier
            string modifierValue = JsonUtils.GetValueOrDefault<string>(
                json,
                GcConstants.Resources.MODIFIER,
                GcConstants.Resources.DEFAULT_MODIFIER.ToString());
            result.StartMod = TreeConverter.Build(modifierValue,Engine);

            return result;
        }
    }
}
