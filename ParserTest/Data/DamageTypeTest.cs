using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPGEngine;
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
            DamageTypeTemplate expected = new DamageTypeTemplate() { Key = "P", Key="Physical", MitigationFormula= "NONNEG(VALUE-DEF)" };
            DamageTypeTemplate result = DamageTypeTemplate.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.MitigationFormula, result.MitigationFormula);
        }

        [TestMethod]
        public void FromJsonTest_IncompleteJson_NoMitigationFormula()
        {
            string json = "{\"StatKey\":\"P\",\"name\":\"Physical\"}";
            DamageTypeTemplate expected = new DamageTypeTemplate() { Key = "P", Key = "Physical", MitigationFormula = null };
            DamageTypeTemplate result = DamageTypeTemplate.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.MitigationFormula, result.MitigationFormula);
        }

        [TestMethod]
        public void FromJsonTest_IncompleteJson_NoName()
        {
            string json = "{\"StatKey\":\"P\",\"mitigation\":\"NONNEG(VALUE-DEF)\"}";
            DamageTypeTemplate expected = new DamageTypeTemplate() { Key = "P", Key = null, MitigationFormula = "NONNEG(VALUE-DEF)" };
            DamageTypeTemplate result = DamageTypeTemplate.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.MitigationFormula, result.MitigationFormula);
        }

        [TestMethod]
        public void FromJsonTest_IncompleteJson_NoKey()
        {
            string json = "{\"name\":\"Physical\",\"mitigation\":\"NONNEG(VALUE-DEF)\"}";
            DamageTypeTemplate expected = new DamageTypeTemplate() { Key = null, Key = "Physical", MitigationFormula = "NONNEG(VALUE-DEF)" };
            DamageTypeTemplate result = DamageTypeTemplate.FromJson(json);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.Key, result.Key);
            Assert.AreEqual(expected.MitigationFormula, result.MitigationFormula);
        }

        [TestMethod]
        public void FromJsonTest_EmptyJson()
        {
            string json = "";
            DamageTypeTemplate expected = null;
            DamageTypeTemplate result = DamageTypeTemplate.FromJson(json);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FromJsonTest_NullJson()
        {
            string json = null;
            DamageTypeTemplate expected = null;
            DamageTypeTemplate result = DamageTypeTemplate.FromJson(json);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FromJsonTest_InvalidJson()
        {
            string json = "Invalid";
            DamageTypeTemplate expected = null;
            DamageTypeTemplate result = DamageTypeTemplate.FromJson(json);
            Assert.AreEqual(expected, result);

        }
        */
    }
}
