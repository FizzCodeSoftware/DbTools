namespace FizzCode.DbTools.DataDefinition.Migration
{
    using FizzCode.DbTools.DataDefinition.Base;

    public abstract class ColumnNewOrDelete : ColumnMigration
    {
        public string Name => SqlColumn.Name;
        public SqlType Type => SqlColumn.Type;

        public SqlTable Table => SqlColumn.Table;
    }
}
