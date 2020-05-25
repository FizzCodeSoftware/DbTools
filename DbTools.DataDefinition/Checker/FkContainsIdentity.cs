namespace FizzCode.DbTools.DataDefinition.Checker
{
    using FizzCode.DbTools.DataDefinition;

    public class FkContainsIdentity : SchemaCheckFk
    {
        public override string DisplayName => "Fk contains identity field";
        public Identity Identity { get; set; }

        public override string DisplayInfo
        {
            get
            {
                return $"FK: {ForeignKey}\r\nIdentity: {Identity}";
            }
        }

        public override SchemaAndContentCheckSeverity Severity => SchemaAndContentCheckSeverity.Error;
    }
}
