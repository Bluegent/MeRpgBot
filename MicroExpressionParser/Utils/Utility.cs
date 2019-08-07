using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RPGEngine.Utils
{
    public class Utility
    {
        public static bool Chance(double chance)
        {
            double input = chance * 10;
            int dice = new Random().Next(0, 1000);
            return dice < input;
        }
    }
}
