namespace RPGEngine.Commands
{
    using RPGEngine.Core;
    using RPGEngine.Discord;
    using RPGEngine.Game;
    using RPGEngine.Managers;

    public class DuelCommand : ICommand
    {
        public string GetKey()
        {
            return CommandsConstants.DUEL_COMMAND;
        }

        public void Execute(Command command, IGameEngine engine)
        {
            if (command.Args.Length < 1)
            {
                engine.Log().Log("Usage of cast command: duel challenge <target>\nduel accept \nduel reject \nduel exit");
                return;
            }
            if (!CommandManager.Instance.CheckCurrentPlayer(command))
            {
                return;
            }
            Player currentPlayer = engine.GetPlayerManager().FindPlayerById(command.UserId);

            string option = command.Args[0];
            switch (option)
            {
                case "challenge":
                    {
                        if (command.Args.Length < 2)
                        {
                            engine.Log().Log("Please chose a target to challenge (duel challenge <target>)");
                            return;
                        }

                        string targetName = command.Args[1];

                        Player target = engine.GetPlayerManager().FindPlayerByName(targetName);
                        if (target == null)
                        {
                            engine.Log().Log($"A player with name {targetName} does not exist.");
                            return;
                        }

                        if (target.Entity.IsDead)
                        {
                            engine.Log().Log($"{targetName} is dead. You can't challenge a dead player.");
                            return;
                        }

                        if (currentPlayer.Id == target.Id)
                        {
                            engine.Log().Log($"{targetName} you can't challenge yourself, you idiot.");
                            return;
                        }
                        target.AddChallenge(currentPlayer);

                    }
                    break;
                case "accept":
                    {
                        if (currentPlayer.Dueling)
                        {
                            engine.Log().Log($"[{currentPlayer.Entity.Name}] Currently dueling.");
                            return;
                        }
                        if (currentPlayer.DuelRequests.Count != 0)
                        {
                            currentPlayer.AcceptDuel();
                        }
                        else
                        {
                            engine.Log().Log($"[{currentPlayer.Entity.Name}] No duel requests.");
                        }
                    }
                    break;
                case "reject":
                    {
                        if (currentPlayer.Dueling)
                        {
                            engine.Log().Log($"[{currentPlayer.Entity.Name}] Currently dueling.");
                            return;
                        }
                        if (currentPlayer.DuelRequests.Count != 0)
                        {
                            currentPlayer.AcceptDuel();
                        }
                        else
                        {
                            engine.Log().Log($"[{currentPlayer.Entity.Name}] No duel requests.");
                        }
                    }
                    break;
                case "exit":
                    {
                        if (currentPlayer.Dueling)
                        {
                            currentPlayer.EndDuel();
                        }
                        else
                        {
                            engine.Log().Log($"[{currentPlayer.Entity.Name}] Not currently dueling.");
                        }
                    }
                    break;

            }
        }
    }
}