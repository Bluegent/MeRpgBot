using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            StatusTemplate result = new StatusTemplate();
            result.LoadBase(json);

            result.MaxStacks = json.ContainsKey(GcConstants.MAX_STACK)
                ? TreeConverter.Build(json[GcConstants.MAX_STACK].ToString(), Engine)
                : new MeNode(1);
            ;

            result.Interval = json.ContainsKey(GcConstants.INTERVAL)
                ? TreeConverter.Build(json[GcConstants.MAX_STACK].ToString(), Engine)
                : new MeNode(1);
            ;

            result.Interval = json.ContainsKey(GcConstants.INTERVAL)
                ? TreeConverter.Build(json[GcConstants.MAX_STACK].ToString(), Engine)
                : new MeNode(1);
            ;

            result.Type = json.ContainsKey(GcConstants.STACK_TYPE)
                ? StatusTemplate.FromString(json[GcConstants.STACK_TYPE].ToString())
                : StackingType.Independent;

            result.ComponentFormulas = Engine.GetSanitizer().SplitAndConvert(json[GcConstants.FORMULA].ToString());

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
