using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MicroExpressionParser;
namespace ParserTest
{
    [TestClass]
    public class DamageTypeTest
    {
        /*
        [TestMethod]
        public void FromJsonTest_Succes()
        {
            string json = "{\"StatKey\":\"P\",\"name\":\"Physical\",\"mitigation\":\"NONNEG(VALUE-DEF)\"}";
            DamageType expected = new DamageType() { Key = "P", Name="Physical", MitigationFormula= "NONNEG(VALUE-DEF)" };
            DamageType result = DamageType.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.Name, result.Name);
            Assert.AreEqual(expected.MitigationFormula, result.MitigationFormula);
        }

        [TestMethod]
        public void FromJsonTest_IncompleteJson_NoMitigationFormula()
        {
            string json = "{\"StatKey\":\"P\",\"name\":\"Physical\"}";
            DamageType expected = new DamageType() { Key = "P", Name = "Physical", MitigationFormula = null };
            DamageType result = DamageType.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.Name, result.Name);
            Assert.AreEqual(expected.MitigationFormula, result.MitigationFormula);
        }

        [TestMethod]
        public void FromJsonTest_IncompleteJson_NoName()
        {
            string json = "{\"StatKey\":\"P\",\"mitigation\":\"NONNEG(VALUE-DEF)\"}";
            DamageType expected = new DamageType() { Key = "P", Name = null, MitigationFormula = "NONNEG(VALUE-DEF)" };
            DamageType result = DamageType.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.Name, result.Name);
            Assert.AreEqual(expected.MitigationFormula, result.MitigationFormula);
        }

        [TestMethod]
        public void FromJsonTest_IncompleteJson_NoKey()
        {
            string json = "{\"name\":\"Physical\",\"mitigation\":\"NONNEG(VALUE-DEF)\"}";
            DamageType expected = new DamageType() { Key = null, Name = "Physical", MitigationFormula = "NONNEG(VALUE-DEF)" };
            DamageType result = DamageType.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.Name, result.Name);
            Assert.AreEqual(expected.MitigationFormula, result.MitigationFormula);
        }

        [TestMethod]
        public void FromJsonTest_EmptyJson()
        {
            string json = "";
            DamageType expected = null;
            DamageType result = DamageType.FromJson(json);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FromJsonTest_NullJson()
        {
            string json = null;
            DamageType expected = null;
            DamageType result = DamageType.FromJson(json);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FromJsonTest_InvalidJson()
        {
            string json = "Invalid";
            DamageType expected = null;
            DamageType result = DamageType.FromJson(json);
            Assert.AreEqual(expected, result);

        }
        */
    }
}
