using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using System.Threading;

namespace MicroExpressionParser
{
    using System.Threading.Tasks;

    using RPGEngine.Core;
    using RPGEngine.Discord;
    using RPGEngine.GameInterface;

    class Program
    {

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        async Task MainAsync()
        {
            const int SLEEP_TIME = 1000;
            DiscordClient discord = new DiscordClient();
            ILogHelper logger = new DiscordLogHelper(discord);
            GameEngine engine = new GameEngine(logger);
            discord.SetEngine(engine);
            discord.Init();
            while (true)
            {
             engine.Update();
             Thread.Sleep(SLEEP_TIME);
            }
        }
    }
}
