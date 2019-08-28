using System;
using System.Text;
using System.IO;

using RPGEngine.Game;
namespace RPGEngine.Utils
{
   

    public class Utility
    {
        private static readonly Random Generator = new Random();
        public static bool Chance(double chance)
        {
            double input = chance*10; //multiply by 10 so we can get chance that's .1% at least
            int dice = Generator.Next(0, 1000);
            return dice < input;
        }

        public static bool DoubleEq(double a, double b)
        {
            return Math.Abs(a - b) < 0.0000000000001;
        }


        public const int BarLength = 10;
        public const char BarFullChar = '▮';
        public const char BarEmptyChar = '▯';

        public static string getBar(double current, double max)
        {
            StringBuilder bar = new StringBuilder();
            int segments = (int)Math.Floor(current / max * BarLength);
            bar.Append('[');
            
            for (int i = 0; i < segments; ++i)
                bar.Append(BarFullChar);
            for (int i = 0; i < BarLength - segments; ++i)
                bar.Append(BarEmptyChar);
            bar.Append($" {current}/{max}");
            bar.Append(']');
            return bar.ToString();
        }

        public static double Clamp(double amount, double min, double max)
        {
            if (amount < min)
                return min;
            if (amount > max)
                return max;
            return amount;
        }


        public static string GetFilePath(string file)
        {
            return Path.Combine(ConfigFiles.BASE_PATH, file + ConfigFiles.EXENSION);
        }
    }
}
