namespace FizzCode.DbTools.DataDefinition.Tests
{
    using FizzCode.DbTools.DataDefinition;

    public class TestDatabaseSelfFK : DatabaseDeclaration
    {
        public SqlTable Company { get; } = AddTable(table =>
          {
              table.AddInt32("Id").SetPK().SetIdentity();
              table.AddForeignKey(nameof(Company), "Parent");
              table.AddNVarChar("Name", 100);
          });
    }
}