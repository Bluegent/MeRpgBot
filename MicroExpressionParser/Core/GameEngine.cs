
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RPGEngine.Core;

using RPGEngine.Discord;
using RPGEngine.Language;
using RPGEngine.Entities;
using RPGEngine.Cleanup;
using RPGEngine.Game;
using RPGEngine.GameInterface;
using RPGEngine.Parser;

namespace RPGEngine.Core
{


    public interface IGameEngine
    {
        SkillManager GetSkillManager();
        PlayerManager GetPlayerManager();

        PropertyManager GetPropertyManager();
        ClassManager GetClassManager();
        void AddPlayer(Entity entity);

        void AddEnemy(Entity entity);

        void AddDamageType(DamageType type);
        Entity[] GetAllPlayers();

        DamageType GeDamageType(string key);
        Entity GetEntityByKey(string key);
        StatusTemplate GetStatusByKey(string key);
        void AddStatus(StatusTemplate status, string key);
        ITimer GetTimer();
        Sanitizer GetSanitizer();
        MeVariable GetVariable(string key);
        void AddVariable(string key, MeVariable var);
        void SetVariable(string key, MeVariable var);
        ILogHelper Log();
        long GetMaxExp(int level);

        long GetDefaultSkillThreat();

        void EnqueueCommand(Command command);

        void Update();

    }
    public class GameEngine : IGameEngine
    {

        private ConcurrentQueue<Command> commandsQueue;

        private List<long> ExpValues;
        public long SkillThreat { get; set; }
        private SkillManager _skillManager;
        private PlayerManager _playerManager;
        private ClassManager _classManager;

        private PropertyManager _propertyManager;
        public Dictionary<string, Entity> Players { get; }
        public Dictionary<string, Entity> Enemies { get; }
        private Dictionary<string, DamageType> DamageTypes { get; }
        private Dictionary<string, MeVariable> DeclaredVariables;
        private Sanitizer Sanit { get; set; }
        private ILogHelper _log;
        public ITimer Timer { get; set; }
        private Dictionary<string,StatusTemplate> statuses;
        public MeNode ExpFormula { get; set; }


        public GameEngine(ILogHelper log)
        {
            Definer.Instance().Init(this);
            _propertyManager = new PropertyManager();
            
            SkillThreat = 5;
            _skillManager = new SkillManager();
            _playerManager = new PlayerManager();
            _playerManager.Engine = this;
            _classManager = new ClassManager();
            _classManager.Engine = this;
            ExpValues = new List<long>();
            _log = log;
            Players = new Dictionary<string, Entity>();
            Enemies = new Dictionary<string, Entity>();
            DamageTypes = new Dictionary<string, DamageType>();
            Timer = new MockTimer();
            Sanit = new Sanitizer(this);
            statuses = new Dictionary<string, StatusTemplate>();
            DeclaredVariables = new Dictionary<string, MeVariable>();

            commandsQueue = new ConcurrentQueue<Command>();

        }

        public void SetStartExp(long value)
        {
            if(ExpValues.Count == 0)
                ExpValues.Add(value);
        }

        public SkillManager GetSkillManager()
        {
            return _skillManager;
        }

        public PlayerManager GetPlayerManager()
        {
            return _playerManager;
        }

        public PropertyManager GetPropertyManager()
        {
            return _propertyManager;
        }

        public ClassManager GetClassManager()
        {
            return _classManager;
        }

        public void AddPlayer(Entity entity)
        {
            if (Players.ContainsKey(entity.Key))
            {
                Players[entity.Key] = entity;
            }
            else
            {
                Players.Add(entity.Key, entity);
            }
        }

        public void AddEnemy(Entity entity)
        {
            Enemies.Add(entity.Key, entity);
        }

        public void AddDamageType(DamageType type)
        {
            DamageTypes.Add(type.Key,type);
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

        public StatusTemplate GetStatusByKey(string key)
        {
            return statuses.ContainsKey(key)?statuses[key]:null;
        }

        public ITimer GetTimer()
        {
            return Timer;
        }

        public Sanitizer GetSanitizer()
        {
            return Sanit;
        }

        public void AddStatus(StatusTemplate status, string key)
        {
            if (!statuses.ContainsKey(key))
                statuses.Add(key, status);
            else
                statuses[key] = status;
        }

        public MeVariable GetVariable(string key)
        {
            return DeclaredVariables.ContainsKey(key) ? DeclaredVariables[key] : null;
        }

        public void AddVariable(string key, MeVariable var)
        {
            if (!DeclaredVariables.ContainsKey(key))
                DeclaredVariables.Add(key, var);
        }

        public void SetVariable(string key, MeVariable var)
        {
            if (DeclaredVariables.ContainsKey(key))
                DeclaredVariables[key] = var;
        }

        public ILogHelper Log()
        {
            return _log;
        }

        public long GetMaxExp(int level)
        {
            if (ExpFormula == null)
                return 0;
            if (ExpValues.Count <= level)
            {
                ExpValues.Capacity = level+1;
                for (int i = ExpValues.Count-1; i <= level; ++i)
                {
                    double prev = ExpValues[i];
                    MeNode sanitized = Sanitizer.ReplaceExpValues(ExpFormula, (long)prev, i+1);
                    ExpValues.Add((long)Math.Floor(sanitized.Resolve().Value.ToDouble()));
                }
            }
            return ExpValues[level];
        }

        public long GetDefaultSkillThreat()
        {
            return SkillThreat;
        }
        public void Update()
        {
            PollCommands();
            //Update entities
            
        }
        public void PollCommands()
        {
            while (!commandsQueue.IsEmpty)
            {
                Command command;
                bool result = commandsQueue.TryDequeue(out command);
                if (result)
                {
                    CommandManager.Instance.Execute(command);
                    //handle command
                }
            }
        }
        public void EnqueueCommand(Command command)
        {
            commandsQueue.Enqueue(command);
        }
    }
}
