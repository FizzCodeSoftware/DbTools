namespace FizzCode.DbTools.DataDefinition.Base;

public class IndexView(SqlView sqlView, string name, bool unique = false)
    : IndexBase<SqlView>(sqlView, name, unique)
{
    public SqlView SqlView { get => SqlTableOrView!; }

    public List<SqlViewColumn> Includes { get; set; } = [];
}
