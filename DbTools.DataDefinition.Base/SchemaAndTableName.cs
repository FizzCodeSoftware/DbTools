namespace FizzCode.DbTools.DataDefinition.Base
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Represents the Schema and Table name of SQL tables.
    /// Example: dbo.MyTable
    /// Use null for Schema if the database engine does not support Schema names, or to use the default Schema name (example: dbo in Microsoft SQL Server).
    /// </summary>
    [DebuggerDisplay("{ToString(),nq}")]
    public class SchemaAndTableName : IComparable, IEquatable<SchemaAndTableName>
    {
        public string? Schema { get; set; }
        public string TableName { get; }

        public SchemaAndTableName(string tableName)
        {
            if (!tableName.Contains(DatabaseDeclarationConst.SchemaTableNameSeparator, StringComparison.InvariantCultureIgnoreCase))
            {
                TableName = tableName;
            }
            else
            {
                var parts = tableName.Split(DatabaseDeclarationConst.SchemaTableNameSeparator);
                Schema = parts[0];
                TableName = parts[1];
            }
        }

        public SchemaAndTableName(string schema, string tableName)
        {
            Schema = schema;
            TableName = tableName;
        }

        public static string Concat(string? schema, string tableName)
        {
            if (schema == null)
                return tableName;

            return schema + "." + tableName;
        }

        /// <summary>
        /// Returns <see cref="Schema"/> and <see cref="TableName"/> joined with a . (dot), or the <see cref="TableName"/> if <see cref="Schema"/> is null.
        /// </summary>
        public string SchemaAndName => Concat(Schema, TableName);

        public override string ToString()
        {
            return SchemaAndName;
        }

        public int CompareTo(object? obj)
        {
            if (obj == null)
                return -1;
            
            var other = (SchemaAndTableName)obj;

            var compareSchema = string.CompareOrdinal(Schema, other.Schema);

            if (compareSchema != 0)
                return compareSchema;

            return string.CompareOrdinal(TableName, other.TableName);
        }

        public bool Equals(SchemaAndTableName? other)
        {
            if (other is null)
                return false;

            return Schema == other.Schema && TableName == other.TableName;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not SchemaAndTableName other)
                return false;

            return Equals(other);
        }

        public static bool operator ==(SchemaAndTableName obj1, SchemaAndTableName obj2)
        {
            if (obj1 is null)
                return obj2 is null;

            return obj1.Equals(obj2);
        }

        public static bool operator !=(SchemaAndTableName obj1, SchemaAndTableName obj2)
        {
            return !obj1.Equals(obj2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = (hash * 23) + Schema?.GetHashCode(StringComparison.InvariantCultureIgnoreCase) ?? 0;
                hash = (hash * 23) + TableName?.GetHashCode(StringComparison.InvariantCultureIgnoreCase) ?? 0;
                return hash;
            }
        }

        public static implicit operator string(SchemaAndTableName schemaAndTableName)
        {
            return schemaAndTableName.ToString();
        }

        public static implicit operator SchemaAndTableName(string tableName)
        {
            return new SchemaAndTableName(tableName);
        }
    }
}
