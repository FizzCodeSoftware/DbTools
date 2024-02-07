namespace FizzCode.DbTools.DataDefinition.Base.Migration;
public class IdentityChange : IdentityMigration
{
    public required Identity NewIdentity { get; init; }
}
