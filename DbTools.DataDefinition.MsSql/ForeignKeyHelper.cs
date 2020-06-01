namespace FizzCode.DbTools.DataDefinition.MsSql2016
{
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    /// <summary>
    /// MsSql2016 specific Foreign key helper extension methods
    /// </summary>
    public static class ForeignKeyHelper
    {
        /// <summary>
        /// Sets an existing column as an FK, pointing to the <paramref name="referredColumnName"/> of <paramref name="referredTableName"/>, with the Nocheck property.
        /// Note <paramref name="referredColumnName"/> has to be a unique key.
        /// </summary>
        /// <param name="singleFkColumn">The existing column to set as FK.</param>
        /// <param name="referredTableName">The name of the referred table.</param>
        /// <param name="referredColumnName">The name of the referred column.</param>
        /// <param name="fkName"></param>
        /// <returns>The original <paramref name="singleFkColumn"/>.</returns>
        public static SqlColumn SetForeignKeyToColumnNoCheck(this SqlColumn singleFkColumn, string referredTableName, string referredColumnName, string fkName = null)
        {
            var property = new SqlEngineVersionSpecificProperty(MsSqlVersion.MsSql2016, "Nocheck", "true");
            return singleFkColumn.SetForeignKeyToColumn(referredTableName, referredColumnName, property, fkName);
        }
    }
}