using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinition.Checker;
public abstract class SchemaCheckFk : SchemaCheck
{
    public required ForeignKey ForeignKey { get; init; }

    public override string DisplayInfo => $"{ForeignKey}";

    public override string Schema => ForeignKey.SqlTable.SchemaAndTableNameSafe.Schema!;
    public override string ElementName => ForeignKey.SqlTable.SchemaAndTableNameSafe.TableName;
}
