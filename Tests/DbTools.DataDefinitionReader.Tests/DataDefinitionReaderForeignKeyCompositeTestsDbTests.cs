namespace FizzCode.DbTools.DataDefinitionReader.Tests
{
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DataDefinitionReaderForeignKeyCompositeTestsDbTests : DataDefinitionReaderTests
    {
        [DataTestMethod]
        [SqlDialects]
        public void CreateTables(SqlDialect sqlDialect)
        {
            var dd = new ForeignKeyCompositeTestsDb();
            _sqlExecuterTestAdapter.Check(sqlDialect);
            _sqlExecuterTestAdapter.Initialize(sqlDialect.ToString(), dd);

            var creator = new DatabaseCreator(dd, _sqlExecuterTestAdapter.GetExecuter(sqlDialect.ToString()));
            creator.ReCreateDatabase(true);
        }

        [DataTestMethod]
        [SqlDialects]
        public void ReadTables(SqlDialect sqlDialect)
        {
            TestHelper.CheckFeature(sqlDialect, "ReadDdl");
            TestHelper.CheckProvider(sqlDialect, _sqlExecuterTestAdapter.ConnectionStrings.All);

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(sqlDialect, _sqlExecuterTestAdapter.GetExecuter(sqlDialect.ToString()));
            var db = ddlReader.GetDatabaseDefinition();

            // TODO db.GetTable("Company") - use default schema
            var company = db.GetTable("Company");

            var pkCompany = company.Properties.OfType<PrimaryKey>().FirstOrDefault();
            Assert.IsNotNull(pkCompany);
            Assert.AreEqual(1, pkCompany.SqlColumns.Count);
            Assert.AreEqual("Id", pkCompany.SqlColumns[0].SqlColumn.Name);

            var topOrdersPerCompany = db.GetTable("dbo", "TopOrdersPerCompany");

            var fks = topOrdersPerCompany.Properties.OfType<ForeignKey>().ToList();
            Assert.AreEqual(2, fks.Count);

            var fk1 = fks[0];
            var fk2 = fks[1];

            Assert.AreEqual(2, fk1.ForeignKeyColumns.Count);
            Assert.AreEqual(2, fk2.ForeignKeyColumns.Count);

            var order = db.GetTable("dbo", "Order");

            Assert.AreEqual(topOrdersPerCompany.Columns["Top1A"], fk1.ForeignKeyColumns[0].ForeignKeyColumn);
            Assert.AreEqual(topOrdersPerCompany.Columns["Top1B"], fk1.ForeignKeyColumns[1].ForeignKeyColumn);

            Assert.AreEqual(order.Columns["OrderHeaderId"], fk1.ForeignKeyColumns[0].ReferredColumn);
            Assert.AreEqual(order.Columns["LineNumber"], fk1.ForeignKeyColumns[1].ReferredColumn);

            Assert.AreEqual(topOrdersPerCompany.Columns["Top2A"], fk2.ForeignKeyColumns[0].ForeignKeyColumn);
            Assert.AreEqual(topOrdersPerCompany.Columns["Top2B"], fk2.ForeignKeyColumns[1].ForeignKeyColumn);

            Assert.AreEqual(order.Columns["OrderHeaderId"], fk2.ForeignKeyColumns[0].ReferredColumn);
            Assert.AreEqual(order.Columns["LineNumber"], fk2.ForeignKeyColumns[1].ReferredColumn);
        }
    }
}