namespace FizzCode.DbTools.DataDefinition
{
    public class SqlTableProperty
    {
        public SqlTable SqlTable { get; }

        public SqlTableProperty(SqlTable sqlTable)
        {
            SqlTable = sqlTable;
        }
    }
}
