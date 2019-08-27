namespace RPGEngine.GameConfigReader
{
    using Newtonsoft.Json.Linq;

    using RPGEngine.Core;
    using RPGEngine.Utils;

    public class DamageTypeReader
    {
        private IGameEngine _engine;
        public DamageTypeReader(IGameEngine engine)
        {
            _engine = engine;
        }

        public DamageTypeTemplate FromJson(JObject json)
        {
            string mitigation = JsonUtils.GetValueOrDefault<string>(json,GcConstants.DamageType.MITIGATION,null);
            string dodge = JsonUtils.GetValueOrDefault<string>(json, GcConstants.DamageType.DODGE, null); ;
            string crit = JsonUtils.GetValueOrDefault<string>(json, GcConstants.DamageType.CRIT, null); ;
            string critmod = JsonUtils.GetValueOrDefault<string>(json, GcConstants.DamageType.CRIT_MULT, null); ;

            DamageTypeTemplate template = new DamageTypeTemplate(_engine,mitigation,dodge,crit,critmod);
            template.LoadBase(json);
            return template;
        }
    }
}