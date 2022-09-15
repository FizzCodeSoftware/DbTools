namespace FizzCode.DbTools.DataDefinition.Migration
{
    using FizzCode.DbTools.DataDefinition.Base;

    public class IdentityChange : IdentityMigration
    {
        public Identity NewIdentity { get; set; }
    }
}
