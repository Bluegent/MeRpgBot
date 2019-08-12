using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGEngine.Logging
{
    public interface ILogger
    {
        void Log(string msg);
    }

    public class DiscordLogger : ILogger
    {
        public void Log(string msg)
        {
            //make bot talk...
        }
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string msg)
        {
            Console.WriteLine(msg);
        }
    }
}
