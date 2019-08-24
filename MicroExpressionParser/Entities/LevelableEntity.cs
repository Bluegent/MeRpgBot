
using RPGEngine.Core;

namespace RPGEngine.Entities
{
    public class LevelableEntity : BaseEntity
    {
        public int Level { get; private set; }
        public int AttributePoints { get; private set; }
        public double CurrentExp { get; set; }
        private double _CurrentLevelMaxExp;

        public LevelableEntity(IGameEngine engine) : base(engine)
        {
            Level = 0;
            AttributePoints = 0;
            CurrentExp = 0;
            _CurrentLevelMaxExp = Engine.GetMaxExp(Level);
        }



        public void LevelUp()
        {
            ++Level;
            ++AttributePoints;
            CurrentExp -= _CurrentLevelMaxExp;
            _CurrentLevelMaxExp = Engine.GetMaxExp(Level);
        }
    }
}
