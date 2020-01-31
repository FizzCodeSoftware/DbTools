namespace FizzCode.DbTools.DataDefinitionExecuter.Tests
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GenerateDatabaseTests : DataDefinitionExecuterTests
    {
        [TestMethod]
        [LatestSqlVersions]
        public void GenerateTestDatabaseSimple(SqlVersion version)
        {
            GenerateDatabase(new TestDatabaseSimple(), version);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GenerateForeignKeyCompositeTestDatabase(SqlVersion version)
        {
            GenerateDatabase(new ForeignKeyComposite(), version);
        }

        public static void GenerateDatabase(DatabaseDefinition dd, SqlVersion version)
        {
            _sqlExecuterTestAdapter.Check(version);
            _sqlExecuterTestAdapter.Initialize(version.ToString(), dd);

            var databaseCreator = new DatabaseCreator(dd, _sqlExecuterTestAdapter.GetExecuter(version.ToString()));

            try
            {
                databaseCreator.ReCreateDatabase(true);
            }
            finally
            {
                databaseCreator.CleanupDatabase();
            }
        }

        [TestMethod]
        [LatestSqlVersions]
        public void GenerateDatabase_Index(SqlVersion version)
        {
            GenerateDatabase(new Index(), version);
        }

        [TestMethod]
        public void GenerateDatabase_TableDescription()
        {
            GenerateDatabase(new TableDescription(), new MsSql2016());
        }

        [TestMethod]
        public void GenerateDatabase_ColumnDescription()
        {
            GenerateDatabase(new ColumnDescription(), new MsSql2016());
        }

        public class Index : DatabaseDeclaration
        {
            public SqlTable Table { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100);
                table.AddIndex("Name");
                table.AddIndex("Id", "Name");
            });
        }

        public class TableDescription : DatabaseDeclaration
        {
            public SqlTable Table { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100);
                table.AddDescription("Table description");
            });
        }

        public class ColumnDescription : DatabaseDeclaration
        {
            public SqlTable Table { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity().AddDescription("Id Column description");
                table.AddNVarChar("Name", 100).AddDescription("Name Column description");
            });
        }

        [TestMethod]
        public void GenerateDatabase_DefaultValue()
        {
            GenerateDatabase(new DefaultValue(), new MsSql2016());
        }

        public class DefaultValue : DatabaseDeclaration
        {
            public SqlTable Table { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100).AddDefaultValue("'apple'");
                table.AddDateTime("DateTime").AddDefaultValue("'" + new System.DateTime(2019, 8, 7, 13, 59, 57, 357).ToString("yyyy-M-d HH:mm:ss.fff") + "'");
            });
        }

        [TestMethod]
        [SqlVersions(typeof(MsSql2016), typeof(Oracle12c))]
        public void DatabaseDefinitionWithSchemaTableNameSeparator(SqlVersion version)
        {
            GenerateDatabase(new SchemaTableNameSeparator(), version);
        }

        [TestMethod]
        public void DatabaseDefinitionWithSchemaAndDefaultSchema()
        {
            GenerateDatabase(new SchemaTableNameDefaultSchema(), new MsSql2016());
        }

        public class SchemaTableNameSeparator : DatabaseDeclaration
        {
            public SqlTable SchemaAꜗTable { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100);
            });

            public SqlTable SchemaBꜗTable { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100);
                table.AddForeignKey(nameof(SchemaAꜗTable));
            });
        }

        public class SchemaTableNameDefaultSchema : SchemaTableNameSeparator
        {
            public SqlTable Table { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100);
            });
        }
    }
}