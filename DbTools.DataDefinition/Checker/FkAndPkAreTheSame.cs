namespace FizzCode.DbTools.DataDefinition.Checker
{
    using FizzCode.DbTools.DataDefinition;

    public class FkAndPkAreTheSame : SchemaCheckFk
    {
        public override string DisplayName => "Fk and Pk columns are the same";

        public PrimaryKey PrimaryKey { get; set; }
        public string Comment { get; set; }

        public override string DisplayInfo => $"FK: {ForeignKey}\r\nPK: {PrimaryKey}";

        public override SchemaAndContentCheckSeverity Severity => SchemaAndContentCheckSeverity.Error;
    }
}
