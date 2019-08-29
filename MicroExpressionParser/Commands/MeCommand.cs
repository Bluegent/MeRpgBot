namespace RPGEngine.Commands
{
    using RPGEngine.Core;
    using RPGEngine.Discord;
    using RPGEngine.Game;
    using RPGEngine.Managers;

    public class MeCommand : ICommand
    {
        public string GetKey()
        {
            return CommandsConstants.ME_COMMAND;
        }

        public void Execute(Command command, IGameEngine engine)
        {
            if (command.Args.Length != 0)
            {
                engine.Log().Log("Usage of me command: me");
                return;
            }
            if (!CommandManager.Instance.CheckCurrentPlayer(command))
            {
                return;
            }

            Player currentPlayer = engine.GetPlayerManager().FindPlayerById(command.UserId);
            currentPlayer.DisplayOverall();
        }
    }
}