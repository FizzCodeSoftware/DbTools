using System.Globalization;
using FizzCode.DbTools.DataDefinition;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Generic;
using FizzCode.DbTools.DataDefinition.Tests;
using FizzCode.DbTools.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable CA1034 // Nested types should not be visible
namespace FizzCode.DbTools.SqlExecuter.Tests;
[TestClass]
public class GenerateDatabaseTests : GenerateDatabaseTestsBase
{
    [TestMethod]
    [LatestSqlVersions]
    public void GenerateTestDatabaseSimple(SqlEngineVersion version)
    {
        GenerateDatabase(new TestDatabaseSimple(), version);
    }

    [TestMethod]
    [LatestSqlVersions]
    public void GenerateTestDatabaseSimpleTyped(SqlEngineVersion version)
    {
        GenerateDatabase(new TestDatabaseSimpleTyped(), version);
    }

    [TestMethod]
    [LatestSqlVersions]
    public void GenerateTestDatabaseFks(SqlEngineVersion version)
    {
        GenerateDatabase(new TestDatabaseFks(), version);
    }

    [TestMethod]
    [LatestSqlVersions]
    public void GenerateTestDatabaseFksTyped(SqlEngineVersion version)
    {
        GenerateDatabase(new TestDatabaseFksTyped(), version);
    }

    [TestMethod]
    [LatestSqlVersions]
    public void GenerateForeignKeyCompositeTestDatabase(SqlEngineVersion version)
    {
        GenerateDatabase(new ForeignKeyComposite(), version);
    }

    [TestMethod]
    [LatestSqlVersions]
    public void GenerateDatabase_Index(SqlEngineVersion version)
    {
        GenerateDatabase(new TestDatabaseIndex(), version);
    }

    [TestMethod]
    [LatestSqlVersions]
    public void GenerateDatabase_IndexTyped(SqlEngineVersion version)
    {
        GenerateDatabase(new IndexTyped(), version);
    }

    [TestMethod]
    public void GenerateDatabase_TableDescription()
    {
        GenerateDatabase(new TableDescription(), MsSqlVersion.MsSql2016);
    }

    [TestMethod]
    public void GenerateDatabase_ColumnDescription()
    {
        GenerateDatabase(new ColumnDescription(), MsSqlVersion.MsSql2016);
    }

    public class TestDatabaseIndex : TestDatabaseDeclaration
    {
        public SqlTable Table { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
            table.AddIndex("Name");
            table.AddIndex("Id", "Name");
        });
    }

    public class IndexTyped : TestDatabaseDeclaration
    {
        public Table_ Table { get; } = new Table_();

        public class Table_ : SqlTable
        {
            public SqlColumn Id { get; } = Generic1.AddInt32().SetPK().SetIdentity();
            public SqlColumn Name { get; } = Generic1.AddNVarChar(100);
#pragma warning disable IDE1006 // Naming Styles
            public Index _i1 { get; } = Generic1.AddIndex(nameof(Name));
            public Index _i2 { get; } = Generic1.AddIndex(nameof(Id), nameof(Name));
#pragma warning restore IDE1006 // Naming Styles
        }
    }

    public class TableDescription : TestDatabaseDeclaration
    {
        public SqlTable Table { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
            table.AddDescription("Table description");
        });
    }

    public class ColumnDescription : TestDatabaseDeclaration
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
        GenerateDatabase(new DefaultValue(), MsSqlVersion.MsSql2016);
    }

    public class DefaultValue : TestDatabaseDeclaration
    {
        public SqlTable Table { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100).AddDefaultValue("'apple'");
            table.AddDateTime("DateTime").AddDefaultValue("'" + new System.DateTime(2019, 8, 7, 13, 59, 57, 357).ToString("yyyy-M-d HH:mm:ss.fff", CultureInfo.InvariantCulture) + "'");
        });
    }

    [TestMethod]
    [LatestSqlVersions]
    public void DatabaseDefinitionWithSchemaTableNameSeparator(SqlEngineVersion version)
    {
        TestHelper.CheckFeature(version, "Schema");
        GenerateDatabase(new SchemaTableNameSeparator(), version);
    }

    [TestMethod]
    [LatestSqlVersions]
    public void DatabaseDefinitionWithSchemaAndDefaultSchema(SqlEngineVersion version)
    {
        TestHelper.CheckFeature(version, "Schema");
        GenerateDatabase(new SchemaTableNameDefaultSchema(), version);
    }

    public class SchemaTableNameSeparator : TestDatabaseDeclaration
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
    public void UniqueConstratintAsFkNoPk(SqlEngineVersion version)
    {
        if (version is SqLiteVersion)
            return;
        // TODO SqLite - You can't add a constraint to existing table in SQLite - should work at create table time
        GenerateDatabase(new DbUniqueConstratintAsFkNoPk(), version);
    }

    [TestMethod]
    [LatestSqlVersions]
    public void UniqueConstratintAsFkNoPkTyped(SqlEngineVersion version)
    {
        if (version is SqLiteVersion)
            return;
        // TODO SqLite - You can't add a constraint to existing table in SQLite - should work at create table time
        GenerateDatabase(new DbUniqueConstratintAsFkNoPkTyped(), version);
    }

    public class DbUniqueConstratintAsFkNoPk : TestDatabaseDeclaration
    {
        public SqlTable Primary { get; } = AddTable(table =>
        {
            table.AddInt32("Id");
            table.AddNVarChar("Name", 100);
            table.AddUniqueConstraint("Id");
        });

        public SqlTable Foreign { get; } = AddTable(table =>
        {
            table.AddInt32("Id");
            table.AddInt32("PrimaryId").SetForeignKeyToTable(nameof(Primary));
        });
    }

    public class DbUniqueConstratintAsFkNoPkTyped : TestDatabaseDeclaration
    {
        public Primary_ Primary { get; } = new Primary_();
        public Foreign_ Foreign { get; } = new Foreign_();

        public class Primary_ : SqlTable
        {
            public SqlColumn Id { get; } = Generic1.AddInt32();
            public SqlColumn Name { get; } = Generic1.AddNVarChar(100);

#pragma warning disable IDE1006 // Naming Styles
            public UniqueConstraint _uc1 { get; } = Generic1.AddUniqueConstraint(nameof(Id));
#pragma warning restore IDE1006 // Naming Styles
        }

        public class Foreign_ : SqlTable
        {
            public SqlColumn Id { get; } = Generic1.AddInt32();
            public SqlColumn PrimaryId { get; } = Generic1.SetForeignKeyTo(nameof(Primary));
        }
    }

    [TestMethod]
    [LatestSqlVersions]
    public void UniqueConstratintAsFk(SqlEngineVersion version)
    {
        if (version is SqLiteVersion)
            return;
        // TODO SqLite - You can't add a constraint to existing table in SQLite - should work at create table time
        GenerateDatabase(new DbUniqueConstratintAsFk(), version);
    }

    [TestMethod]
    [LatestSqlVersions]
    public void UniqueIndexAsFk(SqlEngineVersion version)
    {
        if (version is OracleVersion)
            return;

        // TODO Unique index by default is not acceptable as reference for FK in Oracle
        GenerateDatabase(new DbUniqueIndexAsFk(), version);
    }

    public class DbUniqueIndexAsFk : TestDatabaseDeclaration
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
            table.AddInt32("PrimaryId").SetForeignKeyToTable(nameof(Primary));
        });
    }
}