using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EngineTest.GameConfigReader
{
    using Newtonsoft.Json.Linq;

    using RPGEngine.Core;
    using RPGEngine.Game;
    using RPGEngine.GameConfigReader;
    using RPGEngine.GameInterface;
    using RPGEngine.Language;
    using RPGEngine.Parser;

    [TestClass]
    public class ClassReaderTest
    {

        public static readonly GameEngine Engine = new GameEngine(new DiscordLogHelper(new ConsoleLogger()));
        public static readonly ClassReader ClassReader = new ClassReader(Engine);
        private static SkillTemplate _testSkill;
        [ClassInitialize]
        public static void StartUp(TestContext ctx)
        {
            Definer.Instance().Engine = Engine;
            DamageTypeTemplate trueDamage = new DamageTypeTemplate(Engine, null, null, null, null);
            trueDamage.Name = "true";
            trueDamage.Key = "T";
            Engine.AddDamageType(trueDamage);
            SkillCost nullCost = new SkillCost("MP", TreeConverter.Build("0", Engine));
           
            _testSkill = new SkillTemplate();
            _testSkill.Type = SkillType.Cast;
            _testSkill.Key = "TEST_CAST";
            SkillLevelTemplate testLevelTemplate = new SkillLevelTemplate();
            testLevelTemplate.Cooldown = TreeConverter.Build("3", Engine);
            testLevelTemplate.Duration = TreeConverter.Build($"{LConstants.SourceKeyword}{LConstants.PROP_OP}INT", Engine);
            testLevelTemplate.Interruptible = TreeConverter.Build("true", Engine);
            testLevelTemplate.Formulas.Add(TreeConverter.Build($"{LConstants.HARM_F}({LConstants.TargetKeyword},{LConstants.SourceKeyword},{trueDamage.Key},10)", Engine));
            testLevelTemplate.PushBack = TreeConverter.Build("true", Engine);
            testLevelTemplate.Cost = nullCost;
            _testSkill.ByLevel.Add(testLevelTemplate);

            Engine.GetSkillManager().AddSkill(_testSkill);
        }

        [TestMethod]
        public void ClassReaderTestNonDefaultValues()
        {
            string baseAttack = _testSkill.Key;
            string description = "test class";
            string classKey = "test";
            string className = "Test Class";
            JObject baseDmg = new JObject();
            baseDmg.Add(GcConstants.General.KEY,"BASE_DMG");
            baseDmg.Add(GcConstants.General.VALUE,"10");
            JObject intAttr = new JObject();
            intAttr.Add(GcConstants.General.KEY, "INT");
            intAttr.Add(GcConstants.General.VALUE, "5");
            JObject json = new JObject();
            json.Add(GcConstants.Classes.BASE_ATTACK,baseAttack);
            JArray baseValues = new JArray();
            baseValues.Add(baseDmg);
            JArray basicAttributes = new JArray();
            string[] skills = {_testSkill.Key};
            basicAttributes.Add(intAttr);
            json.Add(GcConstants.General.KEY,classKey);
            json.Add(GcConstants.General.NAME,className);
            json.Add(GcConstants.General.DESC, description);
            json.Add(GcConstants.Classes.BASE_VALUES,baseValues);
            json.Add(GcConstants.Classes.BASIC_ATTRIBUTES,basicAttributes);
            json.Add(GcConstants.Classes.SKILLS,JToken.FromObject(skills));

            ClassTemplate result = ClassReader.FromJson(json);
            Assert.AreEqual(classKey,result.Key);
            Assert.AreEqual(className,result.Name);
            Assert.AreEqual(description,result.Description);
            Assert.AreEqual(baseAttack,result.BaseAttack.Key);



        }
    }
}
