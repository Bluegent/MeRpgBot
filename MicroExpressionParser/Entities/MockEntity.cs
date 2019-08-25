using RPGEngine.Core;
using RPGEngine.Parser;

namespace RPGEngine.Entities
{

    public class MockEntity : LevelableEntity
    {

        public static ResourceTemplate getMockHP(IGameEngine engine)
        {
            MeNode max = TreeConverter.Build("VIT*20", engine); ;
            MeNode regen = TreeConverter.Build("0", engine);
            ResourceTemplate hp = new ResourceTemplate(Entity.HP_KEY,max,regen);
            return hp;
        }
         
        public MockEntity(IGameEngine engine)
            : base(engine)
        {
            AddAttribute("VIT",5);
            AddAttribute("STR", 5);
            AddAttribute("INT", 5);
            AddAttribute("AGI", 10);
            AddAttribute("DEF", 10);
            AddAttribute("MDEF", 10);
            AddResource(getMockHP(engine));
            MeNode mpNode = TreeConverter.Build("INT*10", engine);
            MeNode mpRegen = TreeConverter.Build("INT/100", engine);
            ResourceTemplate resTemp = new ResourceTemplate("MP", mpNode, mpRegen);
            AddResource(resTemp);

        }
    }
}
