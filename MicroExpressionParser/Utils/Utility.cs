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
        public const char SeparatorChar = '=';

        public const int ValueAlign = 10;

        public const string Filler = "...";

        public static string getBar(double current, double max)
        {
            StringBuilder bar = new StringBuilder();
            int segments = (int)Math.Floor(current / max * BarLength);
            bar.Append('[');
            
            for (int i = 0; i < segments; ++i)
                bar.Append(BarFullChar);
            for (int i = 0; i < BarLength - segments; ++i)
                bar.Append(BarEmptyChar);
            bar.Append($" {GetDisplayValue(current)}/{GetDisplayValue(max)}".PadLeft(ValueAlign, ' '));
            bar.Append(']');
            return bar.ToString();
        }

        public static string GetSeparatorLine(int length)
        {
            string result = "";
            for (int i = 0; i < length; ++i)
                result += SeparatorChar;
            return result;
        }

        public static string TruncateAndAlign(string input, int size)
        {
            if (input.Length > size +Filler.Length)
                return input.Substring(0, size)+Filler;
            return input;
        }

        public static string FormatSeconds(double seconds)
        {
            if (seconds <= 0)
                return "now";
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            int h = time.Hours;
            int m = time.Minutes;
            int s = time.Seconds;
            string hs = (h > 0) ? $"{h}h " : "";
            string ms = (m > 0) ? $"{m}m " : "";
            string ss = (s > 0) ? $"{s}s" : "";
            return hs + ms + ss;
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
            if (!Directory.Exists(ConfigFiles.BASE_PATH))
                Directory.CreateDirectory(ConfigFiles.BASE_PATH);
            return Path.Combine(ConfigFiles.BASE_PATH, file + ConfigFiles.EXENSION);
        }
        public static string GetPersistenceFilePath(string file)
        {
            if (!Directory.Exists(PersistenceFiles.BASE_PATH))
                Directory.CreateDirectory(PersistenceFiles.BASE_PATH);
            return Path.Combine(PersistenceFiles.BASE_PATH, file + PersistenceFiles.EXENSION);
        }

        private static string GetDisplayValue(double value)
        {
            String result = null;
            double[] values =
                { 1E33,1E30,1E27,1E24,1E21,1E18,1E15,1E12,1E9,1E6,1E3,1};
            string[] sufix = { "D", "N", "O", "S", "s", "Q", "q", "T", "B", "M", "k", "" };
            int index = 0;
            bool finish = false;
            if (value > 1)
            {
                while (!finish)
                {
                    double temp = value / values[index];
                    if (temp >= 1 && temp < 1000)
                        finish = true;
                    index++;
                }
                index--;
                double displayValue = value / values[index];
                result = $"{displayValue:.}{sufix[index]}";
            }
            else
            {
                result = value.ToString("0");
            }
            return result;
        }

    }
}
