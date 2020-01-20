namespace FizzCode.DbTools.DataDefinition.Tests
{
    using System.Collections.Generic;
    using FizzCode.DbTools.DataDefinition.Generic1;

    // TODO
    public class ForeignKeyToAnotherSchema : DatabaseDeclaration
    {
        public SqlTable ChildꜗChild { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
            table.AddForeignKey(nameof(ParentꜗParent));
        });

        public SqlTable ParentꜗParent { get; } = AddTable(table =>
        {
            table.AddInt32("Id").SetPK().SetIdentity();
            table.AddNVarChar("Name", 100);
        });

    }
}
