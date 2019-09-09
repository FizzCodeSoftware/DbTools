using System.Collections.Generic;

namespace FizzCode.DbTools.DataDefinition
{

    public class ForeignKeyRegistrationToReferredTable : ForeignKeyRegistrationBase
    {
        public bool IsNullable { get; set; }
        public List<ColumnReference> Map { get; set; }

        public ForeignKeyRegistrationToReferredTable(SqlTable table, SchemaAndTableName referredTableName, bool isNullable, string fkName, List<ColumnReference> map) : base(table, referredTableName, fkName)
        {
            IsNullable = isNullable;
            Map = map;
        }
    }
}
