namespace FizzCode.DbTools.DataDefinitionExecuter.Tests
{
    using System;
    using System.Configuration;
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.DataDefinitionGenerator;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GenerateDatabaseTests
    {
        [TestMethod]
        public void GenerateForeignKeyCompositeTestDatabase()
        {
            GenerateDatabase(new ForeignKeyCompositeTestsDb(), "MsSql");
        }

        public void GenerateDatabase(DatabaseDefinition dd, string connectionStringKey, bool isIntegrationTest = true)
        {
            if (isIntegrationTest && !Helper.ShouldForceIntegrationTests())
                Assert.Inconclusive("Test is skipped, integration tests are not running.");

            var connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringKey];

            var generateForeignKeyCompositeTestDatabase = DatabaseCreator.FromConnectionStringSettings(dd, connectionStringSettings);
            try
            {
                generateForeignKeyCompositeTestDatabase.ReCreateDatabase(true);
            }
            finally
            { 
                var generator = SqlGeneratorFactory.CreateGenerator(SqlDialectHelper.GetSqlDialectFromConnectionStringSettings(connectionStringSettings));
                var executer = SqlExecuterFactory.CreateSqlExecuter(connectionStringSettings, generator);
                executer.DropDatabaseIfExists();
            }
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

        [TestMethod]
        public void GenerateDatabase_Index()
        {
            GenerateDatabase(new IndexTestDb(), "MsSql");
        }

        [TestMethod]
        public void GenerateDatabase_TableDescription()
        {
            GenerateDatabase(new TableDescriptionTestDb(), "MsSql");
        }

        [TestMethod]
        public void GenerateDatabase_ColumnDescription()
        {
            GenerateDatabase(new ColumnDescriptionTestDb(), "MsSql");
        }

        public class IndexTestDb : DatabaseDeclaration
        {
            public static LazySqlTable Table = new LazySqlTable(() =>
            {
                var table = new SqlTableDeclaration();
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100);
                table.AddIndex("Name");
                table.AddIndex("Id", "Name");
                return table;
            });
        }

        public class TableDescriptionTestDb : DatabaseDeclaration
        {
            public static LazySqlTable Table = new LazySqlTable(() =>
            {
                var table = new SqlTableDeclaration();
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100);
                table.AddDescription("Table description");
                return table;
            });
        }

        public class ColumnDescriptionTestDb : DatabaseDeclaration
        {
            public static LazySqlTable Table = new LazySqlTable(() =>
            {
                var table = new SqlTableDeclaration();
                table.AddInt32("Id").SetPK().SetIdentity().AddDescription("Id Column description");
                table.AddNVarChar("Name", 100).AddDescription("Name Column description");
                return table;
            });
        }

        [TestMethod]
        public void GenerateDatabase_DefaultValue()
        {
            GenerateDatabase(new DefaultValueTestDb(), "MsSql");
        }

        public class DefaultValueTestDb : DatabaseDeclaration
        {
            public static LazySqlTable Table = new LazySqlTable(() =>
            {
                var table = new SqlTableDeclaration();
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100).AddDefaultValue("'apple'");
                table.AddDateTime("DateTime").AddDefaultValue("'" + new DateTime(2019,8,7,13,59,57,357).ToString("yyyy-M-d HH:mm:ss.fff") + "'");
                return table;
            });
        }
    }
}
