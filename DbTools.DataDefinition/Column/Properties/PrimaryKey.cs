namespace FizzCode.DbTools.DataDefinition
{
    public class PrimaryKey : IndexBase
    {
        public PrimaryKey(SqlTable sqlTable, string name)
            : base(sqlTable, name)
        {
        }

        public override string ToString()
        {
            return $"{GetColumnsInString()} on {SqlTable.SchemaAndTableName}";
        }
    }
}
