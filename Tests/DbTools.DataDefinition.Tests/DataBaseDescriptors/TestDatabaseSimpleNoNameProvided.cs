namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition;

    public class TestDatabaseSimpleNoNameProvided : DatabaseDeclaration
    {
        public SqlTable Company { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
        });
    }
}