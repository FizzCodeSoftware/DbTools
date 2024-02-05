namespace FizzCode.DbTools.DataDefinition.Base;
public class Index : IndexBase<SqlTable>
{
    public SqlTable SqlTable { get => SqlTableOrView; }

    public List<SqlColumn> Includes { get; set; } = [];

    public Index(SqlTable sqlTable, string? name, bool unique = false)
        : base(sqlTable, name, unique)
    {
    }
}

public class IndexView : IndexBase<SqlView>
{
    public SqlView SqlView { get => SqlTableOrView; }

    public List<SqlViewColumn> Includes { get; set; } = [];

    public IndexView(SqlView sqlView, string name, bool unique = false)
        : base(sqlView, name, unique)
    {
    }
}
