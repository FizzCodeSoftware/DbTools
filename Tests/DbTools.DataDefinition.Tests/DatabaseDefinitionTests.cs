namespace FizzCode.DbTools.DataDefinition.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DatabaseDefinitionTests
    {
        [TestMethod]
        public void TestDatabaseSimple()
        {
            var tables = new TestDatabaseSimple().GetTables();
            Assert.AreEqual(1, tables.Count);
            Assert.AreEqual("Company", tables[0].SchemaAndTableName.TableName);
        }

        [TestMethod]
        public void TestDatabaseFks()
        {
            var tables = new TestDatabaseFks().GetTables();
            Assert.AreEqual(3, tables.Count);

            CheckFK(tables, "Child", "ParentId");
        }

        protected static void CheckFK(IList<SqlTable> tables, string childName, string fkColumnName)
        {
            var child = tables.First(t => t.SchemaAndTableName.TableName == childName);
            var fk = child.Properties.OfType<ForeignKey>().First();

            // TODO CheckFK if there are more Columns

            Assert.AreEqual(1, fk.ForeignKeyColumns.Count);
            Assert.AreEqual(fkColumnName, fk.ForeignKeyColumns[0].ForeignKeyColumn.Name);
        }

        [TestMethod]
        public void TestDatabaseCircular2FK()
        {
            var tables = new TestDatabaseCircular2FK().GetTables();
            Assert.AreEqual(2, tables.Count);

            CheckFK(tables, "A", "BId");
            CheckFK(tables, "B", "AId");
        }

        [TestMethod]
        public void TestDatabaseCircular3FK()
        {
            var tables = new TestDatabaseCircular3FK().GetTables();
            Assert.AreEqual(3, tables.Count);

            CheckFK(tables, "A", "BId");
            CheckFK(tables, "B", "CId");
            CheckFK(tables, "C", "AId");
        }
    }
}