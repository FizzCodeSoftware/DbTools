#pragma warning disable CA1034 // Nested types should not be visible

namespace FizzCode.DbTools.DataDefinitionExecuter.Tests
{
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
            GenerateDatabase(new TableDescription(), SqlVersions.MsSql2016);
        }

        [TestMethod]
        public void GenerateDatabase_ColumnDescription()
        {
            GenerateDatabase(new ColumnDescription(), SqlVersions.MsSql2016);
        }

        public class Index : DatabaseDeclaration
        {
            public SqlTable Table { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100);
                table.AddIndex("Name");
                table.AddIndex( "Id", "Name");
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
            GenerateDatabase(new DefaultValue(), SqlVersions.MsSql2016);
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
        [LatestSqlVersions]
        public void DatabaseDefinitionWithSchemaTableNameSeparator(SqlVersion version)
        {
            TestHelper.CheckFeature(version, "Schema");
            GenerateDatabase(new SchemaTableNameSeparator(), version);
        }

        [TestMethod]
        [LatestSqlVersions]
        public void DatabaseDefinitionWithSchemaAndDefaultSchema(SqlVersion version)
        {
            TestHelper.CheckFeature(version, "Schema");
            GenerateDatabase(new SchemaTableNameDefaultSchema(), version);
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

        [TestMethod]
        [LatestSqlVersions]
        public void UniqueConstratintAsFk(SqlVersion version)
        {
            if (version is ISqLiteDialect)
                return;
            // TODO SqLite - You can't add a constraint to existing table in SQLite - should work at create table time
            GenerateDatabase(new DbUniqueConstratintAsFk(), version);
        }

        public class DbUniqueConstratintAsFk : DatabaseDeclaration
        {
            public SqlTable Primary { get; } = AddTable(table =>
            {
                table.AddInt32("Id");
                table.AddNVarChar("Name", 100);
                table.AddUniqueConstraint("Id");
            });

            public SqlTable Foreign { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddInt32("PrimaryId").SetForeignKeyTo(nameof(Primary));
            });
        }

        [TestMethod]
        [LatestSqlVersions]
        public void UniqueIndexAsFk(SqlVersion version)
        {
            if (version is IOracleDialect)
                return;

            // TODO Unique index by default is not acceptable as reference for FK in Oracle
            GenerateDatabase(new DbUniqueIndexAsFk(), version);
        }

        public class DbUniqueIndexAsFk : DatabaseDeclaration
        {
            public SqlTable Primary { get; } = AddTable(table =>
            {
                table.AddInt32("Id");
                table.AddNVarChar("Name", 100);
                table.AddIndex(true, "Id");
            });

            public SqlTable Foreign { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddInt32("PrimaryId").SetForeignKeyTo(nameof(Primary));
            });
        }
    }
}