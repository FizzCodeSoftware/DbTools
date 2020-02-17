namespace FizzCode.DbTools.DataDefinition
{
    public abstract class SqlColumnProperty
    {
        public SqlColumn SqlColumn { get; }

        public SqlColumnProperty(SqlColumn sqlColumn)
        {
            SqlColumn = sqlColumn;
        }
    }
}