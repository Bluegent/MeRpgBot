namespace RPGEngine.Commands
{
    using RPGEngine.Core;
    using RPGEngine.Discord;
    using RPGEngine.Game;
    using RPGEngine.Managers;

    public class AutoAttackCommand : ICommand
    {
        public string GetKey()
        {
            return CommandsConstants.AUTO_ATTACK_COMMAND;
        }

        public void Execute(Command command, IGameEngine engine)
        {
            Player player = engine.GetPlayerManager().FindPlayerById(command.UserId);
            if (!CommandManager.Instance.CheckCurrentPlayer(command))
            {
                return;
            }
            if (player.Entity.IsDead)
            {
                return;
            }

            if (player.Entity.IsAutoCasting)
            {
                engine.Log().Log($"[{player.Entity.Name}] You are already auto-attacking.");
                return;
            }

            Player target;
            if (command.Args.Length == 0)
            {
                if (!player.Entity.HasTarget)
                {
                    engine.Log().Log($"[{player.Entity.Name}] Invalid target.");
                    return;
                }

                target = null;

            }
            else
            {
                string name = command.Args[0];
                target = engine.GetPlayerManager().FindPlayerByName(name);
                if (target == null)
                {
                    engine.Log().Log($"[{player.Entity.Name}] Invalid target.");
                    return;
                }
                else
                {
                    player.Entity.Target(target.Entity);
                }
            }

            player.Entity.StartAutoCasting(player.Class.BaseAttack.Key, target?.Entity);
        }
    }
}