namespace FizzCode.DbTools.DataDefinition.Base.Migration
{
    using FizzCode.DbTools.DataDefinition.Base;

    public abstract class ColumnNewOrDelete : ColumnMigration
    {
        public string Name => SqlColumn.Name;
        public SqlType Type => (SqlType)SqlColumn.Type;

        public SqlTable Table => SqlColumn.Table;
    }
}
