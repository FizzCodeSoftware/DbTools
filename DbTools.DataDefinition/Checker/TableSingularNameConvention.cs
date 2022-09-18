namespace FizzCode.DbTools.DataDefinition.Checker
{
    public abstract class Convention : SchemaCheck
    {
        public override string Type => "Convention";
    }

    public class TableSingularNameConvention : Convention
    {
        public TableSingularNameConvention(SchemaAndTableName schemaAndTableName)
        {
            _schemaAndTableName = schemaAndTableName;
        }

        private SchemaAndTableName _schemaAndTableName;

        public void SetTableSchemaAndTableName(SchemaAndTableName schemaAndTableName)
        {
            _schemaAndTableName = schemaAndTableName;
        }

        public override string DisplayName => "Table name should be singular.";

        public override string DisplayInfo => $"TableName: {_schemaAndTableName.TableName}";

        public override string Schema => _schemaAndTableName.Schema;
        public override string ElementName => _schemaAndTableName.TableName;
    }
}
