namespace FizzCode.DbTools.DataDefinition
{
    public abstract class SqlColumnProperty
    {
        public SqlColumn SqlColumn { get; }

        protected SqlColumnProperty(SqlColumn sqlColumn)
        {
            SqlColumn = sqlColumn;
        }
    }
}