namespace RPGEngine.Managers
{
    using System.Collections.Generic;

    using RPGEngine.Core;
    using RPGEngine.Game;

    public class DuelManager
    {
        public IGameEngine Engine { get; set; }
        private List<DuelInstance> _duels;
        private List<DuelInstance> _toRemove;

        public DuelManager()
        {
            _duels = new List<DuelInstance>();
            _toRemove = new List<DuelInstance>();
        }
        public void CreateDuel(Player challenger, Player target)
        {
            DuelInstance instance = DuelInstance.Create(challenger,target,Engine);
            _duels.Add(instance);
        }

        public void Update()
        {
            
            foreach (DuelInstance duel in _duels)
            {
                duel.Update();
                if (duel.Ended)
                    _toRemove.Add(duel);
            }
            if (_toRemove.Count != 0)
            {
                foreach (DuelInstance duel in _toRemove)
                {
                    _duels.Remove(duel);
                }
                _toRemove.Clear();
            }
        }
    }
}