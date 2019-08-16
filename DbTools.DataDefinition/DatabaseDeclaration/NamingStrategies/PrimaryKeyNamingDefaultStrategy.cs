namespace FizzCode.DbTools.DataDefinition
{
    public class PrimaryKeyNamingDefaultStrategy : IPrimaryKeyNamingStrategy
    {
        public void SetPrimaryKeyName(PrimaryKey pk)
        {
            if (pk.SqlTable.SchemaAndTableName.TableName == null)
                return;

            pk.Name = $"PK_{pk.SqlTable.SchemaAndTableName.TableName}";
        }
    }
}
