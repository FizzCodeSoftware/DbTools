namespace FizzCode.DbTools.DataDefinition
{
    public class ForeignKeyColumnMap
    {
        public SqlColumn ForeignKeyColumn { get; }

        public SqlColumn ReferredColumn { get; }

        public ForeignKeyColumnMap(SqlColumn foreignKeyColumn, SqlColumn referredColumn)
        {
            ForeignKeyColumn = foreignKeyColumn;
            ReferredColumn = referredColumn;
        }
    }
}
