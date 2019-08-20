using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPGEngine.Discord;

namespace EngineTest.Discord
{
    [TestClass]
    public class CommandTest
    {
        [TestMethod]
        public void CommandTestRegular()
        {
            long userId = 0;
            Command expect=new Command(){UserId = userId, Name = "testCommand", Args = new string[]{"afa","asd"}};
            string command = expect.ToString();
            Command result = Command.FromMessage(userId, command);
            Assert.AreEqual(expect.UserId,result.UserId);
            Assert.AreEqual(expect.Name,result.Name);
            CollectionAssert.AreEqual(expect.Args,result.Args);
        }

        [TestMethod]
        public void CommandTestQuote()
        {
            long userId = 0;
            Command expect = new Command() { UserId = userId, Name = "testCommand", Args = new string[] { "afa", "asd fgh" } };
            string command = expect.ToString();
            Command result = Command.FromMessage(userId, command);
            Assert.AreEqual(expect.UserId, result.UserId);
            Assert.AreEqual(expect.Name, result.Name);
            CollectionAssert.AreEqual(expect.Args, result.Args);
        }
    }
}
