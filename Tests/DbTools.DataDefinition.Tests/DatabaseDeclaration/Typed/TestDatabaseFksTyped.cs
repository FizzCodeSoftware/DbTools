namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.TestBase;

    public class TestDatabaseFksTyped : TestDatabaseDeclaration
    {
        public Child Child { get; } = new Child();
        public ChildChild ChildChild { get; } = new ChildChild();
        public Parent Parent { get; } = new Parent();
    }

    public class Child : SqlTable
    {
        public SqlColumn Id { get; } = Generic1.AddInt32().SetPK().SetIdentity();
        public SqlColumn Name { get; } = Generic1.AddNVarChar(100);

        public SqlColumn ParentId { get; } = Generic1.SetForeignKeyTo(nameof(TestDatabaseFksTyped.Parent));
    }

    public class ChildChild : SqlTable
    {
        public SqlColumn Id { get; } = Generic1.AddInt32().SetPK().SetIdentity();
        public SqlColumn Name { get; } = Generic1.AddNVarChar(100);

        public SqlColumn ChildId { get; } = Generic1.SetForeignKeyTo(nameof(TestDatabaseFksTyped.Child));
    }

    public class Parent : SqlTable
    {
        public SqlColumn Id { get; } = Generic1.AddInt32().SetPK().SetIdentity();
        public SqlColumn Name { get; } = Generic1.AddNVarChar(100);
    }
}