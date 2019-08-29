namespace RPGEngine.Commands
{
    using RPGEngine.Core;
    using RPGEngine.Discord;
    using RPGEngine.Game;
    using RPGEngine.Managers;

    public class TargetCommand : ICommand
    {
        public string GetKey()
        {
            return CommandsConstants.TARGET_COMMAND;
        }

        public void Execute(Command command, IGameEngine engine)
        {
            if (command.Args.Length < 1)
            {
                engine.Log().Log("Usage of target command: create <name>");
                return;
            }

            if (!CommandManager.Instance.CheckCurrentPlayer(command))
            {
                return;
            }

            string name = command.Args[0];
            Player currentPlayer = engine.GetPlayerManager().FindPlayerById(command.UserId);
            Player target = engine.GetPlayerManager().FindPlayerByName(name);
            if (target == null)
            {
                engine.Log().Log($"[{currentPlayer.Entity.Name}] Invalid target.");
                return;
            }


            currentPlayer.Entity.Target(target.Entity);
        }
    }
}