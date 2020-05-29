namespace FizzCode.DbTools.DataDefinition.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ForeignKeyTests
    {
        [TestMethod]
        public void ToStringTestTestDatabaseFks()
        {
            var dd = new TestDatabaseFks();

            var child = dd.GetTable("Child");

            var fkToString = child.Properties.OfType<ForeignKey>().First().ToString();

            Assert.AreEqual("Child.ParentId -> Parent.Id", fkToString);

            var childChild = dd.GetTable("ChildChild");

            fkToString = childChild.Properties.OfType<ForeignKey>().First().ToString();

            Assert.AreEqual("ChildChild.ChildId -> Child.Id", fkToString);
        }

        [TestMethod]
        public void ToStringTestForeignKeyComposite()
        {
            var dd = new ForeignKeyComposite();

            var topOrdersPerCompany = dd.GetTable("TopOrdersPerCompany");
            var fks = topOrdersPerCompany.Properties.OfType<ForeignKey>().ToList();

            var fk1ToString = fks[0].ToString();
            var fk2ToString = fks[1].ToString();

            Assert.AreEqual("TopOrdersPerCompany.Top1A -> Order.OrderHeaderId, TopOrdersPerCompany.Top1B -> Order.LineNumber", fk1ToString);

            Assert.AreEqual("TopOrdersPerCompany.Top2A -> Order.OrderHeaderId, TopOrdersPerCompany.Top2B -> Order.LineNumber", fk2ToString);
        }

        [TestMethod]
        public void CheckCompositeFks1()
        {
            var tables = new ForeignKeyComposite().GetTables();
            CheckCompositeFks(tables);
        }

        [TestMethod]
        public void CheckCompositeFks2()
        {
            var tables = new ForeignKeyCompositeSetForeignKeyTo().GetTables();
            CheckCompositeFks(tables);
        }

        [TestMethod]
        public void CheckCompositeFks2Typed()
        {
            var tables = new ForeignKeyCompositeSetForeignKeyToTyped().GetTables();
            CheckCompositeFks(tables);
        }

        private static void CheckCompositeFks(List<SqlTable> tables)
        {
            Assert.AreEqual(4, tables.Count);

            var topOrdersPerCompany = tables.First(t => t.SchemaAndTableName.TableName == "TopOrdersPerCompany");
            var fks = topOrdersPerCompany.Properties.OfType<ForeignKey>().ToList();

            Assert.AreEqual(2, fks.Count);

            var top1AColumn = fks[0].ForeignKeyColumns.First(x0 => x0.ForeignKeyColumn.Name == "Top1A");
            var top1BColumn = fks[0].ForeignKeyColumns.First(x0 => x0.ForeignKeyColumn.Name == "Top1B");
            var top2AColumn = fks[1].ForeignKeyColumns.First(x0 => x0.ForeignKeyColumn.Name == "Top2A");
            var top2BColumn = fks[1].ForeignKeyColumns.First(x0 => x0.ForeignKeyColumn.Name == "Top2B");

            // TODO check that AA and AB vs BA and BB are in 2 different FKs
        }
    }
}