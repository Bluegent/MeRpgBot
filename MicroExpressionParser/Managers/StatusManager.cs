﻿namespace RPGEngine.Managers
{
    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    using RPGEngine.Core;
    using RPGEngine.Game;
    using RPGEngine.GameConfigReader;
    using RPGEngine.Templates;
    using RPGEngine.Utils;

    public class StatusManager
    {
        public IGameEngine Engine { get; set; }

        public Dictionary<string, StatusTemplate> Statuses;

        public StatusManager()
        {
            Statuses = new Dictionary<string, StatusTemplate>();
        }

        public bool HasStatus(string key)
        {
            return Statuses.ContainsKey(key);
        }

        public StatusTemplate GetStatus(string key)
        {
            return Statuses.ContainsKey(key) ? Statuses[key] : null;
        }

        public void AddStatus(StatusTemplate status)
        {
            if (Statuses.ContainsKey(status.Key))
            {
                throw new MeException($"Duplicate key for {status.Key}({status.Name}).");
            }
            Statuses.Add(status.Key, status);
        }

        public void LoadStatusesFromFile(string path)
        {
            StatusReader reader = new StatusReader(Engine);
            JArray json = FileHandler.FromPath<JArray>(path);

            foreach (JToken entry in json)
            {
                if (entry.Type != JTokenType.Object)
                {
                    throw new MeException($"Expected a json object \"{path}\"at  \"{entry}\".");
                }

                StatusTemplate newEntry = reader.FromJSON(entry.ToObject<JObject>());
                AddStatus(newEntry);
            }
        }
    
    }
}