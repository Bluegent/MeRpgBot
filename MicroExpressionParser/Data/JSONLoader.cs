using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.IO;

namespace EngineTest
{
    using Newtonsoft.Json;

    public class JsonLoader
    {

        public static JObject LoadFromFile(string path)
        {
            JObject result = null;
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    if (sr != null)
                    {
                        string json = sr.ReadToEnd();
                        result = JsonConvert.DeserializeObject<JObject>(json);
                        sr.Close();

                    }
                }

                
            }
            return result;

        }
    }
}
