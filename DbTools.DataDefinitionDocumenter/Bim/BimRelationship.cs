namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using FizzCode.DbTools.DataDefinition;

    public class BimRelationship
    {
        public SqlColumn FromColumn { get; set; }
        public SchemaAndTableName ToTableSchemaAndTableName { get; set; }
        public string ToColumnName { get; set; }
        public string RelationshipIdentifier { get; set; }

        public BimRelationship(SqlColumn fromColumn, SchemaAndTableName toTableSchemaAndTableName, string toColumnName, string relationshipIdentifier = null)
        {
            FromColumn = fromColumn;
            ToTableSchemaAndTableName = toTableSchemaAndTableName;
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

        public string ToKey
        {
            get
            {
                return ToTableSchemaAndTableName + "/" + ToColumnName;
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

        public override string ToString()
        {
            return FromTableSchemaAndTableName + " -> " + ToTableSchemaAndTableName + " ("+ RelationshipIdentifier +")";
        }
    }
}
