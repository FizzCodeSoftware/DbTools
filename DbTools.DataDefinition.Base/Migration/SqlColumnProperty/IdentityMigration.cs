namespace FizzCode.DbTools.DataDefinition.Base.Migration
{
    using FizzCode.DbTools.DataDefinition.Base;
    public class IdentityMigration : SqlColumnPropertyMigration
    {
        public Identity Identity { get; set; }
    }
}
