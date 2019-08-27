namespace ConsoleTester
{
    using RPGEngine.Core;
    using RPGEngine.GameInterface;

    class Program
    {
        static void Main(string[] args)
        {
            GameEngine engine = new GameEngine(new DiscordLogHelper(new ConsoleLogger()));
            engine.LoadConfigFromFiles();
        }
    }
}
