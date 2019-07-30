using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroExpressionParser
{
    using System.Dynamic;

    public interface IGameEngine
    {
        void AddPlayer(Entity entity);
        Entity[] GetAllPlayers();

        Entity GetEntityByKey(string key);
    }
    public class GameEngine : IGameEngine
    {
        public List<Entity> Players { get; }
        public GameEngine()
        {
            Players = new List<Entity>();
        }

        public void AddPlayer(Entity entity)
        {
            Players.Add(entity);
        }

        public Entity[] GetAllPlayers()
        {
            return Players.ToArray();
        }

        public Entity GetEntityByKey(string key)
        {
            foreach (Entity ent in Players)
            {
                if (ent.Key.Equals(key))
                    return ent;
            }

            return null;
        }
    }
}
