using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;

namespace MicroExpressionParser
{
    using System.Threading.Tasks;

    using RPGEngine.Core;
    using RPGEngine.Discord;

    class Program
    {

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        async Task MainAsync()
        {
            GameEngine engine = new GameEngine(null);
            DiscordClient discord = new DiscordClient(engine);
            discord.Init();
            await Task.Delay(-1);
        }
    }
}
