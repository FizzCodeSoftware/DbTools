namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition.Generic1;

    public class TestDatabaseSimple : DatabaseDeclaration
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

    public class TestDatabaseSimple2 : DatabaseDeclaration
    {
        public Company Company { get; } = new Company();
    }
}