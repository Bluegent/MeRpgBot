using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPGEngine.Entities;

namespace RPGEngine.Managers
{
    using Newtonsoft.Json;

    using RPGEngine.Core;
    using RPGEngine.Discord;
    using RPGEngine.Game;
    using RPGEngine.Templates;

    public class CommandManager
    {
        private static CommandManager _instance;

        private Dictionary<string, Action<Command>> _commands;

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
            RegisterCommands();
        }

        private void RegisterCommands()
        {
            RegisterCommand(CommandsConstants.CREATE_COMMAND,CreateCommand);
            RegisterCommand(CommandsConstants.TARGET_COMMAND,TargetCommand);
            RegisterCommand(CommandsConstants.ATTACK_COMMAND,AttackCommand);
            RegisterCommand(CommandsConstants.CAST_COMMAND, CastCommand);
            RegisterCommand(CommandsConstants.DUEL_COMMAND, DuelCommand);
            RegisterCommand(CommandsConstants.ME_COMMAND,MeCommand);
            RegisterCommand(CommandsConstants.LIST_COMMAND,ListCommand);
            RegisterCommand(CommandsConstants.AUTO_ATTACK_COMMAND, AutoAttackCommand);
        }

        private void AutoAttackCommand(Command command)
        {
            Player player = Engine.GetPlayerManager().FindPlayerById(command.UserId);
            if (!CheckCurrentPlayer(command))
            {
                return;
            }
            if (player.Entity.IsDead)
            {
                Engine.Log().Log("You can't auto-attack if you're dead.");
                return;
            }

            if (player.Entity.IsAutoCasting)
            {
                Engine.Log().Log("You are already auto-attacking.");
                return;
            }

            Player target;
            if (command.Args.Length == 0)
            {
                if (!player.Entity.HasTarget)
                {
                    Engine.Log().Log("You do not have a target or your current target is dead. Chose a valid target.");
                    return;
                }

                target = null;

            }
            else
            {
                string name = command.Args[0];
                target = Engine.GetPlayerManager().FindPlayerByName(name);
                if (target == null)
                {
                    Engine.Log().Log("Target does not exist.");
                    return;
                }
                else
                {
                    player.Entity.Target(target.Entity);
                }
            }

            player.Entity.StartAutoCasting(player.Class.BaseAttack.Key, target?.Entity);
        }

        private bool CheckCurrentPlayer(Command command)
        {
            if (!Engine.GetPlayerManager().PlayerExists(command.UserId))
            {

                Engine.Log().Log("You do not have a character. Create one.");
                return false;
            }
            return true;
        }

        private void RegisterCommand(string commandName, Action<Command> command)
        {
            if (!_commands.ContainsKey(commandName))
            {
                _commands.Add(commandName,command);
            }
        }

        private void ListCommand(Command command)
        {
            if (command.Args.Length == 0)
            {
                Engine.Log().Log("Usage of me command: list <option> (players, classes)");
                return;
            }

            string option = command.Args[0];

            switch (option)
            {
                case "players":
                    Engine.GetPlayerManager().DisplayPlayerList();
                    break;
                case "classes":
                    Engine.GetClassManager().DisplayClassList();
                    break;
            }

        }

        private void MeCommand(Command command)
        {
            if (command.Args.Length != 0)
            {
                Engine.Log().Log("Usage of me command: me");
                return;
            }
            if (!CheckCurrentPlayer(command))
            {
                return;
            }

            Player currentPlayer = Engine.GetPlayerManager().FindPlayerById(command.UserId);
            currentPlayer.DisplayOverall();

        }

        private void CreateCommand(Command command)
        {
            if (command.Args.Length < 2)
            {
                Engine.Log().Log("Usage of create command: create <name> <class_name>");
                return;
            }

            string name = command.Args[0];
            string classKey = command.Args[1];
            if (!Engine.GetClassManager().HasClass(classKey))
            {
                Engine.Log().Log($"The class {classKey} does not exist.");
                return;
            }

            ClassTemplate playerClass = Engine.GetClassManager().GetClass(classKey);
            Engine.GetPlayerManager().CreatePlayer(command.UserId, name, playerClass);
        }

        private void TargetCommand(Command command)
        {
            if (command.Args.Length < 1)
            {
                Engine.Log().Log("Usage of target command: create <name>");
                return;
            }

            if (!CheckCurrentPlayer(command))
            {
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

        private void AttackCommand(Command command)
        {
            if (!CheckCurrentPlayer(command))
            {
                return;
            }
            Player currentPlayer = Engine.GetPlayerManager().FindPlayerById(command.UserId);

            if (!currentPlayer.Entity.HasTarget)
            {
                Engine.Log().Log("You do not have a target or your current target is dead. Chose a valid target.");
                return;
            }

            if (currentPlayer.Entity.IsAutoCasting
                && currentPlayer.Entity.AutoCastSkill == currentPlayer.Class.BaseAttack.Key)
            {
                Engine.Log().Log("You are already auto-attacking that target.");
                return;
            }

            currentPlayer.Entity.Cast(
                currentPlayer.Entity.CurrentTarget,
                currentPlayer.Class.BaseAttack.Key);
        }

        private void CastCommand(Command command)
        {
            if (command.Args.Length < 1)
            {
                Engine.Log().Log("Usage of cast command: cast <skill_alias> [optional] target");
                return;
            }

            if (!CheckCurrentPlayer(command))
            {
                return;
            }



            string skillAlias = command.Args[0];

            Player currentPlayer = Engine.GetPlayerManager().FindPlayerById(command.UserId);

            if (!currentPlayer.Entity.HasTarget)
            {
                Engine.Log().Log("You do not have a target or your current target is dead. Chose a valid target.");
                return;
            }

            currentPlayer.Entity.Cast(currentPlayer.Entity.CurrentTarget, skillAlias);
        }

        private void DuelCommand(Command command)
        {
            if (command.Args.Length < 1)
            {
                Engine.Log().Log("Usage of cast command: duel challenge <target>\nduel accept \nduel reject \nduel exit");
                return;
            }
            if (!CheckCurrentPlayer(command))
            {
                return;
            }
            Player currentPlayer = Engine.GetPlayerManager().FindPlayerById(command.UserId);

            string option = command.Args[0];
            switch (option)
            {
                case "challenge":
                    {
                        if (command.Args.Length < 2)
                        {
                            Engine.Log().Log("Please chose a target to challenge (duel challenge <target>)");
                            return;
                        }

                        string targetName = command.Args[1];

                        Player target = Engine.GetPlayerManager().FindPlayerByName(targetName);
                        if (target == null)
                        {
                            Engine.Log().Log($"A player with name {targetName} does not exist.");
                            return;
                        }

                        if (target.Entity.IsDead)
                        {
                            Engine.Log().Log($"{targetName} is dead. You can't challenge a dead player.");
                            return;
                        }

                        if (currentPlayer.Id == target.Id)
                        {
                            Engine.Log().Log($"{targetName} you can't challenge yourself you idiot.");
                            return;
                        }
                        target.AddChallenge(currentPlayer);

                    }
                    break;
                case "accept":
                    {
                        if (currentPlayer.Dueling)
                        {
                            Engine.Log().Log($"{currentPlayer.Entity.Name} you are currently in a duel.");
                            return;
                        }
                        if (currentPlayer.DuelRequests.Count != 0)
                        {
                            currentPlayer.AcceptDuel();
                        }
                        else
                        {
                            Engine.Log().Log($"{currentPlayer.Entity.Name} you do not have duel requests.");
                        }
                    }
                    break;
                case "reject":
                    {
                        if (currentPlayer.Dueling)
                        {
                            Engine.Log().Log($"{currentPlayer.Entity.Name} you are currently in a duel.");
                            return;
                        }
                        if (currentPlayer.DuelRequests.Count != 0)
                        {
                            currentPlayer.AcceptDuel();
                        }
                        else
                        {
                            Engine.Log().Log($"{currentPlayer.Entity.Name} you do not have duel requests.");
                        }
                    }
                    break;
                case "exit":
                    {
                        if (currentPlayer.Dueling)
                        {
                            currentPlayer.EndDuel();
                            return;
                        }
                        else
                        {
                            Engine.Log().Log($"{currentPlayer.Entity.Name} you are not in a duel at the moment.");
                        }
                    }
                    break;

            }


        }

        public void Execute(Command command)
        {
            if (_commands.ContainsKey(command.Name))
                _commands[command.Name](command);
        }
    }
}
