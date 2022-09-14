namespace FizzCode.DbTools.DataDefinition
{
    public class PrimaryKey : IndexBase<SqlTable>
    {
        public SqlTable SqlTable { get => SqlTableOrView; }

        public PrimaryKey(SqlTable sqlTable, string name)
            : base(sqlTable, name)
        {
        }

        public override string ToString()
        {
            return $"{GetColumnsInString()} on {SqlTableOrView.SchemaAndTableName}";
        }
    }
}
