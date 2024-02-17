using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
public class BimRelationship(SqlColumn fromColumn, SchemaAndTableName toTableSchemaAndTableName, string toColumnName, string? relationshipIdentifier = null)
{
    public SqlColumn FromColumn { get; set; } = fromColumn;
    public SchemaAndTableName ToTableSchemaAndTableName { get; set; } = toTableSchemaAndTableName;
    public string ToColumnName { get; set; } = toColumnName;
    public string? RelationshipIdentifier { get; set; } = relationshipIdentifier;

    public SchemaAndTableName FromTableSchemaAndTableName => FromColumn.Table.SchemaAndTableName!;

    public string ToKey => ToTableSchemaAndTableName + "/" + ToColumnName;

    public string? RelationshipIdentifierKey
    {
        get
        {
            if (RelationshipIdentifier is null)
                return null;

            return ToTableSchemaAndTableName + "/" + RelationshipIdentifier;
        }
    }

    public override string ToString()
    {
        return FromTableSchemaAndTableName + " -> " + ToTableSchemaAndTableName + " (" + RelationshipIdentifier + ")";
    }
}
