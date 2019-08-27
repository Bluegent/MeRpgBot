
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
    using RPGEngine.Utils;

    public interface IGameEngine
    {
        SkillManager GetSkillManager();
        PlayerManager GetPlayerManager();

        PropertyManager GetPropertyManager();
        ClassManager GetClassManager();

        DamageTypeManager GetDamageTypeManager();
        StatusManager GetStatusManager();

        CoreManager GetCoreManager();


        DuelManager GetDuelManager();

        void AddPlayer(Entity entity);

        void AddEnemy(Entity entity);

        void AddDamageType(DamageTypeTemplate typeTemplate);
        Entity[] GetAllPlayers();

        DamageTypeTemplate GeDamageType(string key);
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

        void EnqueueCommand(Command command);

        void LoadConfigFromFiles();

        void Update();

    }
    public class GameEngine : IGameEngine
    {

        private ConcurrentQueue<Command> commandsQueue;

        private List<long> ExpValues;
        private SkillManager _skillManager;
        private PlayerManager _playerManager;
        private ClassManager _classManager;
        private CoreManager _coreManager;
        private DamageTypeManager _damageTypeManager;
        private StatusManager _statusManager;

        private PropertyManager _propertyManager;

        private DuelManager _duelManager;
        public Dictionary<string, Entity> Players { get; }
        public Dictionary<string, Entity> Enemies { get; }
        private Dictionary<string, MeVariable> DeclaredVariables;
        private Sanitizer Sanit { get; set; }
        private ILogHelper _log;
        public ITimer Timer { get; set; }
        public MeNode ExpFormula { get; set; }


        public GameEngine(ILogHelper log)
        {
            Definer.Instance().Init(this);
            _propertyManager = new PropertyManager();
            _propertyManager.Engine = this;

            _skillManager = new SkillManager();
            _skillManager.Engine = this;

            _coreManager = new CoreManager();
            _coreManager.Engine = this;

            _playerManager = new PlayerManager();
            
            _damageTypeManager = new DamageTypeManager();
            _damageTypeManager.Engine = this;

            _statusManager = new StatusManager();
            _statusManager.Engine = this;

            _playerManager.Engine = this;
            _classManager = new ClassManager();
            _classManager.Engine = this;
            _duelManager = new DuelManager();
            _duelManager.Engine = this;
            ExpValues = new List<long>();
            _log = log;
            Players = new Dictionary<string, Entity>();
            Enemies = new Dictionary<string, Entity>();
            Timer = new MockTimer();
            Sanit = new Sanitizer(this);
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

        public DamageTypeManager GetDamageTypeManager()
        {
            return _damageTypeManager;
        }

        public StatusManager GetStatusManager()
        {
            return _statusManager;
        }

        public CoreManager GetCoreManager()
        {
            return _coreManager;
        }

        public DuelManager GetDuelManager()
        {
            return _duelManager;
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

        public void AddDamageType(DamageTypeTemplate typeTemplate)
        {
           _damageTypeManager.AddDamageType(typeTemplate);
        }

        public Entity[] GetAllPlayers()
        {
            return Players.Values.ToArray();
        }

        public DamageTypeTemplate GeDamageType(string key)
        {
            return _damageTypeManager.GetDamageType(key);
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
            return _statusManager.GetStatus(key);
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
            status.Key = key;
            _statusManager.AddStatus(status);
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


        public void LoadConfigFromFiles()
        {
            _propertyManager.LoadAttributesFromPath(Utility.GetFilePath(ConfigFiles.ATTRIBUTES));
            _propertyManager.LoadBaseValuesFromPath(Utility.GetFilePath(ConfigFiles.BASE_VALUES));
            _propertyManager.LoadResourcesFromPath(Utility.GetFilePath(ConfigFiles.RESOURCES));

            _coreManager = CoreManager.FromFilePath(Utility.GetFilePath(ConfigFiles.CORE),this);

            _damageTypeManager.LoadDamageTypesFromfile(Utility.GetFilePath(ConfigFiles.DAMAGE_TYPES));

            _statusManager.LoadStatusesFromFile(Utility.GetFilePath(ConfigFiles.STATUSES));

            _skillManager.LoadSkillsFromFile(Utility.GetFilePath(ConfigFiles.SKILLS));

            _classManager.LoadClassesFromFile(Utility.GetFilePath(ConfigFiles.CLASSES));
        }

        public void Update()
        {
            PollCommands();
            _playerManager.Update();
            _duelManager.Update();
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
