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

       public CoreManager()
       {

       }

       public static CoreManager FromFilePath(string filePath, IGameEngine engine)
       {
           CoreReader reader = new CoreReader(engine);
           return reader.FromJson(FileReader.FromPath<JObject>(filePath));
       }
    }
}