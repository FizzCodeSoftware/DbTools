namespace FizzCode.DbTools.DataDefinition.Tests
{
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
        public void ToStringTestForeignKeyCompositeTestsDb()
        {
            var dd = new ForeignKeyCompositeTestsDb();

            var topOrdersPerCompany = dd.GetTable("TopOrdersPerCompany");
            var fks = topOrdersPerCompany.Properties.OfType<ForeignKey>().ToList();

            var fk1ToString = fks[0].ToString();
            var fk2ToString = fks[1].ToString();

            Assert.AreEqual("TopOrdersPerCompany.Top1A -> Order.OrderHeaderId, TopOrdersPerCompany.Top1B -> Order.LineNumber", fk1ToString);

            Assert.AreEqual("TopOrdersPerCompany.Top2A -> Order.OrderHeaderId, TopOrdersPerCompany.Top2B -> Order.LineNumber", fk2ToString);
        }
    }
}