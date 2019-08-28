namespace RPGEngine.Core
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;

    using Newtonsoft.Json.Linq;

    using RPGEngine.Cleanup;
    using RPGEngine.Game;
    using RPGEngine.Utils;

    public class CoreManager
    {
        private List<long> ExpValues;

        private long _startExp;
        public MeNode ReviveTime { get; set; }
       public MeNode ExpFormula { get; set; }

       public long DefaultSkillThreat { get; set; }
       public long AttributePointsPerLevel { get; set; }
       public long StartExp
       {
           get
           {
               return _startExp;
           }
           set
           {
               _startExp = value;
               ExpValues.Clear();
               ExpValues.Add(StartExp);
            }
       }

       public long MaxLevel { get; set; }
       public IGameEngine Engine { get; set; }

       public CoreManager()
       {
           ExpValues = new List<long>();
           
        }
       public long GetMaxExp(int level)
       {
           if (ExpFormula == null)
               return 0;
           if (ExpValues.Count <= level)
           {
               ExpValues.Capacity = level + 1;
               for (int i = ExpValues.Count - 1; i <= level; ++i)
               {
                   double prev = ExpValues[i];
                   MeNode sanitized = Sanitizer.ReplaceExpValues(ExpFormula, (long)prev, i + 1);
                   ExpValues.Add((long)Math.Floor(sanitized.Resolve().Value.ToDouble()));
               }
           }
           return ExpValues[level];
       }
       public static CoreManager FromFilePath(string filePath, IGameEngine engine)
       {
           CoreReader reader = new CoreReader(engine);
           CoreManager result = reader.FromJson(FileReader.FromPath<JObject>(filePath));
           result.Engine = engine;
           return result;
       }
    }
}