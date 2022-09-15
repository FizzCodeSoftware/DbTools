namespace FizzCode.DbTools.DataDefinition.Migration
{
    using FizzCode.DbTools.DataDefinition.Base;
    public class IdentityMigration : SqlColumnPropertyMigration
    {
        public Identity Identity { get; set; }
    }
}
