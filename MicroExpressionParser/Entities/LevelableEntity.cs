
using RPGEngine.Core;

namespace RPGEngine.Entities
{
    public class LevelableEntity : BaseEntity
    {
        public int Level { get; private set; }
        public int AttributePoints { get; private set; }
        public double CurrentExp { get; set; }
        public double CurrentLevelMaxExp { get; set; }

        public LevelableEntity(IGameEngine engine) : base(engine)
        {
            Level = 0;
            AttributePoints = 0;
            CurrentExp = 0;
            CurrentLevelMaxExp = Engine.GetMaxExp(Level);
        }

        public void AddExp(long amount)
        {
            CurrentExp += amount;
            if(CurrentExp >= CurrentLevelMaxExp)
                LevelUp();
        }

        private void LevelUp()
        {
            ++Level;
            ++AttributePoints;
            CurrentExp -= CurrentLevelMaxExp;
            CurrentLevelMaxExp = Engine.GetMaxExp(Level);
        }

        public bool AssignAttributePoint(string key)
        {
            if (AttributePoints == 0)
            {
                //log that you don't have attribute points
                return false;
            }

            if (!Attributes.ContainsKey(key))
            {
                //log that you don't have that attribute
                return false;
            }

            ++Attributes[key].FromPoints;
            RefreshProperties();

            return true;
        }
    }
}
