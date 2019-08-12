using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGEngine.Logging
{
    public interface ILogHelper
    {
        void Log(string msg);
        void LogBlock(string msg);
    }

    public class DiscordLogHelper : ILogHelper
    {
        private ILogger _log;
        public const string CODE_BLOCK = "```";
        
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
            _log.Log(CODE_BLOCK+msg+CODE_BLOCK);
        }
    }

}
