using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.Tabular;
/// <summary>
/// For a Tabular relationship, this property indicates that if multiple relationships are referencing to a table, references with the same <see cref="RelationshipIdentifier"/> will use the same key table.
/// The Tabular model is created with DbTools BimGenerator.
/// </summary>
public class TabularRelationProperty(SqlColumn table, string keySchemaName, string keyTableName, string keyColumnName, string relationshipIdentifier) : SqlColumnProperty(table)
{
    public SchemaAndTableName KeySchemaAndTableName { get; } = new SchemaAndTableName(keySchemaName, keyTableName);
    public string KeyColumnName { get; } = keyColumnName;
    public string RelationshipIdentifier { get; } = relationshipIdentifier;
}

public static class TabularRelationPropertyHelper
{
    public static TabularRelationProperty AddTabularRelation(this SqlColumn sqlColumn, string keySchemaName, string keyTableName, string keyColumnName, string relationshipIdentifier)
    {
        var property = new TabularRelationProperty(sqlColumn, keySchemaName, keyTableName, keyColumnName, relationshipIdentifier);
        sqlColumn.Properties.Add(property);
        return property;
    }
}
