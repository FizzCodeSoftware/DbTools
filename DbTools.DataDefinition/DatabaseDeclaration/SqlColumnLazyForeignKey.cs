namespace FizzCode.DbTools.DataDefinition
{
    public class SqlColumnLazyForeignKey : SqlColumnDeclaration
    {
        public LazySqlTable ReferredTable { get; }
        public string ColumnNamePrefix { get; }

        public SqlColumnLazyForeignKey(SqlTableDeclaration referringTable, LazySqlTable referredTable, string columnNamePrefix, bool isNullable)
        {
            Table = referringTable;
            ReferredTable = referredTable;
            ColumnNamePrefix = columnNamePrefix;
            IsNullable = isNullable;
            Type = SqlType.Unknown;
        }
    }
}