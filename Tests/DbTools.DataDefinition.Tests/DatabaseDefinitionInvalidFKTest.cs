namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.TestBase;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class InvalidFK_SingleSetFkToMultiPk : TestDatabaseDeclaration
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

    public class InvalidFK_SingleAddFkToMultiPk : TestDatabaseDeclaration
    {
        public SqlTable Primary { get; } = AddTable(table =>
        {
            table.AddInt32("Id1").SetPK();
            table.AddInt32("Id2").SetPK();
        });

        public SqlTable Foreign { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddForeignKey(nameof(Primary), "PrimaryId");
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

    public class InvalidFK_UniqueConstratintAsFk_SingleToMulti : TestDatabaseDeclaration
    {
        public SqlTable Primary { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddInt32("UniqueId1");
            table.AddInt32("UniqueId2");
            table.AddUniqueConstraint("UniqueId1", "UniqueId2");
        });

        public SqlTable Foreign { get; } = AddTable(table =>
        {
            table.AddInt32("Id");
            table.AddInt32("PrimaryId").SetForeignKeyToColumn(nameof(Primary), "UniqueId1");
        });
    }

    [TestClass]
    public class DatabaseDefinitionInvalidFKTest
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidForeignKeyRegistrationException))]
        public void InvalidFK_SingleSetFkToMultiPk()
        {
            var _ = new InvalidFK_SingleSetFkToMultiPk();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidForeignKeyRegistrationException))]
        public void InvalidFK_SingleAddFkToMultiPk()
        {
            var _ = new InvalidFK_SingleAddFkToMultiPk();
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

        [TestMethod]
        [ExpectedException(typeof(InvalidForeignKeyRegistrationException))]
        public void InvalidFK_UniqueConstratintAsFk_SingleToMulti()
        {
            var _ = new InvalidFK_UniqueConstratintAsFk_SingleToMulti();
        }
    }
}
