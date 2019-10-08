using System.Collections.Generic;

namespace FizzCode.DbTools.DataDefinition
{
    internal class ForeignKeyRegistrationToReferredTableExistingColumns : ForeignKeyRegistrationBase
    {
        public List<ColumnReference> Map { get; set; }

        public ForeignKeyRegistrationToReferredTableExistingColumns(SqlTable table, SchemaAndTableName referredTableName, string fkName, List<ColumnReference> map)
            : base(table, referredTableName, fkName)
        {
            Map = map;
        }
    }
}
