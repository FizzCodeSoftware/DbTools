namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using FizzCode.DbTools.DataDefinition;

    public class BimRelationship
    {
        public SqlColumn FromColumn { get; set; }
        public SchemaAndTableName ToSchemaAndTableName { get; set; }
        public string ToColumnName { get; set; }
        public string RelationshipIdentifier { get; set; }

        public BimRelationship(SqlColumn fromColumn, SchemaAndTableName toSchemaAndTableName, string toColumnName, string relationshipIdentifier = null)
        {
            FromColumn = fromColumn;
            ToSchemaAndTableName = toSchemaAndTableName;
            ToColumnName = toColumnName;
            RelationshipIdentifier = relationshipIdentifier;
        }

        public SchemaAndTableName FromTableSchemaAndTableName
        {
            get
            {
                return FromColumn.Table.SchemaAndTableName;
            }
        }

        public SchemaAndTableName ToTableSchemaAndTableName
        {
            get
            {
                return ToSchemaAndTableName;
            }
        }

        public string ToKey
        {
            get
            {
                return FromTableSchemaAndTableName + "/" + FromColumn.Name + "/" + ToTableSchemaAndTableName + "/" + ToColumnName;
            }
        }

        public string RelationshipIdentifierKey
        {
            get
            {
                if (RelationshipIdentifier == null)
                    return null;
                
                return ToTableSchemaAndTableName + "/" + RelationshipIdentifier;
            }
        }
    }
}
