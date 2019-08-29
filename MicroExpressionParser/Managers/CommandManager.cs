using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGEngine.Entities;

namespace RPGEngine.Managers
{
    using System.Reflection;

    using Newtonsoft.Json;

    using RPGEngine.Commands;
    using RPGEngine.Core;
    using RPGEngine.Discord;
    using RPGEngine.Game;
    using RPGEngine.Templates;

    public class CommandManager
    {
        private static CommandManager _instance;

        private Dictionary<string, Action<Command>> _commands;
        private Dictionary<string, ICommand> _commandInstances;

        public IGameEngine Engine { get; set; }
        public static CommandManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CommandManager();
                }

                return _instance;
            }
        }

        private CommandManager()
        {
            _commands = new Dictionary<string, Action<Command>>();
            _commandInstances = new Dictionary<string, ICommand>();
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            string commandsNamespace = typeof(ICommand).Namespace;
            Type[] commands = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type command in commands)
            {
                if (command.Namespace != null
                    && command.Namespace.Equals(commandsNamespace)
                    && !command.IsInterface)
                {
                    ICommand commandInstance = (ICommand)Activator.CreateInstance(command);

                    if (!_commandInstances.ContainsKey(commandInstance.GetKey()))
                    {
                        RegisterCommand(commandInstance);
                    }
                }
            }
        }

        public bool CheckCurrentPlayer(Command command)
        {
            if (!Engine.GetPlayerManager().PlayerExists(command.UserId))
            {

                Engine.Log().Log("You don't have a character.");
                return false;
            }
            return true;
        }
        private void RegisterCommand(ICommand command)
        {
            if (!_commandInstances.ContainsKey(command.GetKey()))
            {
                _commandInstances.Add(command.GetKey(), command);
            }
        }



        public void Execute(Command command)
        {
            if (_commandInstances.ContainsKey(command.Name))
                _commandInstances[command.Name].Execute(command, Engine);
            if (_commands.ContainsKey(command.Name))
                _commands[command.Name](command);
        }
    }
}
