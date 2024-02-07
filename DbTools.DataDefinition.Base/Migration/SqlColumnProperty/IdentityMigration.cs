namespace FizzCode.DbTools.DataDefinition.Base.Migration;
public class IdentityMigration : SqlColumnPropertyMigration
{
    public required Identity Identity { get; init; }
}
