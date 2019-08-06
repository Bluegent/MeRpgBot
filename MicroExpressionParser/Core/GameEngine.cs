using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MicroExpressionParser
{
    using MicroExpressionParser.Core;
    using MicroExpressionParser.Sanitizer;
    using System.Dynamic;

    public interface IGameEngine
    {
        void AddPlayer(Entity entity);

        void AddEnemy(Entity entity);
        Entity[] GetAllPlayers();

        DamageType GeDamageType(string key);
        Entity GetEntityByKey(string key);
        StatusTemplate GetStatusByKey(string key);
        ITimer getTimer();

    }
    public class GameEngine : IGameEngine
    {
        public Dictionary<string, Entity> Players { get; }
        public Dictionary<string, Entity> Enemies { get; }
        private Dictionary<string, DamageType> DamageTypes { get; }
        private Sanitizer.Sanitizer Sanit { get; set; }

        public ITimer Timer { get; set; }
        private StatusTemplate test;
        public GameEngine()
        {
            ParserConstants.Init(this);
            Players = new Dictionary<string, Entity>();
            Enemies = new Dictionary<string, Entity>();
            DamageTypes = new Dictionary<string, DamageType>();
            DamageTypes.Add("P", new DamageType() { Key = "P", MitigationFormula = "NON_NEG($VALUE - GET_PROP($TARGET, DEF))" });
            DamageTypes.Add("M", new DamageType() { Key = "M", MitigationFormula = "NON_NEG($VALUE - GET_PROP($TARGET, MDEF))" });
            DamageTypes.Add("T", new DamageType() { Key = "T", MitigationFormula = "$VALUE" });
            string expression = "MOD_VALUE(STR,$0)";
            Sanit = new Sanitizer.Sanitizer(this);
            FunctionalNode[] statuses = Sanit.SplitStatus(expression);
            test = new StatusTemplate() { formula = statuses };
            Timer = new MockTimer();
     
        }

        public void AddPlayer(Entity entity)
        {
            Players.Add(entity.Key, entity);
        }

        public void AddEnemy(Entity entity)
        {
            Enemies.Add(entity.Key, entity);
        }

        public Entity[] GetAllPlayers()
        {
            return Players.Values.ToArray();
        }

        public DamageType GeDamageType(string key)
        {
            return DamageTypes.ContainsKey(key) ? DamageTypes[key] : null;
        }

        public Entity GetEntityByKey(string key)
        {
            if(Players.ContainsKey(key)) 
                return Players[key];
            if(Enemies.ContainsKey(key))
                return Enemies[key];
            return null;
        }

        StatusTemplate IGameEngine.GetStatusByKey(string key)
        {
            return test;
        }

        public ITimer getTimer()
        {
            return Timer;
        }
    }
}
