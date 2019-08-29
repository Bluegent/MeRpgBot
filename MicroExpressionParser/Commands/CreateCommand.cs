namespace RPGEngine.Commands
{
    using RPGEngine.Core;
    using RPGEngine.Discord;
    using RPGEngine.Game;
    using RPGEngine.Templates;

    public class CreateCommand : ICommand
    {
        public string GetKey()
        {
            return CommandsConstants.CREATE_COMMAND;
        }

        public void Execute(Command command, IGameEngine engine)
        {
            if (command.Args.Length < 2)
            {
                engine.Log().Log("Usage of create command: create <name> <class_name>");
                return;
            }

            string name = command.Args[0];
            string classKey = command.Args[1];
            if (!engine.GetClassManager().HasClass(classKey))
            {
                engine.Log().Log($"The class {classKey} does not exist.");
                return;
            }

            ClassTemplate playerClass = engine.GetClassManager().GetClass(classKey);
            engine.GetPlayerManager().CreatePlayer(command.UserId, name, playerClass);
        }
    }
}