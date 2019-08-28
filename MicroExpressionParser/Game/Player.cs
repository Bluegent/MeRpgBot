using System.Collections.Generic;
using RPGEngine.Core;
using RPGEngine.Entities;

namespace RPGEngine.Game
{
    using RPGEngine.GameConfigReader;

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
            DuelRequests = new Queue<Player>();
            //TODO: deserialize properly
            foreach (SkillTemplate skill in Class.Skills.Values)
            {
                skills.Add(skill.Key,new SkillInstance() {Skill =  skill,SkillLevel = 0});
            }
            skills.Add(Class.BaseAttack.Key,new SkillInstance(){Skill = Class.BaseAttack,SkillLevel = 0});
            Entity = new LevelableEntity(engine);
            foreach (var baseValue in Class.BasicValues)
            {
                Entity.AddBaseValue(baseValue.Key, baseValue.Value);
            }
            foreach (var attr in Class.Attributes)
            {
                Entity.AddAttribute(attr.Key,attr.Value);
            }

            foreach (StatTemplate stat in Engine.GetStatManager().GetStats())
            {
                Entity.AddStat(stat);
            }

            Entity.AddResource(Engine.GetPropertyManager().GetResource(Entities.Entity.HP_KEY));
            foreach (ResourceTemplate resource in Class.Resources.Values)
            {
                Entity.AddResource(resource);
            }
            
            
            Entity.Skills = skills;
            Entity.Key = Id.ToString();
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

        public void DisplayOverall()
        {
            string displayString =
                $"{Entity.Name}[Lvl {Entity.Level + 1} {Class.Name}]\n\n{Entity.GetResourcesString()}{Utils.Utility.getBar(Entity.CurrentExp, Entity.CurrentLevelMaxExp)} EXP";
            Engine.Log().Log(displayString);
        }
    }
}
