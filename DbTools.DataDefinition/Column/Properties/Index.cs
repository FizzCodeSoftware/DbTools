namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;

    public class Index : IndexBase<SqlTable>
    {
        public SqlTable SqlTable { get => SqlTableOrView; }

        public List<SqlColumn> Includes { get; set; } = new();

        public Index(SqlTable sqlTable, string name, bool unique = false)
            : base(sqlTable, name, unique)
        {
        }
    }

    public class IndexView : IndexBase<SqlView>
    {
        public SqlView SqlView { get => SqlTableOrView; }

        public List<SqlViewColumn> Includes { get; set; } = new();

        public IndexView(SqlView sqlView, string name, bool unique = false)
            : base(sqlView, name, unique)
        {
        }
    }
}
