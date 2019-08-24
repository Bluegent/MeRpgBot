using System;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPGEngine.Core;
using RPGEngine.GameInterface;
using RPGEngine.Language;
using RPGEngine.Parser;

namespace EngineTest.Leveling
{
    [TestClass]
    public class LevelingTest
    {
        public static readonly GameEngine Engine = new GameEngine(new DiscordLogHelper(new ConsoleLogger()));
        public const long StartExp = 500;

        public static long expHelper(long prev, int level)
        {
            double power = Math.Floor((double)level / 5.0);
            double multiplier = Math.Pow(2, power);
            long current = (long)Math.Floor(prev + 50 * multiplier);
            return current;
        }

        [ClassInitialize]
        public static void StartUp(TestContext ctx)
        {
            Definer.Instance().Engine = Engine;
            Engine.ExpFormula = TreeConverter.Build($"{LConstants.ExpPrevKeyword}+50*2^{LConstants.FLOOR_F}({LConstants.LevelKeyword}/5.0)", Engine);
            Engine.SetStartExp(StartExp);
        }

        [TestMethod]
        public void LevelingTestPowerOperator()
        {
            double prev = 500;
            for (int i = 0; i < 20; ++i)
            {
                MeNode ExpFormula = TreeConverter.Build($"{prev}+50*2^{LConstants.FLOOR_F}({i}/5.0)",
                    Engine);
                double res = ExpFormula.Resolve().Value.ToDouble();
                double exp = prev+50*Math.Pow(2, Math.Floor(i/5.0));
                prev = exp;
                Assert.AreEqual(exp,res);
            }
        }

        [TestMethod]
        public void LevelingTestNextLevelExp()
        {
;           long expected = StartExp;
            for (int i = 0; i <= 40; ++i)
            {
               
                Assert.AreEqual(expected, Engine.GetMaxExp(i));
                long prev = expected;
                expected = expHelper(prev, i+1);
            }
        }

        [TestMethod]
        public void LevelingTestNextLevelExpJump()
        {
             long expected = StartExp;
            for (int i = 0; i < 40; ++i)
            {


                long prev = expected;
                expected = expHelper(prev, i + 1);
            }

            Assert.AreEqual(expected, Engine.GetMaxExp(40));
        }
    }
}
