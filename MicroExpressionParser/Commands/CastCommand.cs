namespace RPGEngine.Commands
{
    using RPGEngine.Core;
    using RPGEngine.Discord;
    using RPGEngine.Game;
    using RPGEngine.Managers;

    public class CastCommand : ICommand
    {
        public string GetKey()
        {
            return CommandsConstants.CAST_COMMAND;
        }

        public void Execute(Command command, IGameEngine engine)
        {
            if (command.Args.Length < 1)
            {
                engine.Log().Log("Usage of cast command: cast <skill_alias> [optional] target");
                return;
            }

            if (!CommandManager.Instance.CheckCurrentPlayer(command))
            {
                return;
            }



            string skillAlias = command.Args[0];

            Player currentPlayer = engine.GetPlayerManager().FindPlayerById(command.UserId);



            if (command.Args.Length > 1)
            {
                string name = command.Args[1];
                Player target = engine.GetPlayerManager().FindPlayerByName(name);
                if (target == null)
                {
                    engine.Log().Log($"[{currentPlayer.Entity.Name}] Invalid target.");
                    return;
                }
                currentPlayer.Entity.Cast(target.Entity, skillAlias);
                return;
            }
            if (!currentPlayer.Entity.HasTarget)
            {
                engine.Log().Log($"[{currentPlayer.Entity.Name}] Invalid target.");
                return;
            }
            currentPlayer.Entity.Cast(currentPlayer.Entity.CurrentTarget, skillAlias);
        }
    }
}