using System.Threading;

namespace RPGEngine
{
    using System.Threading.Tasks;

    using RPGEngine.Core;
    using RPGEngine.Discord;
    using RPGEngine.GameInterface;
    using RPGEngine.Managers;

    class Program
    {

        public static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        async Task MainAsync()
        {
            const int SLEEP_TIME = 1000;
            DiscordClient discord = new DiscordClient();
            ILogHelper logger = new DiscordLogHelper(discord);
            GameEngine engine = new GameEngine(logger);
            engine.Timer = new RealTimer();
            engine.LoadConfigFromFiles();
            engine.LoadPersistenceFiles();
            CommandManager.Instance.Engine = engine;
            discord.Engine = engine;
            discord.Init();
            while (true)
            {
             engine.Update();
             Thread.Sleep(SLEEP_TIME);
            }
        }
    }
}
