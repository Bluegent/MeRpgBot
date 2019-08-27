namespace RPGEngine.Core
{
    using System.Collections.Generic;

    using Newtonsoft.Json.Linq;

    using RPGEngine.Game;
    using RPGEngine.GameConfigReader;
    using RPGEngine.Utils;

    public class ClassManager
    {
        private Dictionary<string, ClassTemplate> _classes;
        public IGameEngine Engine { get; set; }
        public ClassManager()
        {
            _classes = new Dictionary<string, ClassTemplate>();
        }

        public bool HasClass(string key)
        {
            return _classes.ContainsKey(key);
        }

        public ClassTemplate GetClass(string key)
        {
            return HasClass(key) ? _classes[key] : null;
        }

        public void AddClass(ClassTemplate classT)
        {
            if (_classes.ContainsKey(classT.Key))
            {
                throw new MeException($"Attempting to add class {classT.Name} but a class with key {classT.Key} already exists.");
            }
            _classes.Add(classT.Key,classT);
        }

        public void LoadClassesFromFile(string path)
        {
            ClassReader reader = new ClassReader(Engine);
            JArray json = FileReader.FromPath<JArray>(path);

            foreach (JToken entry in json)
            {
                if (entry.Type != JTokenType.Object)
                {
                    throw new MeException($"Expected a json object \"{path}\"at  \"{entry}\".");
                }

                ClassTemplate newEntry = reader.FromJson(entry.ToObject<JObject>());
                AddClass(newEntry);
            }
        }
    }
}