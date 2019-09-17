namespace FizzCode.DbTools.DataDefinition
{
    public abstract class SqlTableProperty
    {
        public SqlTable SqlTable { get; }

        protected SqlTableProperty(SqlTable sqlTable)
        {
            SqlTable = sqlTable;
        }
    }
}
