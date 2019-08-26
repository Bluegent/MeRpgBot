
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPGEngine.Core;
using RPGEngine.Entities;
using RPGEngine.GameInterface;

namespace EngineTest.Core
{



    [TestClass]
    public class DeathTest
    {
        private static readonly GameEngine Engine = new GameEngine(new DiscordLogHelper(new ConsoleLogger()));


        [ClassInitialize]
        public static void StartUp(TestContext context)
        {
        }

        [TestMethod]
        public void DeathTestKill()
        {
            BaseEntity player = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };
            player.Kill();
            player.Update();
            Assert.AreEqual(true, player.IsDead);
        }

        [TestMethod]
        public void DeathTestRevive()
        {
            BaseEntity player = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };
            player.Kill();
            player.Update();
            Assert.AreEqual(true, player.IsDead);
            player.Update();
            Assert.AreEqual(false, player.IsDead);
            ResourceInstance hp = player.GetResource(Entity.HP_KEY);
            Assert.AreEqual(hp.MaxAmount * hp.Modifier, hp.Value);
        }


        [TestMethod]
        public void DeathTestReviveLater()
        {
            BaseEntity player = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };
            long reviveDuration = 10;
            player.ReviveDuration = new MeNode(reviveDuration);
            player.RefreshProperties();
            player.Kill();
            MockTimer timer = (MockTimer)Engine.GetTimer();
            for (int i = 0; i < reviveDuration; ++i)
            {
                player.Update();
                timer.ForceTick();
                Assert.AreEqual(true, player.IsDead);
            }
            player.Update();
            Assert.AreEqual(false, player.IsDead);
            ResourceInstance hp = player.GetResource(Entity.HP_KEY);
            Assert.AreEqual(hp.MaxAmount * hp.Modifier, hp.Value);
        }
    }
}