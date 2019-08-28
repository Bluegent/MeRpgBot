using RPGEngine.Core;
using RPGEngine.Parser;

namespace RPGEngine.Entities
{
    using System;

    using RPGEngine.GameConfigReader;
    using RPGEngine.Templates;

    public class MockEntity : LevelableEntity
    {

        public static ResourceTemplate getMockHP(IGameEngine engine)
        {
            MeNode max = TreeConverter.Build("VIT*20", engine); ;
            MeNode regen = TreeConverter.Build("0", engine);
            ResourceTemplate hp = new ResourceTemplate();
            hp.Formula = max;
            hp.RegenFormula = regen;
            hp.Key = Entity.HP_KEY;
            hp.RegenInterval = new MeNode(GcConstants.Resources.DEFAULT_INTERVAL);
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
            ResourceTemplate resTemp = new ResourceTemplate();
            resTemp.Formula = mpNode;
            resTemp.RegenFormula = mpRegen;
            resTemp.Key = "MP";
            resTemp.RegenInterval = new MeNode(GcConstants.Resources.DEFAULT_INTERVAL);
            AddResource(resTemp);

        }
    }
}
