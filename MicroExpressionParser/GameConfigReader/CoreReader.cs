using RPGEngine.Core;
namespace RPGEngine.GameConfigReader
{
    using Newtonsoft.Json.Linq;

    using RPGEngine.GameConfigReader;
    using RPGEngine.Managers;
    using RPGEngine.Parser;
    using RPGEngine.Utils;

    public class CoreReader
    {
        private IGameEngine _engine;
        public CoreReader(IGameEngine engine)
        {
            _engine = engine;
        }

        public CoreManager FromJson(JObject json)
        {

            CoreManager manager = new CoreManager();

            //get max level 
            manager.MaxLevel = JsonUtils.GetValueOrDefault<long>(json, GcConstants.Core.MAX_LEVEL, GcConstants.Core.DEFAULT_MAX_LEVEL);

            //get attributes per level
            manager.AttributePointsPerLevel = JsonUtils.GetValueOrDefault<long>(json, GcConstants.Core.ATTRIBUTES_PER_LEVEL, GcConstants.Core.DEFAULT_ATTRIBUTE_POINTS);

            //get default skill threat
            manager.DefaultSkillThreat = JsonUtils.GetValueOrDefault<long>(json, GcConstants.Core.DEFAULT_SKILL_THREAT, GcConstants.Core.DEFAULT_THREAT);
            
            //get start experience
            manager.StartExp = JsonUtils.ValidateJsonEntry(
                GcConstants.Core.LEVEL_ONE_EXP,
                json,
                JTokenType.Integer,
                "Core config is missing experience amount necessary for the first level up.").ToObject<long>();

            //get revive time
            string reviveString = JsonUtils.ValidateJsonEntry(GcConstants.Core.REVIVE_TIME,
                json,
                JTokenType.String,
                "Core config is missing revive time.").ToString();
            manager.ReviveTime = TreeConverter.Build(reviveString, _engine);

            //get exp formula
            string expFormulaString = JsonUtils.ValidateJsonEntry(GcConstants.Core.EXP_FORMULA,
                json,
                JTokenType.String,
                "Core config is missing experience calculation formula.").ToString();
            manager.ExpFormula = TreeConverter.Build(expFormulaString, _engine);

            

            return manager;
        }
    }
}