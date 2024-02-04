using System.Linq;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Tests;
using FizzCode.DbTools.TestBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzCode.DbTools.DataDefinitionReader.Tests;
[TestClass]
public class ForeignKeyToAnotherSchemaTest : DataDefinitionReaderTests
{
    [DataTestMethod]
    [LatestSqlVersions]
    public void ReadTables(SqlEngineVersion version)
    {
        Init(version, new ForeignKeyToAnotherSchema());

        TestHelper.CheckFeature(version, "ReadDdl");

        var db = ReadDd(version, new ForeignKeyToAnotherSchema().GetSchemaNames().ToList());

        var parent = db.GetTable("Parent", "Parent");

        var pkParent = parent.Properties.OfType<PrimaryKey>().FirstOrDefault();
        Assert.IsNotNull(pkParent);
        Assert.AreEqual(1, pkParent.SqlColumns.Count);
        Assert.AreEqual("Id", pkParent.SqlColumns[0].SqlColumn.Name);

        var child = db.GetTable("Child", "Child");

        var fks = child.Properties.OfType<ForeignKey>().ToList();
        Assert.AreEqual(1, fks.Count);

        var fk1 = fks[0];

        Assert.AreEqual(1, fk1.ForeignKeyColumns.Count);

        Assert.AreEqual(child.Columns["Parent.ParentId"], fk1.ForeignKeyColumns[0].ForeignKeyColumn);
        Assert.AreEqual(parent.Columns["Id"], fk1.ForeignKeyColumns[0].ReferredColumn);
        Assert.AreEqual(parent.SchemaAndTableName, fk1.ReferredTable.SchemaAndTableName);
    }
}