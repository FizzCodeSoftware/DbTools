namespace FizzCode.DbTools.DataDefinition.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using FizzCode.DbTools.DataDefinition;

    [TestClass]
    public class SchemaAndTableNameTests
    {
        [TestMethod]
        public void CompareToTestSchema()
        {
            var schemaAndTableName1 = new SchemaAndTableName("A", "A");
            var schemaAndTableName2 = new SchemaAndTableName("B", "A");

            var result = schemaAndTableName1.CompareTo(schemaAndTableName2);
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void CompareToTestTable()
        {
            var schemaAndTableName1 = new SchemaAndTableName("A", "Z");
            var schemaAndTableName2 = new SchemaAndTableName("A", "A");

            var result = schemaAndTableName1.CompareTo(schemaAndTableName2);
            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public void CompareToTestEquality()
        {
            var schemaAndTableName1 = new SchemaAndTableName("A", "A");
            var schemaAndTableName2 = new SchemaAndTableName("A", "A");

            var result = schemaAndTableName1.CompareTo(schemaAndTableName2);
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void CompareToTestNoSchema()
        {
            var schemaAndTableName1 = new SchemaAndTableName("X");
            var schemaAndTableName2 = new SchemaAndTableName("A");

            var result = schemaAndTableName1.CompareTo(schemaAndTableName2);
            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public void EqualsTestFalse()
        {
            var schemaAndTableName1 = new SchemaAndTableName("A", "Z");
            var schemaAndTableName2 = new SchemaAndTableName("A", "A");

            Assert.IsFalse(schemaAndTableName1.Equals(schemaAndTableName2));
        }

        [TestMethod]
        public void EqualsTestTrue()
        {
            var schemaAndTableName1 = new SchemaAndTableName("A", "A");
            var schemaAndTableName2 = new SchemaAndTableName("A", "A");

            Assert.IsTrue(schemaAndTableName1.Equals(schemaAndTableName2));
        }

        [TestMethod]
        public void EqualsTestFalseNoSchema()
        {
            var schemaAndTableName1 = new SchemaAndTableName("Z");
            var schemaAndTableName2 = new SchemaAndTableName("A");

            Assert.IsFalse(schemaAndTableName1.Equals(schemaAndTableName2));
        }

        [TestMethod]
        public void EqualsTestTrueNoSchema()
        {
            var schemaAndTableName1 = new SchemaAndTableName("A");
            var schemaAndTableName2 = new SchemaAndTableName("A");

            Assert.IsTrue(schemaAndTableName1.Equals(schemaAndTableName2));
        }
    }
}
