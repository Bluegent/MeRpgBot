using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGEngine.Core;

namespace RPGEngine.Logging
{
    public interface ILogHelper
    {
        void Log(string msg);
        void LogBlock(string msg);
        void LogDamage(Entity target, Entity source, DamageType type, double amount, double resisted);
        void LogDodge(Entity target, Entity source);
    }

    public class DiscordLogHelper : ILogHelper
    {
        private ILogger _log;
        public const string CodeBlock = "```";

        public DiscordLogHelper(ILogger log)
        {
            _log = log;
        }
        public void Log(string message)
        {
            _log.Log(message);
        }

        public void LogBlock(string msg)
        {
            _log.Log(CodeBlock+msg+CodeBlock);
        }

        public void LogDamage(Entity target, Entity source, DamageType type, double amount, double resisted)
        {
            string hpBar = Utils.Utility.getBar(target.GetProperty("CHP").Value,target.GetProperty("MHP").Value);
            string resist = Utils.Utility.DoubleEq(resisted,0.0)?"": $"and resisted {resisted}";
            string msg = $"{target.Name} {hpBar} took {amount} {resist} {type.Name} damage.";
            LogBlock(msg);
        }

        public void LogDodge(Entity target, Entity source)
        {
            LogBlock($"{target.Name} dodged {source.Name}'s attack.");
        }
    }

}
