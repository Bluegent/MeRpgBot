
using System.Text;
using RPGEngine.Core;
using RPGEngine.Entities;

namespace RPGEngine.GameInterface
{
    public interface ILogHelper
    {
        string Enclose(string msg, string enclosure);

        void Log(string msg);
        void LogEntity(Entity me);
        void LogDamage(Entity target, Entity source, DamageTypeTemplate typeTemplate, double amount, double resisted);
        void LogDodge(Entity target, Entity source);
        void LogSay(Entity source, string msg);
    }

    public class DiscordLogHelper : ILogHelper
    {
        private ILogger _log;
        public const string CodeBlock = "```";
        public const string Italics = "_";

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
            _log.Log(Enclose(msg,CodeBlock));
        }

        public string Enclose(string msg, string enclosure)
        {
            StringBuilder sb = new StringBuilder(msg);
            sb.Insert(0, enclosure);
            sb.Append(enclosure);
            return  sb.ToString();
        }

        public void LogEntity(Entity target)
        {
            ResourceInstance hp = target.GetResource(Entity.HP_KEY);
            string hpBar = Utils.Utility.getBar(hp.Value, hp.MaxAmount);
            string msg = $"{target.Name} {hpBar}";
            LogBlock(msg);
        }
        public void LogDamage(Entity target, Entity source, DamageTypeTemplate typeTemplate, double amount, double resisted)
        {
            ResourceInstance hp = target.GetResource(Entity.HP_KEY);
            string hpBar = Utils.Utility.getBar(hp.Value,hp.MaxAmount);
            string resist = Utils.Utility.DoubleEq(resisted,0.0)?"": $"and resisted {resisted}";
            string msg = $"{target.Name} {hpBar} took {amount} {resist} {typeTemplate.Name} damage.";
            LogBlock(msg);
        }

        public void LogDodge(Entity target, Entity source)
        {
            LogBlock($"{target.Name} dodged {source.Name}'s attack.");
        }

        public virtual void LogSay(Entity source, string message)
        {
            LogBlock($"{source.Name}:{Enclose(message,Italics)}");
        }
    }

}
