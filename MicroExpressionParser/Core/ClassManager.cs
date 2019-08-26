namespace RPGEngine.Core
{
    using System.Collections.Generic;

    using RPGEngine.Game;

    public class ClassManager
    {
        private Dictionary<string, ClassTemplate> _classes;
        public IGameEngine Engine { get; set; }
        public ClassManager()
        {
            LoadClasses();
        }

        private void LoadClasses()
        {
            _classes = new Dictionary<string, ClassTemplate>();
        }

        public bool ClassExists(string key)
        {
            return _classes.ContainsKey(key);
        }

        public ClassTemplate GetClass(string key)
        {
            if (ClassExists(key))
                return _classes[key];
            return null;
        }
    }
}