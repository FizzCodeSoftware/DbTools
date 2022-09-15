namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.TestBase;

    public class SqlTableCustomPropertyConstructor : TestDatabaseDeclaration
    {
        public Table1Table Table1 { get; } = new Table1Table();
    }

    public class Table1Table : SqlTable
    {
        public SqlColumn Id { get; } = Generic1.AddInt32().SetPK().SetIdentity();
        public SqlColumn Name { get; } = Generic1.AddNVarChar(100);
        public SqlTableCustomProperty MyCustomPropertyWithConstructor { get; } = new MyCustomPropertyWithConstructor("TestValue");
    }

    public class MyCustomPropertyWithConstructor : SqlTableCustomProperty
    {
        public MyCustomPropertyWithConstructor(string value)
        {
            Value = value;
        }

        public MyCustomPropertyWithConstructor(SqlTable sqlTable, string value)
            : base(sqlTable)
        {
            Value = value;
        }

        public string Value { get; }

        public override string GenerateCSharpConstructorParameters()
        {
            return $"\"{Value}\"";
        }
    }
}
