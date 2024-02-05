using System.Diagnostics;
using FizzCode.DbTools.Common;

namespace FizzCode.DbTools.DataDefinition.Base;
[DebuggerDisplay("{ToString(),nq}")]
public abstract class SqlTableOrView
{
    /// <summary>
    /// The <see cref="DatabaseDefinition">definition</see> or <see cref="DatabaseDeclaration">declaration</see> of the <see cref="SqlTableOrView"/>.
    /// Note that with declaration, first this will be null, and will be set from the constructor of <see cref="DatabaseDeclaration">.
    /// </summary>
    public IDatabaseDefinition? DatabaseDefinition { get; set; }
    /// <summary>
    /// The <see cref="SchemaAndTableName"/> of the <see cref="SqlTableOrView"/>.
    /// Note that with declaration, first this will be null, and will be set from the constructor of <see cref="DatabaseDeclaration">.
    /// </summary>
    public SchemaAndTableName? SchemaAndTableName { get; set; }

    public SqlTableOrView()
    {
    }

    public SqlTableOrView(string schema, string tableName)
    {
        SchemaAndTableName = new SchemaAndTableName(schema, tableName);
    }

    public SqlTableOrView(string tableName)
    {
        SchemaAndTableName = new SchemaAndTableName(tableName);
    }

    public SqlTableOrView(SchemaAndTableName schemaAndTableName)
    {
        SchemaAndTableName = schemaAndTableName;
    }

    public override string ToString()
    {
        return SchemaAndTableName?.SchemaAndName ?? "";
    }

    /// <summary>
    /// Accessing the <see cref="SchemaAndTableName"/> with null check.
    /// </summary>
    public SchemaAndTableName SchemaAndTableNameSafe
    {
        get
        {
            Throw.InvalidOperationExceptionIfNull(SchemaAndTableName);
            return SchemaAndTableName;
        }
    }
}
