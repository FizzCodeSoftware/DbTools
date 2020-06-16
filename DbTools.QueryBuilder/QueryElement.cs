namespace FizzCode.DbTools.QueryBuilder
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FizzCode.DbTools.DataDefinition;

    public static class SqlTableExtension
    {
        public static T Alias<T>(this T table, string alias = null) where T : SqlTable, new()
        {
            var newTable = new T
            {
                DatabaseDefinition = table.DatabaseDefinition,
                SchemaAndTableName = table.SchemaAndTableName
            };

            newTable.Properties.AddRange(table.Properties);

            foreach (var column in table.Columns)
            {
                var newSqlColumn = new SqlColumn();
                column.CopyTo(newSqlColumn);
                newSqlColumn.Table = newTable;
                newTable.Columns.Add(newSqlColumn);
            }

            SetAlias(newTable, alias);

            SetupDeclaredTable(newTable);

            return newTable;
        }

        private static void SetAliasProperty(this SqlTable sqlTable, string alias)
        {
            var aliasTableProperty = sqlTable.Properties.OfType<AliasTableProperty>().FirstOrDefault();
            if (aliasTableProperty == null)
            {
                aliasTableProperty = new AliasTableProperty();
                sqlTable.Properties.Add(aliasTableProperty);
            }

            aliasTableProperty.Alias = alias;
        }

        public static string GetAlias(this SqlTable sqlTable)
        {
            var aliasTableProperty = sqlTable.Properties.OfType<AliasTableProperty>().FirstOrDefault();
            return aliasTableProperty?.Alias;
        }

        public static void SetAlias(SqlTable table, string alias)
        {
            if (!string.IsNullOrEmpty(alias))
            {
                table.SetAliasProperty(alias);
                return;
            }

            var tableName = table.SchemaAndTableName.TableName;
            var capitals = new string(tableName.Where(c => char.IsUpper(c)).ToArray());

#pragma warning disable CA1308 // Normalize strings to uppercase
            table.SetAliasProperty( capitals.Length > 0 ? capitals.ToLowerInvariant()
                : alias ?? tableName.Substring(0, 1).ToLowerInvariant());
#pragma warning restore CA1308 // Normalize strings to uppercase
        }

        private static void SetupDeclaredTable(SqlTable table)
        {
            AddDeclaredColumns(table);
            AddDeclaredForeignKeys(table);
            UpdateDeclaredIndexes(table);
            UpdateDeclaredCustomProperties(table);
        }

        private static void AddDeclaredColumns(SqlTable table)
        {
            var properties = table.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => typeof(SqlColumn).IsAssignableFrom(pi.PropertyType) && !pi.GetIndexParameters().Any());

            foreach (var property in properties)
            {
                var column = (SqlColumn)property.GetValue(table);
                column.Name = property.Name;
                column.Table = table;
            }
        }

        private static void AddDeclaredForeignKeys(SqlTable table)
        {
            var properties = table.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi =>
                    typeof(ForeignKey).IsAssignableFrom(pi.PropertyType)
                    && !pi.GetIndexParameters().Any());

            foreach (var property in properties)
            {
                var fk = (ForeignKey)property.GetValue(table);

                if (!property.Name.StartsWith('_'))
                    fk.Name = property.Name;

                fk.SqlTable = table;
            }
        }

        private static void UpdateDeclaredIndexes(SqlTable table)
        {
            var properties = table.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi =>
                    (typeof(Index).IsAssignableFrom(pi.PropertyType)
                    || typeof(UniqueConstraint).IsAssignableFrom(pi.PropertyType))
                    && !pi.GetIndexParameters().Any());

            foreach (var property in properties)
            {
                var index = (IndexBase)property.GetValue(table);

                if (!property.Name.StartsWith('_'))
                    index.Name = property.Name;

                index.SqlTable = table;

                var registeredIdexes = index.SqlColumns.OfType<ColumnAndOrderRegistration>().ToList();

                // TODO ?
                foreach (var cr in registeredIdexes)
                {
                    index.SqlColumns.Remove(cr);
                    index.SqlColumns.Add(new ColumnAndOrder(table.Columns[cr.ColumnName], cr.Order));
                }
            }
        }

        private static void UpdateDeclaredCustomProperties(SqlTable table)
        {
            var properties = table.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi =>
                    typeof(SqlTableCustomProperty).IsAssignableFrom(pi.PropertyType)
                    && !pi.GetIndexParameters().Any());

            // TODO ?
            foreach (var property in properties)
            {
                var customProperty = (SqlTableCustomProperty)property.GetValue(table);
                customProperty.SqlTable = table;
            }
        }
    }

    public class QueryElement
    {
        public SqlTable Table { get; set; }
        public List<QueryColumn> QueryColumns { get; set; }

        public QueryElement(SqlTable sqlTable, params QueryColumn[] columns)
        {
            Table = sqlTable;
            QueryColumns = columns.ToList();
        }

        public QueryElement(SqlTable sqlTable, string alias, params QueryColumn[] columns)
            : this(sqlTable, columns)
        {
            if(alias == null || Table.GetAlias() != alias)
                Table = sqlTable.Alias(alias);

            /*if (string.IsNullOrEmpty(Table.Alias))
                SqlTableExtension.SetAlias(Table, alias);
            else
                Table = sqlTable.Clone(alias);*/
        }
    }
}
