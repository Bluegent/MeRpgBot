
using System.Text;
using RPGEngine.Core;
using RPGEngine.Entities;

namespace RPGEngine.GameInterface
{
    using System;
    using System.Collections.Generic;

    public interface ILogHelper
    {
        string Enclose(string msg, string enclosure);


        void StartBlock();
        void EndBlock();
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

        public const long MAX_MESSAGE_LENGTH = 1800;

        public const string NEW_LINE = "\n";

        private StringBuilder output;

        public DiscordLogHelper(ILogger log)
        {
            _log = log;
            output = new StringBuilder();
        }

        public void EndBlock()
        {
            if (output.Length == 0)
                return;
            _log.Log(Enclose(output.ToString(),"```"));
            output.Clear();
        }

        public void Log(string message)
        {
            if (output.Length + message.Length> MAX_MESSAGE_LENGTH)
            {
                EndBlock();
                output.Append(message);
                output.Append(NEW_LINE);
            }
            else
            {
                output.Append(message);
                output.Append(NEW_LINE);
            }
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

        public void StartBlock()
        {
            output.Clear();
        }

        public void LogEntity(Entity target)
        {
            ResourceInstance hp = target.GetResource(Entity.HP_KEY);
            string hpBar = Utils.Utility.getBar(hp.Value, hp.MaxAmount);
            string msg = $"{target.Name} {hpBar}";
            Log(msg);
        }
        public void LogDamage(Entity target, Entity source, DamageTypeTemplate typeTemplate, double amount, double resisted)
        {
            ResourceInstance hp = target.GetResource(Entity.HP_KEY);
            string hpBar = Utils.Utility.getBar(hp.Value,hp.MaxAmount);
            string resist = Utils.Utility.DoubleEq(resisted,0.0)?"": $"({resisted} resisted)";
            string msg = $"{target.Name} {hpBar} [-{amount}{resist} {typeTemplate.Name} damage]";
            Log(msg);
        }

        public void LogDodge(Entity target, Entity source)
        {
            Log($"{target.Name} dodged {source.Name}'s attack.");
        }

        public virtual void LogSay(Entity source, string message)
        {
            Log($"{source.Name}:{Enclose(message,Italics)}");
        }
    }

}
