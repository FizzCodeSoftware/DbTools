namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.TestBase;

    public class TestDatabaseSimple : TestDatabaseDeclaration
    {
        public SqlTable Company { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
        });
    }

    public class Company : SqlTable
    {
        private SqlColumn _id;
        public SqlColumn Id
        {
            get
            {
                if (_id == null)
                    _id = this.AddInt32("Id").SetPK().SetIdentity();
                return _id;
            }
        }

        private SqlColumn _name;
        public SqlColumn Name
        {
            get
            {
                if (_name == null)
                    _name = this.AddNVarChar("Name", 100);
                return _name;
            }
        }
    }

    public class TestDatabaseSimple2 : TestDatabaseDeclaration
    {
        public Company Company { get; } = new Company();
    }

    public class TestDatabaseIndex : TestDatabaseDeclaration
    {
        public SqlTable Company { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
            table.AddIndex("Name");
        });
    }

    public class TestDatabaseUniqueIndex : TestDatabaseDeclaration
    {
        public SqlTable Company { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
            table.AddIndex(true, "Name");
        });
    }
}