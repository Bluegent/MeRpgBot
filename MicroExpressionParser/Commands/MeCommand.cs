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
            if (!CommandManager.Instance.CheckCurrentPlayer(command))
            {
                return;
            }
            Player currentPlayer = engine.GetPlayerManager().FindPlayerById(command.UserId);
            if (command.Args.Length == 0)
            {
                
                currentPlayer.DisplayOverall();
            }
            else
            {
                if (command.Args.Length == 1)
                {
                    string option = command.Args[0];
                    switch (option)
                    {
                        case "skills":
                            {
                                currentPlayer.DisplaySkills();
                            }
                            break;
                        case "resources":
                            {
                                currentPlayer.DisplayResources();
                            }
                            break;
                        case "attributes":
                            {
                                currentPlayer.DisplayAttributes();
                            }
                            break;
                        case "stats":
                            {
                                currentPlayer.DisplayStats();
                            }
                            break;
                        case "all":
                            {
                                currentPlayer.DisplayAll();
                            }
                            break;
                    }
                }
            }

            

        }
    }
}