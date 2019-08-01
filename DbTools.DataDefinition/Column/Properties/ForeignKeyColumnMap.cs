namespace FizzCode.DbTools.DataDefinition
{
    public class ForeignKeyColumnMap
    {
        public SqlColumn ForeignKeyColumn { get; }
        public SqlColumn PrimaryKeyColumn { get; }

        public ForeignKeyColumnMap(SqlColumn foreignKeyColumn, SqlColumn primaryKeyColumn)
        {
            ForeignKeyColumn = foreignKeyColumn;
            PrimaryKeyColumn = primaryKeyColumn;
        }
    }
}
