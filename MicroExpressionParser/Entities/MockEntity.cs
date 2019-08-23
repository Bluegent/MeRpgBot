using RPGEngine.Core;
using RPGEngine.Parser;

namespace RPGEngine.Entities
{

    public class MockEntity : BaseEntity
    {
        public MockEntity(IGameEngine engine)
            : base(engine)
        {
            AddAttribute("CHP", 100);
            AddAttribute("MHP", 100);
            AddAttribute("STR", 5);
            AddAttribute("INT", 5);
            AddAttribute("AGI", 10);
            AddAttribute("DEF", 10);
            AddAttribute("MDEF", 10);

            MeNode mpNode = TreeConverter.Build("INT*10", engine);
            MeNode mpRegen = TreeConverter.Build("INT/100", engine);
            ResourceTemplate resTemp = new ResourceTemplate("MP", mpNode, mpRegen);
            AddResource(resTemp);

        }
    }
}
