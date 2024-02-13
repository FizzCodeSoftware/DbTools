namespace FizzCode.DbTools.DataDefinition.Base;

public class UniqueConstraint(SqlTable sqlTable, string? name) : IndexBase<SqlTable>(sqlTable, name, true)
{
    public SqlTable SqlTable { get => SqlTableOrView!; }

    public new bool Unique
    {
        get => true;

        set
        {
            if (!value)
                throw new System.ArgumentException("Unique Constraint is always Unique.");
        }
    }

    public override string ToString()
    {
        return $"{GetColumnsInString()} on {SqlTableOrView?.SchemaAndTableName}";
    }
}
