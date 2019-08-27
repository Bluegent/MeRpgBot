using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using RPGEngine.Core;
using RPGEngine.Game;
using RPGEngine.GameConfigReader;
using RPGEngine.Utils;

namespace RPGEngine.Manager
{
    public class StatManager
    {
        private Dictionary<string, StatTemplate> _stats;
        private IGameEngine _engine;
        public StatManager(IGameEngine engine)
        {
            _engine = engine;
            _stats = new Dictionary<string, StatTemplate>();
        }

        public bool HasStat(string key)
        {
            return _stats.ContainsKey(key);
        }

        public void AddStat(StatTemplate stat)
        {
            if (_stats.ContainsKey(stat.Key))
            {
                throw new MeException($"Attempted to add stat with key {stat.Key}, but a stat with this key already exists.");
            }
            _stats.Add(stat.Key,stat);
        }

        public StatTemplate GetStat(string key)
        {
            return _stats.ContainsKey(key) ? _stats[key] : null;
        }


        public void LoadStatsFromFile(string path)
        {
            StatReader reader = new StatReader(_engine);
            JArray json = FileReader.FromPath<JArray>(path);

            foreach (JToken entry in json)
            {
                if (entry.Type != JTokenType.Object)
                {
                    throw new MeException($"Expected a json object \"{path}\"at  \"{entry}\".");
                }

                StatTemplate newEntry = reader.FromJson(entry.ToObject<JObject>());
                AddStat(newEntry);
            }

        }

        public StatTemplate[] GetStats()
        {
            return _stats.Values.ToArray();
        }

    }
}