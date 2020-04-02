namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class InvalidFK_SingleFkToMultiPk : TestDatabaseDeclaration
    {
        public SqlTable Primary { get; } = AddTable(table =>
          {
              table.AddInt32("Id1").SetPK();
              table.AddInt32("Id2").SetPK();
          });

        public SqlTable Foreign { get; } = AddTable(table =>
          {
              table.AddInt32("Id").SetPK().SetIdentity();
              table.AddInt32("PrimaryId").SetForeignKeyToTable(nameof(Primary));
          });
    }

    public class InvalidFK_SingleFkToNoUk : TestDatabaseDeclaration
    {
        public SqlTable Primary { get; } = AddTable(table =>
        {
            table.AddInt32("Id1");
        });

        public SqlTable Foreign { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddInt32("PrimaryId").SetForeignKeyToTable(nameof(Primary));
        });
    }

    public class InvalidFK_SingleFkToPkAndUc : TestDatabaseDeclaration
    {
        public SqlTable Primary { get; } = AddTable(table =>
        {
            table.AddInt32("Id1").SetPK();
            table.AddInt32("UniqueId");
            table.AddUniqueConstraint("UniqueId");

        });

        public SqlTable Foreign { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddInt32("PrimaryId").SetForeignKeyToTable(nameof(Primary));
        });
    }

    [TestClass]
    public class DatabaseDefinitionInvalidFKTest
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidForeignKeyRegistrationException))]
        public void InvalidFK_SingleFkToMultiPk()
        {
            var _ = new InvalidFK_SingleFkToMultiPk();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidForeignKeyRegistrationException))]
        public void InvalidFK_SingleFkToNoUk()
        {
            var _ = new InvalidFK_SingleFkToNoUk();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidForeignKeyRegistrationException))]
        public void InvalidFK_SingleFkToPkAndUc()
        {
            var _ = new InvalidFK_SingleFkToPkAndUc();
        }
    }

}
