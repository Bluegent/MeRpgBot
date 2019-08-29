namespace RPGEngine.Commands
{
    using RPGEngine.Core;
    using RPGEngine.Discord;
    using RPGEngine.Game;
    using RPGEngine.Managers;

    public class AttackCommand : ICommand
    {
        public string GetKey()
        {
            return CommandsConstants.ATTACK_COMMAND;
        }

        public void Execute(Command command, IGameEngine engine)
        {
            if (!CommandManager.Instance.CheckCurrentPlayer(command))
            {
                return;
            }
            Player currentPlayer = engine.GetPlayerManager().FindPlayerById(command.UserId);

            if (!currentPlayer.Entity.HasTarget)
            {
                engine.Log().Log($"[{currentPlayer.Entity.Name}] Invalid target.");
                return;
            }

            if (currentPlayer.Entity.IsAutoCasting
                && currentPlayer.Entity.AutoCastSkill == currentPlayer.Class.BaseAttack.Key)
            {
                engine.Log().Log($"[{currentPlayer.Entity.Name}] You are already auto-attacking that target.");
                return;
            }

            currentPlayer.Entity.Cast(
                currentPlayer.Entity.CurrentTarget,
                currentPlayer.Class.BaseAttack.Key);
        }
    }
}