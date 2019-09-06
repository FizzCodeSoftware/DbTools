namespace FizzCode.DbTools.DataDefinition
{
    using System;

    public class SchemaAndTableName : IComparable, IEquatable<SchemaAndTableName>
    {
        public string Schema { get; protected set; }
        public string TableName { get; protected set; }

        public SchemaAndTableName(string tableName)
        {
            if (tableName.IndexOf(DatabaseDeclaration.SchemaTableNameSeparator) == -1)
            {
                TableName = tableName;
            }
            else
            {
                var parts = tableName.Split(DatabaseDeclaration.SchemaTableNameSeparator);
                Schema = parts[0];
                TableName = parts[1];
            }
        }

        public SchemaAndTableName(string schema, string tableName)
        {
            Schema = schema;
            TableName = tableName;
        }

        public string SchemaAndName
        {
            get
            {
                if (Schema == null)
                    return TableName;

                return Schema + "." + TableName;
            }
        }

        public override string ToString()
        {
            return SchemaAndName;
        }

        public int CompareTo(object obj)
        {
            var other = (SchemaAndTableName)obj;

            var compareSchema = string.CompareOrdinal(Schema, other.Schema);

            if (compareSchema != 0)
                return compareSchema;

            return string.CompareOrdinal(TableName, other.TableName);
        }

        public bool Equals(SchemaAndTableName other)
        {
            if (other is null)
                return false;

            return Schema == other.Schema && TableName == other.TableName;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SchemaAndTableName other))
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
                hash = (hash * 23) + Schema?.GetHashCode() ?? 0;
                hash = (hash * 23) + TableName.GetHashCode();
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
