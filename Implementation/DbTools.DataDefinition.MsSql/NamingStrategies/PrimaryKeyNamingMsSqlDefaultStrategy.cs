namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using System.Globalization;
    using FizzCode.DbTools.DataDeclaration;
    using FizzCode.DbTools.DataDefinition.Base;

    public class PrimaryKeyNamingMsSqlDefaultStrategy : IPrimaryKeyNamingStrategy
    {
        public void SetPrimaryKeyName(PrimaryKey pk)
        {
            if (pk.SqlTable.SchemaAndTableName?.TableName == null)
                return;

            var pkName = $"PK_{pk.SqlTable.SchemaAndTableName.TableName}";
            if (pkName.Length > 110)
            {
                pkName = pkName.Substring(0, 110) + "__" + pkName.GetHashCode(StringComparison.InvariantCultureIgnoreCase).ToString("D", CultureInfo.InvariantCulture);
            }

            pk.Name = pkName;
        }
    }
}
