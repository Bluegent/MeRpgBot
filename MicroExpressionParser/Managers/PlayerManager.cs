﻿namespace RPGEngine.Managers
{
    using System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using RPGEngine.Core;
    using RPGEngine.Game;
    using RPGEngine.Templates;
    using RPGEngine.Utils;

    public class PlayerManager
    {
        private Dictionary<long, Player> _players;
        public IGameEngine Engine { get; set; }

        public PlayerManager()
        {
            LoadPlayers();
        }

        private void LoadPlayers()
        {
            _players = new Dictionary<long, Player>();
        }

        public void CreatePlayer(long id,string name, ClassTemplate playerClass)
        {
            if (PlayerExists(id))
            {
                Engine.Log().Log("A player with your id already exists.");
                return;
            }

            if (PlayerNameExists(name))
            {
                Engine.Log().Log($"Name {name} already in use, please chose another one.");
                return;
            }
            Player player = new Player(Engine, id, playerClass);
            player.Entity.Name = name;
            _players.Add(id, player);
            Engine.Log().Log($"Player {name} created successfully.");
        }

        public bool PlayerExists(long id)
        {
            return _players.ContainsKey(id);
        }

        public Player FindPlayerByName(string name)
        {
            foreach (var player in _players)
            {
                if (player.Value.Entity.Name.Equals(name))
                    return player.Value;
            }

            return null;
        }
        public Player FindPlayerById(long id)
        {
            if(_players.ContainsKey(id))
                return _players[id];
            return null;
        }
        public bool PlayerNameExists(string name)
        {
            foreach (var player in _players)
            {
                if (player.Value.Entity.Name.Equals(name))
                    return true;
            }

            return false;
        }

        public void Update()
        {
            foreach (Player player in _players.Values)
            {
                player.Entity.Update();
            }
        }

        public void DisplayPlayerList()
        {
            string displayString = "Registered players:\n";
            if (_players.Count == 0)
                displayString += "No registered players at the moment.";
            foreach (Player player in _players.Values)
            {
                displayString += $"{player.Entity.Name} \n";
            }
            Engine.Log().Log(displayString);
        }

        public void Load(string path)
        {
            JObject[] players = FileHandler.FromPath<JObject[]>(path);
            if (players != null)
            {
                foreach (JObject playerObject in players)
                {
                    Player player =new Player();
                    player.FromJObject(playerObject, Engine);
                    _players.Add(player.Id,player);
                }
            }
        }

        public void Save(string path)
        {
            List < JObject > result = new List<JObject>();
            foreach(Player player in _players.Values)
            {
                result.Add(player.ToJObject());
            }
            FileHandler.Write(path,JsonConvert.SerializeObject(result,Formatting.Indented));
        }
    }
}