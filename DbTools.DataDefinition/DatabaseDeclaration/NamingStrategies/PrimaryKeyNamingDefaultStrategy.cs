namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.DataDefinition.Base;
    public class PrimaryKeyNamingDefaultStrategy : IPrimaryKeyNamingStrategy
    {
        public void SetPrimaryKeyName(PrimaryKey pk)
        {
            if (pk.SqlTableOrView.SchemaAndTableName?.TableName == null)
                return;

            pk.Name = $"PK_{pk.SqlTable.SchemaAndTableName.TableName}";
        }
    }
}
