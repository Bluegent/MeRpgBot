using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPGEngine.Core;
using RPGEngine.Language;
using RPGEngine.Parser;

namespace EngineTest.Parser
{
    /// <summary>
    /// Summary description for VariableTest
    /// </summary>
    [TestClass]
    public class VariableTest
    {
        public static IGameEngine Engine;
        public static MockEntity MockPlayer;
        [ClassInitialize]
        public static void StartUp(TestContext ctx)
        {

            Console.WriteLine("START UP");
            Engine = new GameEngine();
            Definer.Get().Engine = Engine;
            MockPlayer = new MockEntity(Engine) { Name = "MOCK_PLAYER", Key = "MOCK_KEY" };
            Engine.AddPlayer(MockPlayer);
            DamageType trueDamage = new DamageType(Engine, "T", null, null, null, null);
            Engine.AddDamageType(trueDamage);
        }

        [TestMethod]
        public void VariableTestAssignOperatorWorks()
        {
            string varName = "testVar";
            double number = 3.14;
            string expression = $"{varName} {Constants.ASSIGN_OP} {number}";
            MeNode tree = TreeConverter.Build(expression, Engine);
            tree.Resolve();
            MeVariable result = Engine.GetVariable(varName);
            Assert.IsNotNull(result);
            Assert.AreEqual(number, result.ToDouble());
        }

        [TestMethod]
        public void VariableTestGetOperatorWorks()
         {
            string varName = "testVar";
            string varName2 = "otherVar";
            double number = 3.14;

            string expression = $"{varName} {Constants.ASSIGN_OP} {number}; {varName2} {Constants.ASSIGN_OP} {Constants.GET_F}({varName});";
            MeNode[] trees = Engine.GetSanitizer().SplitAndResolve(expression);
             foreach (MeNode node in trees)
                 node.Resolve();
            MeVariable result = Engine.GetVariable(varName);
            MeVariable other = Engine.GetVariable(varName2);
            Assert.IsNotNull(result);
            Assert.IsNotNull(other);
            Assert.AreEqual(other.ToDouble(), result.ToDouble());
        }

        [TestMethod]
        public void VariableTestAssignFromFunction()
        {
            string varName = "testVar";
            double[] numbers = {10,11,12};
            StringBuilder sb = new StringBuilder();
            foreach (double num in numbers)
            {
                sb.Append(num);
                sb.Append(",");
            }
            sb.Remove(sb.Length - 1, 1);

            string expression = $"{varName} {Constants.ASSIGN_OP} {Constants.MAX_F}({sb.ToString()})";
            MeNode tree = TreeConverter.Build(expression, Engine);
            tree.Resolve();
            MeVariable result = Engine.GetVariable(varName);
            Assert.IsNotNull(result);
            Assert.AreEqual(numbers.Max(), result.ToDouble());
        }

        [TestMethod]
        public void VariableTestAssignFromFunctionWithOp()
        {
            string varName = "testVar";
            double[] numbers = { 10, 11, 12 };
            double addedNumber = 5;
            StringBuilder sb = new StringBuilder();
            foreach (double num in numbers)
            {
                sb.Append(num);
                sb.Append(",");
            }
            sb.Remove(sb.Length - 1, 1);

            string expression = $"{varName} {Constants.ASSIGN_OP} {Constants.MAX_F}({sb.ToString()})+{addedNumber}";
            MeNode tree = TreeConverter.Build(expression, Engine);
            tree.Resolve();
            MeVariable result = Engine.GetVariable(varName);
            Assert.IsNotNull(result);
            Assert.AreEqual(numbers.Max()+addedNumber, result.ToDouble());
        }



        [TestMethod]
        public void VariableTestSimpleReference()
        {
            string varName = "testVar";
            string varName2 = "otherVar";
            double number = 3.14;

            string expression = $"{varName} {Constants.ASSIGN_OP} {number}; {varName2} {Constants.ASSIGN_OP} {varName};";
            MeNode[] trees = Engine.GetSanitizer().SplitAndResolve(expression);
            foreach (MeNode node in trees)
                node.Resolve();
            MeVariable result = Engine.GetVariable(varName);
            MeVariable other = Engine.GetVariable(varName2);
            Assert.IsNotNull(result);
            Assert.IsNotNull(other);
            Assert.AreEqual(other.ToDouble(), result.ToDouble());
        }
    }
}

