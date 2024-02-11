using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Checker;
using FizzCode.DbTools.DataDefinition.Generic;
using FizzCode.DbTools.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzCode.DbTools.DataDefinition.Tests;
[TestClass]
public class SchemaCheckerTests
{
    [TestMethod]
    [LatestSqlVersions(true)]
    public void TableSingularName(SqlEngineVersion version)
    {
        var tcf = new TestContextFactory(null);

        var schemaChecker = new SchemaChecker(tcf.CreateContextWithLogger(version));

        var results = schemaChecker.Check(new TestDatabaseSchemaChecker_TableSingularName());

        Assert.AreEqual(1, results.Count);
        var result = results[0];

        Assert.AreEqual(nameof(TestDatabaseSchemaChecker_TableSingularName.Companies), result.ElementName);
        Assert.IsInstanceOfType<TableSingularNameConvention>(result);
        Assert.AreEqual("Table name should be singular", result.DisplayName);
    }

    [TestMethod]
    [LatestSqlVersions(true)]
    public void FkAndPkAreTheSame(SqlEngineVersion version)
    {
        var tcf = new TestContextFactory(null);

        var schemaChecker = new SchemaChecker(tcf.CreateContextWithLogger(version));

        var results = schemaChecker.Check(new TestDatabaseSchemaChecker_FkAndPkAreTheSame());

        Assert.AreEqual(1, results.Count);
        var result = results[0];

        Assert.AreEqual(nameof(TestDatabaseSchemaChecker_FkAndPkAreTheSame.Company), result.ElementName);
        Assert.IsInstanceOfType<FkAndPkAreTheSame>(result);
        Assert.AreEqual("Fk and Pk columns are the same", result.DisplayName);
    }
}

public class TestDatabaseSchemaChecker_TableSingularName : TestDatabaseDeclaration
{
    public SqlTable Companies { get; } = AddTable(table =>
    {
        table.AddInt32("Id").SetPK().SetIdentity();
        table.AddNVarChar("Name", 100);
    });
}

public class TestDatabaseSchemaChecker_FkAndPkAreTheSame : TestDatabaseDeclaration
{
    public SqlTable Company { get; } = AddTable(table =>
    {
        table.AddInt32("SelfId").SetPK().SetForeignKeyToColumn(nameof(Company), "SelfId");
        table.AddNVarChar("Name", 100);
    });
}

public class TestDatabaseSchemaChecker_SelfIdentity : TestDatabaseDeclaration
{
    public SqlTable Company { get; } = AddTable(table =>
    {
        table.AddInt32("SelfId").SetIdentity().SetForeignKeyToColumn(nameof(Company), "SelfId");
        table.AddNVarChar("Name", 100);
    });
}