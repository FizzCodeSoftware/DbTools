namespace FizzCode.DbTools.DataDefinition
{
    public class SqlColumnProperty
    {
        public SqlColumn SqlColumn { get; }

        public SqlColumnProperty(SqlColumn sqlColumn)
        {
            SqlColumn = sqlColumn;
        }
    }
}