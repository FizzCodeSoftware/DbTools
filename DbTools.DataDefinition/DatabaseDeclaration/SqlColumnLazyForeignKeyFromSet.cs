namespace FizzCode.DbTools.DataDefinition
{
    public class SqlColumnLazyForeignKeyFromSet : SqlColumnLazyForeignKey
    {
        public SqlColumnDeclaration ReferringColumn { get; }

        public SqlColumnLazyForeignKeyFromSet(SqlColumnDeclaration referringColumn, LazySqlTable referredTable, bool isNullable) : base(null, referredTable, null, isNullable)
        {
            ReferringColumn = ReferringColumn;
        }
    }
}