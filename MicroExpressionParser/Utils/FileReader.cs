using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPGEngine.Utils
{
    public class FileReader
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
    }
}
