namespace RPGEngine.Commands
{
    using RPGEngine.Core;
    using RPGEngine.Discord;
    using RPGEngine.Game;

    public class ListCommand : ICommand
    {
        public string GetKey()
        {
            return CommandsConstants.LIST_COMMAND;
        }

        public void Execute(Command command, IGameEngine engine)
        {
            if (command.Args.Length == 0)
            {
                engine.Log().Log("Usage of me command: list <option> (players, classes)");
                return;
            }

            string option = command.Args[0];

            switch (option)
            {
                case "players":
                    engine.GetPlayerManager().DisplayPlayerList();
                    break;
                case "classes":
                    engine.GetClassManager().DisplayClassList();
                    break;
            }

        }
    }
}