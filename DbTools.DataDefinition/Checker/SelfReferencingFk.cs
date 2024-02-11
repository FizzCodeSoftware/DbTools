namespace FizzCode.DbTools.DataDefinition.Checker;

public class SelfReferencingFk : SchemaCheckFk
{
    public override string DisplayName { get; } = "Fk references the same table";

    public string? Comment { get; set; }

    public override string DisplayInfo => $"{base.DisplayInfo} - {Comment}";

    public override SchemaAndContentCheckSeverity Severity => SchemaAndContentCheckSeverity.Check;
}
