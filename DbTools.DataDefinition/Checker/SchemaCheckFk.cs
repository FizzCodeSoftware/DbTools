using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinition.Checker;
public abstract class SchemaCheckFk : SchemaCheck
{
    public ForeignKey ForeignKey { get; set; }

    public override string DisplayInfo => $"{ForeignKey}";

    public override string Schema => ForeignKey.SqlTable.SchemaAndTableName.Schema;
    public override string ElementName => ForeignKey.SqlTable.SchemaAndTableName.TableName;
}
