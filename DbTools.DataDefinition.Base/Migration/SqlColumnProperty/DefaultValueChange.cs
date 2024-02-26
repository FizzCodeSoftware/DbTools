namespace FizzCode.DbTools.DataDefinition.Base.Migration;
public class DefaultValueChange : DefaultValueMigration
{
    public required DefaultValue NewDefaultValue { get; init; }
}
