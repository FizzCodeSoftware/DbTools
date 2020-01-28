namespace FizzCode.DbTools.DataDefinitionReader.Tests
{
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ForeignKeyToAnotherSchemaTest : DataDefinitionReaderTests
    {
        [DataTestMethod]
        [LatestSqlVersions]
        public void CreateTables(SqlVersion version)
        {
            var dd = new ForeignKeyToAnotherSchema();
            Init(version, dd);

            var creator = new DatabaseCreator(dd, _sqlExecuterTestAdapter.GetExecuter(version.ToString()));
            creator.ReCreateDatabase(true);
        }

        [DataTestMethod]
        [LatestSqlVersions]
        public void ReadTables(SqlVersion version)
        {
            Init(version, null);

            TestHelper.CheckFeature(version, "ReadDdl");

            var ddlReader = DataDefinitionReaderFactory.CreateDataDefinitionReader(_sqlExecuterTestAdapter.ConnectionStrings[version.ToString()], _sqlExecuterTestAdapter.GetContext(version));
            var db = ddlReader.GetDatabaseDefinition();

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
            Assert.AreEqual(parent, fk1.ReferredTable.SchemaAndTableName);
        }
    }
}