namespace FizzCode.DbTools.DataDefinition.Checker;

public abstract class SchemaCheck
{
    public virtual SchemaAndContentCheckSeverity Severity { get; set; }
    public abstract string DisplayName { get; }
    public abstract string DisplayInfo { get; }

    public abstract string? Schema { get; }
    public abstract string ElementName { get; }

    public virtual string Type => "Schema";

    public virtual string TargetType => "Table";

    public override string ToString()
    {
#pragma warning disable IDE0071 // Simplify interpolation
        return $"[{Severity.ToString()}] {GetType().Name} {DisplayInfo}";
#pragma warning restore IDE0071 // Simplify interpolation
    }
}