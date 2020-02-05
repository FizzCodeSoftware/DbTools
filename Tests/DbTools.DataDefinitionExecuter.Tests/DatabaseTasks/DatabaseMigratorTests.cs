﻿namespace FizzCode.DbTools.DataDefinitionExecuter.Tests
{
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.DataDefinition.Migration;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.DataDefinitionGenerator;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DatabaseMigratorTests : DataDefinitionExecuterTests
    {
        [TestMethod]
        [LatestSqlVersions]
        public void NewTableTest(SqlVersion version)
        {
            _sqlExecuterTestAdapter.Check(version);
            var dd = new TestDatabaseSimple();
            _sqlExecuterTestAdapter.InitializeAndCreate(version.ToString(), dd);

            var context = new Context
            {
                Settings = TestHelper.GetDefaultTestSettings(version),
                Logger = TestHelper.CreateLogger()
            };

            var migrationGenerator = SqlGeneratorFactory.CreateMigrationGenerator(version, context);

            var executer = _sqlExecuterTestAdapter.GetExecuter(version.ToString());

            var databaseMigrator = new DatabaseMigrator(executer, migrationGenerator);
            var tableNew = new TableNew

            {
                SchemaAndTableName = "NewTableToMigrate"
            };
            ((SqlTable)tableNew).AddInt32("Id", false).SetPK().SetIdentity();

            new PrimaryKeyNamingDefaultStrategy().SetPrimaryKeyName(tableNew.Properties.OfType<PrimaryKey>().First());

            ((SqlTable)tableNew).AddNVarChar("Name", 100);

            dd.AddTable(tableNew);

            databaseMigrator.NewTable(tableNew);
        }
    }
}