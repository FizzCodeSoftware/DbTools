namespace FizzCode.DbTools.DataDefinition.Base.Migration;

public class DefaultValueMigration : SqlColumnPropertyMigration
{
    public required DefaultValue DefaultValue { get; init; }
}