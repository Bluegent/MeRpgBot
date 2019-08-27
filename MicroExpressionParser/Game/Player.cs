using System.Collections.Generic;
using RPGEngine.Core;
using RPGEngine.Entities;

namespace RPGEngine.Game
{
    public class Player 
    {
        public ClassTemplate Class { get; }
        public Dictionary<string, double> Stats;
        public LevelableEntity Entity { get; }
        public long Id { get; set; }
        public bool Dueling { get; set; }
        public IGameEngine Engine { get; set; }
        public DuelInstance Duel { get; set; }
        public Queue<Player> DuelRequests { get; set; }
        public Player(IGameEngine engine, long id, ClassTemplate myClass)
        {
            Engine = engine;
            Id = id;
            Stats = new Dictionary<string, double>();
            Class = myClass;
            Dictionary<string, SkillInstance> skills = new Dictionary<string, SkillInstance>();
            //TODO: deserialize properly
            foreach (SkillTemplate skill in Class.Skills.Values)
            {
                skills.Add(skill.Key,new SkillInstance() {Skill =  skill,SkillLevel = 0});
            }

            Entity = new LevelableEntity(engine);
            Entity.Skills = skills;
            Dueling = false;
            Duel = null;
        }

        public void AcceptDuel()
        {
            Player challenger = DuelRequests.Dequeue();
            Engine.GetDuelManager().CreateDuel(challenger,this);
        }

        public void RejectDuel()
        {
            Player challenger = DuelRequests.Dequeue();
            //log reject
            Engine.Log().Log($" {Entity.Name} rejected the challenge from {challenger.Entity.Name}.");
        }

        public void EndDuel()
        {
            
            if (Duel != null)
            {
                Player enemy = Duel.Player1.Entity.Name == Entity.Name ? Duel.Player2 : Duel.Player1;
                Engine.Log().Log($" {Entity.Name} ended the duel against {enemy.Entity.Name}.");
                Duel.EndDuel();

                //log exit
            }
        }

        public void AddChallenge(Player challenger)
        {
            Engine.Log().Log($" {challenger.Entity.Name} challenged {Entity.Name} to a duel.");
            DuelRequests.Enqueue(challenger);
        }
    }
}
