namespace RPGEngine.Core
{
    using System.Dynamic;

    using Newtonsoft.Json.Linq;

    using RPGEngine.Game;
    using RPGEngine.Utils;

    public class CoreManager
    {
       public MeNode ReviveTime { get; set; }
       public MeNode ExpFormula { get; set; }

       public long DefaultSkillThreat { get; set; }
       public long AttributePointsPerLevel { get; set; }
       public long StartExp { get; set; }
       public long MaxLevel { get; set; }
       public IGameEngine Engine { get; set; }


       public static CoreManager FromFilePath(string filePath, IGameEngine engine)
       {
           CoreReader reader = new CoreReader(engine);
           CoreManager result = reader.FromJson(FileReader.FromPath<JObject>(filePath));
           result.Engine = engine;
           return result;
       }
    }
}