namespace FizzCode.DbTools.DataDefinition.Base;
public class ForeignKeyRegistrationToReferredTableExistingColumns : ForeignKeyRegistrationBase
{
    public List<ColumnReference> Map { get; set; }

    public ForeignKeyRegistrationToReferredTableExistingColumns(SqlTable table, SchemaAndTableName referredTableName, string fkName, List<ColumnReference> map)
        : base(table, referredTableName, fkName)
    {
        Map = map;
    }
}
