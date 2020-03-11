namespace FizzCode.DbTools.DataDefinition.Tests
{
    using System.Linq;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition.MsSql2016;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class TestDatabaseFkNoCheckTest : TestDatabaseDeclaration
    {
        public SqlTable Primary { get; } = AddTable(table =>
          {
              table.AddInt("Id").SetPK();
          });

        public SqlTable Foreign { get; } = AddTable(table =>
          {
              table.AddInt("Id").SetPK().SetIdentity();
              table.AddInt("PrimaryId").SetForeignKeyTo(nameof(Primary), new SqlEngineVersionSpecificProperty(MsSqlVersion.MsSql2016, "Nocheck", "true"));
          });
    }

    [TestClass]
    public class DatabaseDefinitionFkNoCheckTest
    {
        [TestMethod]
        public void FkNoCheckTest()
        {
            var dd_ = new TestDatabaseFkNoCheckTest();

            var property = dd_.GetTable("Foreign").Properties.OfType<ForeignKey>().First().SqlEngineVersionSpecificProperties.First();

            Assert.AreEqual(MsSqlVersion.MsSql2016, property.Version);
            Assert.AreEqual("Nocheck", property.Name);
            Assert.AreEqual("true", property.Value);
        }
    }
}
