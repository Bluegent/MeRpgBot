namespace RPGEngine.Core
{
    using RPGEngine.Game;

    public class DuelInstance
    {
        public IGameEngine Engine { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
        public bool Ended { get; set; }


        private void ExitDuel()
        {
            Player1.Dueling = false;
            Player2.Dueling = false;
            Player1.Entity.Revive();
            Player2.Entity.Revive();
            Player1.Entity.Target(null);
            Player2.Entity.Target(null);
            Player1.Duel = null;
            Player2.Duel = null;
            Ended = true;
        }

        public void EndDuel()
        {
            //log ended
            ExitDuel();
        }
        public void Update()
        {
            if (Player1.Entity.IsDead && Player2.Entity.IsDead)
            {
                Engine.Log().Log($"The duel between {Player1.Entity.Name} and {Player2.Entity.Name} ended in a Draw.");


            }
            else if (Player1.Entity.IsDead)
            {
                Engine.Log().Log($" {Player2.Entity.Name} won the duel against {Player2.Entity.Name}.");
                //log player2 wins

            }
            else if (Player2.Entity.IsDead)
            {
                Engine.Log().Log($" {Player1.Entity.Name} won the duel against {Player1.Entity.Name}.");
            }
            ExitDuel();

        }
        public static DuelInstance Create(Player player1, Player player2, IGameEngine engine)
        {
            player1.Entity.Target(player2.Entity);
            player2.Entity.Target(player1.Entity);
            player1.Dueling = true;
            player2.Dueling = true;
            DuelInstance instance = new DuelInstance();
            instance.Player1 = player1;
            instance.Player2 = player2;
            instance.Player1.Duel = instance;
            instance.Player2.Duel = instance;
            instance.Engine = engine;
            instance.Ended = false;
            engine.Log().Log($"{player2.Entity.Name} accepted a duel challenge from {player1.Entity.Name}");
            return instance;
        }
    }
}