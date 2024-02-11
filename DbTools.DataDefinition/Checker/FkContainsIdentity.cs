using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinition.Checker;
public class FkContainsIdentity : SchemaCheckFk
{
    public override string DisplayName => "Fk contains identity field";
    public required Identity Identity { get; init; }

    public override string DisplayInfo => $"FK: {ForeignKey}\r\nIdentity: {Identity}";

    public override SchemaAndContentCheckSeverity Severity => SchemaAndContentCheckSeverity.Error;
}
