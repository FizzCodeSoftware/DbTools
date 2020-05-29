namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.TestBase;

    public class TestDatabaseSimpleTyped : TestDatabaseDeclaration
    {
        public Company Company { get; } = new Company();
    }

    public class Company : SqlTable
    {
        public SqlColumn Id { get; } = Generic1Columns.AddInt32().SetPK().SetIdentity();

        public SqlColumn Name { get; } = Generic1Columns.AddNVarChar(100);
    }
}