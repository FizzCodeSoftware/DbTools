namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class TestDatabaseInvalidFK : TestDatabaseDeclaration
    {
        public SqlTable Primary { get; } = AddTable(table =>
          {
              table.AddInt32("Id1").SetPK();
              table.AddInt32("Id2").SetPK();
          });

        public SqlTable Foreign { get; } = AddTable(table =>
          {
              table.AddInt32("Id").SetPK().SetIdentity();
              table.AddInt32("PrimaryId").SetForeignKeyTo(nameof(Primary));
          });
    }

    [TestClass]
    public class DatabaseDefinitionInvalidFKTest
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidForeignKeyRegistrationException))]
        public void InvalidFK()
        {
            var _ = new TestDatabaseInvalidFK();
        }
    }
}
