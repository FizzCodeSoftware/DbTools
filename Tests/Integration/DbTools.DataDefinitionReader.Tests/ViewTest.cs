namespace FizzCode.DbTools.DataDefinitionReader.Tests
{
    using System.Linq;
    using FizzCode.DbTools.DataDefinition.Tests;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ViewTest : DataDefinitionReaderTests
    {
        [TestMethod]
        [LatestSqlVersions]
        public void ViewSimple(SqlEngineVersion version)
        {
            Init(version, new TestDatabaseSimpleWithView());

            TestHelper.CheckFeature(version, "ReadDdl");

            var dd = ReadDd(version, new TestDatabaseSimpleWithView().GetSchemaNames());

            var views = dd.GetViews();
            var viewCompanyView = views.Single(v => v.SchemaAndTableName.TableName == "CompanyView");

            Assert.AreEqual(2, viewCompanyView.Columns.Count);

            var idColumn = viewCompanyView.Columns.First();
            var nameColumn = viewCompanyView.Columns.Skip(1).First();

            Assert.AreEqual("Id", idColumn.Name);
            // Assert.IsInstanceOfType(idColumn.Type.SqlTypeInfo, typeof(DataDefinition.MsSql2016.SqlInt));
            Assert.AreEqual("Name", nameColumn.Name);

        }
    }
}
