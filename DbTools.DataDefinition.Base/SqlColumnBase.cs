namespace FizzCode.DbTools.DataDefinition.Base;
public abstract class SqlColumnBase : SqlElementWithNameAndType
{
    /// <summary>
    /// The <see cref="SqlTable"/> or <see cref="SqlView"/> of the <see cref="SqlColumnBase">Sql Column</see>.
    /// Note that with declaration, first this will be null, and will be set from the constructor of <see cref="DatabaseDeclaration">.
    /// </summary>
    public SqlTableOrView? SqlTableOrView { get; set; }

    private List<SqlColumnProperty>? _properties;
    public List<SqlColumnProperty> Properties => _properties ??= [];

    public override string ToString()
    {
        return $"{Name} {Types.Describe(SqlTableOrView?.DatabaseDefinition?.MainVersion)} on {SqlTableOrView?.SchemaAndTableName}";
    }

    public SqlColumnBase CopyTo(SqlColumnBase column)
    {
        column.Name = Name;
        Types.CopyTo(column.Types);
        column.SqlTableOrView = SqlTableOrView;
        return column;
    }
    
    public bool HasProperty<T>()
        where T : SqlColumnProperty
    {
        return Properties.Any(x => x is T);
    }

    protected override IDatabaseDefinition? DatabaseDefinition => SqlTableOrView?.DatabaseDefinition;
}