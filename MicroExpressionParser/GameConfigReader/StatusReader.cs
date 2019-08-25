using Newtonsoft.Json.Linq;
using RPGEngine.Core;
using RPGEngine.Game;
using RPGEngine.Parser;

namespace RPGEngine.GameConfigReader
{
    public class StatusReader
    {
        public IGameEngine Engine { get; }

        public StatusReader(IGameEngine engine)
        {
            Engine = engine;
        }

        public StatusTemplate FromJSON(JObject json)
        {
            MeNode[] formulas = Engine.GetSanitizer().SplitAndConvert(json[GcConstants.General.FORMULA].ToString());
            StatusTemplate result = new StatusTemplate(formulas);
            result.LoadBase(json);

            result.MaxStacks = json.ContainsKey(GcConstants.Statuses.MAX_STACK)
                ? TreeConverter.Build(json[GcConstants.Statuses.MAX_STACK].ToString(), Engine)
                : new MeNode(1);
            ;

            result.Interval = json.ContainsKey(GcConstants.Statuses.INTERVAL)
                ? TreeConverter.Build(json[GcConstants.Statuses.MAX_STACK].ToString(), Engine)
                : new MeNode(1);
            ;

            result.Interval = json.ContainsKey(GcConstants.Statuses.INTERVAL)
                ? TreeConverter.Build(json[GcConstants.Statuses.MAX_STACK].ToString(), Engine)
                : new MeNode(1);
            ;

            result.Type = json.ContainsKey(GcConstants.Statuses.STACK_TYPE)
                ? StatusTemplate.FromString(json[GcConstants.Statuses.STACK_TYPE].ToString())
                : StackingType.Independent;

           

            return result;
        }
    }
}
/*
       {   
        "key":"shonen_powerup",
        "name":"Shonen Powerup",
        "description":"You feel stronger, but somehow dumber.",
        "max_stack":"1",
        "interval":"0",
        "stacking":"refresh",
        "formula":"MOD_VALUE(STR,$0);MOD_VALUE(DEX,$1);MOD_VALUE(INT,$2)"
    }
*/
