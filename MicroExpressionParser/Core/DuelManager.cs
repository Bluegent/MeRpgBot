namespace RPGEngine.Core
{
    using System.Collections.Generic;

    using RPGEngine.Game;

    public class DuelManager
    {
        public IGameEngine Engine { get; set; }
        private List<DuelInstance> _duels;

        public DuelManager()
        {
            _duels = new List<DuelInstance>();
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
                    _duels.Remove(duel);
            }
        }
    }
}