using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;

namespace MicroExpressionParser
{
    using System.Threading.Tasks;

    using RPGEngine.Discord;
    using RPGEngine.Input;

    class Program
    {

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        async Task MainAsync()
        {
            InputHandler handler=new InputHandler();
            handler.Start();
            DiscordClient discord = new DiscordClient(handler);
            discord.Init();
            await Task.Delay(-1);
        }
    }
}
