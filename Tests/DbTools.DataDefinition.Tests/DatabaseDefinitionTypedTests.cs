namespace FizzCode.DbTools.DataDefinition.Tests
{
    using System.Linq;
    using FizzCode.DbTools.DataDefinition.Base;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DatabaseDefinitionTypedTests
    {
        [TestMethod]
        public void CompositePk()
        {
            var db = new ForeignKeyCompositeSetForeignKeyToTyped();
            var order = db.GetTable("Order");

            var pk = order.Properties.OfType<PrimaryKey>().First();

            Assert.AreEqual(2, pk.SqlColumns.Count);
            Assert.AreEqual("OrderHeaderId", pk.SqlColumns[0].SqlColumn.Name);
            Assert.AreEqual("LineNumber", pk.SqlColumns[1].SqlColumn.Name);
        }
    }
}