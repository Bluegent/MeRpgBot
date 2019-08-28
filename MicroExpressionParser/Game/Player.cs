using System.Collections.Generic;
using RPGEngine.Core;
using RPGEngine.Entities;

namespace RPGEngine.Game
{
    using System;

    using Newtonsoft.Json.Linq;

    using RPGEngine.GameConfigReader;
    using RPGEngine.GameInterface;
    using RPGEngine.Templates;

    public class Player : IJsonSerializable
    {
        public ClassTemplate Class { get; private set; }
        public LevelableEntity Entity { get; private set; }
        public long Id { get; set; }
        public bool Dueling { get; set; }
        public IGameEngine Engine { get; set; }
        public DuelInstance Duel { get; set; }
        public Queue<Player> DuelRequests { get; set; }

        public Player()
        {

        }
        public Player(IGameEngine engine, long id, ClassTemplate myClass)
        {
           Initialize(engine,id,myClass);
        }

        private void Initialize(IGameEngine engine, long id, ClassTemplate myClass)
        {
            Engine = engine;
            Id = id;
            Class = myClass;
            Dictionary<string, SkillInstance> skills = new Dictionary<string, SkillInstance>();
            DuelRequests = new Queue<Player>();
            //TODO: deserialize properly
            foreach (SkillTemplate skill in Class.Skills.Values)
            {
                skills.Add(skill.Key, new SkillInstance() { Skill = skill, SkillLevel = 0 });
            }
            skills.Add(Class.BaseAttack.Key, new SkillInstance() { Skill = Class.BaseAttack, SkillLevel = 0 });
            Entity = new LevelableEntity(engine);
            foreach (var baseValue in Class.BasicValues)
            {
                Entity.AddBaseValue(baseValue.Key, baseValue.Value);
            }
            foreach (var attr in Class.Attributes)
            {
                Entity.AddAttribute(attr.Key, attr.Value);
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
            Entity.ReviveDuration = Engine.GetCoreManager().ReviveTime;
            Dueling = false;
            Duel = null;
            Entity.RefreshProperties();
        }

        public void AcceptDuel()
        {
            Player challenger = DuelRequests.Dequeue();
            Engine.GetDuelManager().CreateDuel(challenger,this);
        }

        public void RejectDuel()
        {
            Player challenger = DuelRequests.Dequeue();
            Engine.Log().Log($" {Entity.Name} rejected the challenge from {challenger.Entity.Name}.");
        }

        public void EndDuel()
        {
            
            if (Duel != null)
            {
                Player enemy = Duel.Player1.Entity.Name == Entity.Name ? Duel.Player2 : Duel.Player1;
                Engine.Log().Log($" {Entity.Name} ended the duel against {enemy.Entity.Name}.");
                Duel.EndDuel();
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

        public JObject ToJObject()
        {
            JObject result=new JObject();
            result.Add(GcConstants.General.ID,Id);
            result.Add(GcConstants.General.CLASS,Class.Key);
            JObject entity = Entity.ToJObject();
            result.Add(GcConstants.General.ENTITY,entity);
            return result;
        }
        
        public bool FromJObject(JObject obj, IGameEngine engine)
        {
            long id = obj[GcConstants.General.ID].ToObject<long>();
            string classKey = obj[GcConstants.General.CLASS].ToObject<string>();
            if (!engine.GetClassManager().HasClass(classKey))
                return false;
            ClassTemplate template = engine.GetClassManager().GetClass(classKey);
            Initialize(engine,id,template);
            JObject entity = obj[GcConstants.General.ENTITY].ToObject<JObject>();
            Entity.FromJObject(entity,engine);
            Entity.Key = id.ToString();
            return true;
        }
    }
}
