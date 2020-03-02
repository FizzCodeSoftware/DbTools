namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;

    internal class ForeignKeyRegistrationToReferredTable : ForeignKeyRegistrationNonExsistingColumn
    {
        public List<ColumnReference> Map { get; set; }

        public ForeignKeyRegistrationToReferredTable(SqlTable table, SchemaAndTableName referredTableName, bool isNullable, string fkName, List<ColumnReference> map)
            : base(table, referredTableName, isNullable, fkName)
        {
            Map = map;
        }
    }
}