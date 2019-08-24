
using RPGEngine.Core;

namespace RPGEngine.Entities
{
    public class LevelableEntity : BaseEntity
    {
        public long Level { get; private set; }
        public LevelableEntity(IGameEngine engine) : base(engine)
        {
            Level = 0;
        }
    }
}
