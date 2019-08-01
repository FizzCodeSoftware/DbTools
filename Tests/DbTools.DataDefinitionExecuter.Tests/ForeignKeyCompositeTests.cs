namespace FizzCode.DbTools.DataDefinitionExecuter.Tests
{
    using System.Configuration;
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ForeignKeyCompositeTests
    {
        [TestMethod]
        public void GenerateForeignKeyCompositeTestDatabase()
        {
            if (!Helper.ShouldForceIntegrationTests())
                Assert.Inconclusive("Test is skipped, integration tests are not running.");

            var connectionStringSettings = ConfigurationManager.ConnectionStrings["MsSql"];

            var generateForeignKeyCompositeTestDatabase = DatabaseCreator.FromConnectionStringSettings(new ForeignKeyCompositeTestsDb(), connectionStringSettings);
            generateForeignKeyCompositeTestDatabase.ReCreateDatabase(true);
        }

        [TestMethod]
        public void CheckCompositeFks()
        {
            var tables = new ForeignKeyCompositeTestsDb().GetTables();
            Assert.AreEqual(4, tables.Count);

            var topOrdersPerCompany = tables.First(t => t.Name == "TopOrdersPerCompany");
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
