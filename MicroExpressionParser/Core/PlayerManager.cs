namespace RPGEngine.Core
{
    using System.Collections.Generic;

    using RPGEngine.Game;

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
    }
}