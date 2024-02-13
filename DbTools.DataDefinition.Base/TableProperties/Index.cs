namespace FizzCode.DbTools.DataDefinition.Base;
public class Index(SqlTable sqlTable, string? name, bool unique = false)
    : IndexBase<SqlTable>(sqlTable, name, unique)
{
    public SqlTable SqlTable { get => SqlTableOrView!; }

    public List<SqlColumn> Includes { get; set; } = [];
}
