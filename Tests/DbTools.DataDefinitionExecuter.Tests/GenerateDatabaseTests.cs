namespace FizzCode.DbTools.DataDefinitionExecuter.Tests
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GenerateDatabaseTests : DataDefinitionExecuterTests
    {
        [TestMethod]
        [SqlDialects]
        public void GenerateTestDatabaseSimple(SqlDialect sqlDialect)
        {
            GenerateDatabase(new TestDatabaseSimple(), sqlDialect);
        }

        [TestMethod]
        [SqlDialects]
        public void GenerateForeignKeyCompositeTestDatabase(SqlDialect sqlDialect)
        {
            GenerateDatabase(new ForeignKeyCompositeTestsDb(), sqlDialect);
        }

        public static void GenerateDatabase(DatabaseDefinition dd, SqlDialect sqlDialect)
        {
            _sqlExecuterTestAdapter.Check(sqlDialect);
            _sqlExecuterTestAdapter.Initialize(sqlDialect.ToString(), dd);

            var databaseCreator = new DatabaseCreator(dd, _sqlExecuterTestAdapter.GetExecuter(sqlDialect.ToString()));

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
        [SqlDialects]
        public void GenerateDatabase_Index(SqlDialect sqlDialect)
        {
            GenerateDatabase(new IndexTestDb(), sqlDialect);
        }

        [TestMethod]
        public void GenerateDatabase_TableDescription()
        {
            GenerateDatabase(new TableDescriptionTestDb(), SqlDialect.MsSql);
        }

        [TestMethod]
        public void GenerateDatabase_ColumnDescription()
        {
            GenerateDatabase(new ColumnDescriptionTestDb(), SqlDialect.MsSql);
        }

        public class IndexTestDb : DatabaseDeclaration
        {
            public SqlTable Table { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100);
                table.AddIndex("Name");
                table.AddIndex("Id", "Name");
            });
        }

        public class TableDescriptionTestDb : DatabaseDeclaration
        {
            public SqlTable Table { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100);
                table.AddDescription("Table description");
            });
        }

        public class ColumnDescriptionTestDb : DatabaseDeclaration
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
            GenerateDatabase(new DefaultValueTestDb(), SqlDialect.MsSql);
        }

        public class DefaultValueTestDb : DatabaseDeclaration
        {
            public SqlTable Table { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100).AddDefaultValue("'apple'");
                table.AddDateTime("DateTime").AddDefaultValue("'" + new DateTime(2019, 8, 7, 13, 59, 57, 357).ToString("yyyy-M-d HH:mm:ss.fff") + "'");
            });
        }

        [TestMethod]
        public void DatabaseDefinitionWithSchemaTableNameSeparator()
        {
            GenerateDatabase(new SchemaTableNameSeparatorTestDb(), SqlDialect.MsSql);
        }

        [TestMethod]
        public void DatabaseDefinitionWithSchemaAndDefaultSchema()
        {
            GenerateDatabase(new SchemaTableNameDefaultSchemaTestDb(), SqlDialect.MsSql);
        }

        public class SchemaTableNameSeparatorTestDb : DatabaseDeclaration
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

        public class SchemaTableNameDefaultSchemaTestDb : SchemaTableNameSeparatorTestDb
        {
            public SqlTable Table { get; } = AddTable(table =>
            {
                table.AddInt32("Id").SetPK().SetIdentity();
                table.AddNVarChar("Name", 100);
            });
        }
    }
}