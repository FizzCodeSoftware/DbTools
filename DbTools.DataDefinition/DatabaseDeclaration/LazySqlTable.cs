namespace FizzCode.DbTools.DataDefinition
{
    using System;

    public class LazySqlTable : Lazy<SqlTable>
    {
        public LazySqlTable(Func<SqlTable> valueFactory)
            : base(valueFactory)
        {
        }
    }
}
