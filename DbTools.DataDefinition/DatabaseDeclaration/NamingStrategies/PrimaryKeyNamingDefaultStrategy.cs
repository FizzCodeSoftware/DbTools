namespace FizzCode.DbTools.DataDefinition
{
    public class PrimaryKeyNamingDefaultStrategy : IPrimaryKeyNamingStrategy
    {
        public void SetPrimaryKeyName(PrimaryKey pk)
        {
            if (pk.SqlTable.Name == null)
                return;

            pk.Name = $"PK_{pk.SqlTable.Name}";
        }
    }
}
