namespace FizzCode.DbTools.DataDefinition.Migration
{
    public abstract class ColumnNewOrDelete : ColumnMigration
    {
        public string Name => SqlColumn.Name;
        public SqlType Type => SqlColumn.Type;

        public SqlTable Table => SqlColumn.Table;
    }
}
