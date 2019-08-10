using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGEngine.Utils
{
    using System.Runtime.InteropServices;

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
    }
}
