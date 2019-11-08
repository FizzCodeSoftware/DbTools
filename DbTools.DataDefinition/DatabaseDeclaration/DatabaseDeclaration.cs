namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class DatabaseDeclaration : DatabaseDefinition
    {
        public NamingStrategies NamingStrategies { get; }
        public const char SchemaTableNameSeparator = 'ꜗ';
        public string DefaultSchema { get; }

        protected DatabaseDeclaration(string defaultSchema = null, NamingStrategies namingStrategies = null)
        {
            DefaultSchema = defaultSchema;
            NamingStrategies = namingStrategies ?? new NamingStrategies();

            AddDeclaredTables();
            CreateRegisteredForeignKeys();
            AddAutoNaming(GetTables());
        }

        private IEnumerable<T> GetProperties<T>(SqlTable sqlTable)
        {
            return sqlTable.Properties.OfType<T>().ToList();
        }

        private void CreateRegisteredForeignKeys()
        {
            foreach (var sqlTable in Tables)
            {
                foreach (var fkRegistration in GetProperties<ForeignKeyRegistrationToTableWithPrimaryKeySingleColumn>(sqlTable))
                {
                    if (DefaultSchema != null && fkRegistration.ReferredTableName != null && string.IsNullOrEmpty(fkRegistration.ReferredTableName.Schema))
                        fkRegistration.ReferredTableName.Schema = DefaultSchema;

                    RegisteredForeignKeysCreator.PrimaryKeySingleColum(this, sqlTable, fkRegistration);
                }

                foreach (var fkRegistration in GetProperties<ForeignKeyRegistrationToTableWithPrimaryKey>(sqlTable))
                {
                    if (DefaultSchema != null && fkRegistration.ReferredTableName != null && string.IsNullOrEmpty(fkRegistration.ReferredTableName.Schema))
                        fkRegistration.ReferredTableName.Schema = DefaultSchema;

                    RegisteredForeignKeysCreator.PrimaryKey(this, sqlTable, fkRegistration, NamingStrategies.ForeignKey);
                }

                foreach (var fkRegistration in GetProperties<ForeignKeyRegistrationToTableWithPrimaryKeyExistingColumn>(sqlTable))
                {
                    if (DefaultSchema != null && fkRegistration.ReferredTableName != null && string.IsNullOrEmpty(fkRegistration.ReferredTableName.Schema))
                        fkRegistration.ReferredTableName.Schema = DefaultSchema;

                    RegisteredForeignKeysCreator.PrimaryKeyExistingColumn(this, sqlTable, fkRegistration);
                }

                foreach (var fkRegistration in GetProperties<ForeignKeyRegistrationToReferredTableExistingColumns>(sqlTable))
                {
                    if (DefaultSchema != null && fkRegistration.ReferredTableName != null && string.IsNullOrEmpty(fkRegistration.ReferredTableName.Schema))
                        fkRegistration.ReferredTableName.Schema = DefaultSchema;

                    RegisteredForeignKeysCreator.ReferredTableExistingColumns(this, sqlTable, fkRegistration);
                }

                foreach (var fkRegistration in GetProperties<ForeignKeyRegistrationToReferredTable>(sqlTable))
                {
                    if (DefaultSchema != null && fkRegistration.ReferredTableName != null && string.IsNullOrEmpty(fkRegistration.ReferredTableName.Schema))
                        fkRegistration.ReferredTableName.Schema = DefaultSchema;

                    RegisteredForeignKeysCreator.ReferredTable(this, sqlTable, fkRegistration);
                }
            }
        }

        public void AddAutoNaming(List<SqlTable> tables)
        {
            foreach (var sqlTable in tables)
            {
                foreach (var pk in sqlTable.Properties.OfType<PrimaryKey>().Where(pk => string.IsNullOrEmpty(pk.Name)))
                {
                    NamingStrategies.PrimaryKey.SetPrimaryKeyName(pk);
                }

                foreach (var index in sqlTable.Properties.OfType<Index>().Where(idx => string.IsNullOrEmpty(idx.Name)))
                {
                    NamingStrategies.Index.SetIndexName(index);
                }

                foreach (var fk in sqlTable.Properties.OfType<ForeignKey>().Where(fk => string.IsNullOrEmpty(fk.Name)))
                {
                    NamingStrategies.ForeignKey.SetFKName(fk);
                }
            }

            CircularFKDetector.DectectCircularFKs(Tables.ToList());
        }

        private void AddDeclaredTables()
        {
            var properties = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => pi.PropertyType == typeof(SqlTable));

            foreach (var property in properties)
            {
                var table = (SqlTable)property.GetValue(this);

                if (table.SchemaAndTableName == null)
                {
                    var schemaAndTableName = new SchemaAndTableName(property.Name);
                    if (string.IsNullOrEmpty(schemaAndTableName.Schema) && !string.IsNullOrEmpty(DefaultSchema))
                    {
                        schemaAndTableName.Schema = DefaultSchema;
                    }

                    table.SchemaAndTableName = schemaAndTableName;
                }

                AddTable(table);
            }

            var fields = GetType()
                .GetFields()
                .Where(fi => fi.IsPublic && fi.FieldType == typeof(SqlTable))
                .ToList();

            if (fields.Count > 0)
            {
                throw new InvalidOperationException(nameof(DatabaseDeclaration) + " is only compatible with tabled defined in public properties. Please review the following fields: " + string.Join(", ", fields.Select(fi => fi.Name)));
            }
        }

        protected static SqlTable AddTable(Action<SqlTable> configurator)
        {
            var table = new SqlTable();
            configurator.Invoke(table);
            return table;
        }
    }
}