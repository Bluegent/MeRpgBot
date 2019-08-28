using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGEngine.Utils
{
    using System.Data.SqlClient;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class FileHandler
    {
        public static string Read(string path)
        {
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string json = sr.ReadToEnd();
                    sr.Close();
                    return json;
                }
            }
            return null;
        }

        public static T FromPath<T>(string path)
        {
            string data = Read(path);
            if(data!=null)
                return JsonConvert.DeserializeObject<T>(Read(path));
            return default(T);
        }

        public static void Write(string path, string data)
        {
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                sw.Write(data);
                sw.Flush();
                sw.Close();
            }
        }

        public static void WriteJson(string path, JObject data)
        {
            string dataString = JsonConvert.SerializeObject(data, Formatting.Indented);
        }
    }
}
