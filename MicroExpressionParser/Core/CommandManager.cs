using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGEngine.Core
{
    using RPGEngine.Discord;
    using RPGEngine.Game;

    public class CommandManager
    {
        private static CommandManager _instance;

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
        }

        public void Execute(Command command)
        {
            switch (command.Name)
            {
                case CommandsConstants.CREATE_COMMAND:
                    {
                        if (command.Args.Length < 2)
                        {
                            Engine.Log().Log("Usage of create command: create <name> <class_name>");
                            return;
                        }

                        string name = command.Args[0];
                        string classKey = command.Args[1];
                        if (!Engine.GetClassManager().ClassExists(classKey))
                        {
                            Engine.Log().Log($"The class {classKey} does not exist.");
                            return;
                        }
                        ClassTemplate playerClass = new ClassTemplate();
                        Engine.GetPlayerManager().CreatePlayer(command.UserId, name, playerClass);
                    }
                    break;
                case CommandsConstants.TARGET_COMMAND:
                    {
                        if (command.Args.Length < 1)
                        {
                            Engine.Log().Log("Usage of target command: create <name>");
                            return;
                        }

                        if (!Engine.GetPlayerManager().PlayerExists(command.UserId))
                        {
                            Engine.Log().Log("You do not have a character. Create one.");
                            return;
                        }

                        string name = command.Args[0];
                        Player target = Engine.GetPlayerManager().FindPlayerByName(name);
                        if (target == null)
                        {
                            Engine.Log().Log("Target does not exist.");
                            return;
                        }

                        Player currentPlayer = Engine.GetPlayerManager().FindPlayerById(command.UserId);
                        currentPlayer.Entity.Target(target.Entity);

                    }
                    break;
                case CommandsConstants.ATTACK_COMMAND:
                    {
                        if (!Engine.GetPlayerManager().PlayerExists(command.UserId))
                        {
                            Engine.Log().Log("You do not have a character. Create one.");
                            return;
                        }
                        Player currentPlayer = Engine.GetPlayerManager().FindPlayerById(command.UserId);

                        if (!currentPlayer.Entity.HasTarget)
                        {
                            Engine.Log().Log("You do not have a target or your current target is dead. Chose a valid target.");
                            return;
                        }

                        currentPlayer.Entity.Cast(
                            currentPlayer.Entity.CurrentTarget,
                            currentPlayer.Class.BaseAttack.Key);
                    }
                    break;
            }
        }
    }
}
