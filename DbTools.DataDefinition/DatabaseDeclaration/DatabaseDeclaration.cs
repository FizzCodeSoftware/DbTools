namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FizzCode.DbTools.QueryBuilder.Interface;

    public class DatabaseDeclaration : DatabaseDefinition
    {
        public NamingStrategies NamingStrategies { get; }
        public const char SchemaTableNameSeparator = 'ꜗ';
        public string DefaultSchema { get; }

        protected DatabaseDeclaration(IQueryBuilder queryBuilder, AbstractTypeMapper mainTypeMapper, AbstractTypeMapper[] secondaryTypeMappers = null, string defaultSchema = null, NamingStrategies namingStrategies = null)
            : base(mainTypeMapper, secondaryTypeMappers)
        {
            DefaultSchema = defaultSchema;
            NamingStrategies = namingStrategies ?? new NamingStrategies();

            QueryBuilder = queryBuilder;

            AddDeclaredTables();
            AddDeclaredStoredProcedures();
            CreateRegisteredForeignKeys();
            AddAutoNaming(GetTables());
            CircularFKDetector.DectectCircularFKs(GetTables());
        }

        protected DatabaseDeclaration(AbstractTypeMapper mainTypeMapper, AbstractTypeMapper[] secondaryTypeMappers = null, string defaultSchema = null, NamingStrategies namingStrategies = null)
            : base(mainTypeMapper, secondaryTypeMappers)
        {
            DefaultSchema = defaultSchema;
            NamingStrategies = namingStrategies ?? new NamingStrategies();

            AddDeclaredTables();
            AddDeclaredStoredProcedures();
            CreateRegisteredForeignKeys();
            AddAutoNaming(GetTables());
            CircularFKDetector.DectectCircularFKs(GetTables());
        }

        public IQueryBuilder QueryBuilder { get; }

        private static IEnumerable<T> GetProperties<T>(SqlTable sqlTable)
        {
            return sqlTable.Properties.OfType<T>().ToList();
        }

        private void CreateRegisteredForeignKeys()
        {
            foreach (var sqlTable in Tables)
            {
                CreateRegisteredForeignKeys(sqlTable);
            }
        }

        public void CreateRegisteredForeignKeys(SqlTable sqlTable)
        {
            foreach (var fkRegistration in GetProperties<ForeignKeyRegistrationToTableWithUniqueKeySingleColumn>(sqlTable))
            {
                if (DefaultSchema != null && fkRegistration.ReferredTableName != null && string.IsNullOrEmpty(fkRegistration.ReferredTableName.Schema))
                    fkRegistration.ReferredTableName.Schema = DefaultSchema;

                RegisteredForeignKeysCreator.UniqueKeySingleColumn(this, sqlTable, fkRegistration);
            }

            foreach (var fkRegistration in GetProperties<ForeignKeyRegistrationToTableWithUniqueKey>(sqlTable))
            {
                if (DefaultSchema != null && fkRegistration.ReferredTableName != null && string.IsNullOrEmpty(fkRegistration.ReferredTableName.Schema))
                    fkRegistration.ReferredTableName.Schema = DefaultSchema;

                RegisteredForeignKeysCreator.UniqueKey(this, sqlTable, fkRegistration, NamingStrategies.ForeignKey);
            }

            foreach (var fkRegistration in GetProperties<ForeignKeyRegistrationToTableWithUniqueKeyExistingColumn>(sqlTable))
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

                foreach (var uniqueConstraint in sqlTable.Properties.OfType<UniqueConstraint>().Where(uc => string.IsNullOrEmpty(uc.Name)))
                {
                    NamingStrategies.UniqueConstraint.SetUniqueConstraintName(uniqueConstraint);
                }

                foreach (var fk in sqlTable.Properties.OfType<ForeignKey>().Where(fk => string.IsNullOrEmpty(fk.Name)))
                {
                    NamingStrategies.ForeignKey.SetFKName(fk);
                }
            }
        }

        private void AddDeclaredTables()
        {
            var properties = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => typeof(SqlTable).IsAssignableFrom(pi.PropertyType));

            foreach (var property in properties)
            {
                var table = (SqlTable)property.GetValue(this);

                if (table.SchemaAndTableName == null)
                {
                    var schemaAndTableName = new SchemaAndTableName(property.Name);
                    if (string.IsNullOrEmpty(schemaAndTableName.Schema) && !string.IsNullOrEmpty(DefaultSchema))
                        schemaAndTableName.Schema = DefaultSchema;

                    table.SchemaAndTableName = schemaAndTableName;
                }

                table.DatabaseDefinition = this;
                SetupDeclaredTable(table);
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

                var tablePlaceHolderProperties = column.Table.Properties;

                foreach (var tablePlaceHolderProperty in tablePlaceHolderProperties)
                {
                    tablePlaceHolderProperty.SqlTable = table;
                    AddTablePlaceHolderProperty(table, column, tablePlaceHolderProperty);
                }

                column.Table = table;

                table.Columns.Add(column);
            }
        }

        private static void AddTablePlaceHolderProperty(SqlTable table, SqlColumn column, SqlTableProperty tablePlaceHolderProperty)
        {
            if (tablePlaceHolderProperty is PrimaryKey pk
                && table.HasProperty<PrimaryKey>())
            {
                table.SetPK(column, pk.SqlColumns[0].Order, pk.Name);
            }
            else
            {
                table.Properties.Add(tablePlaceHolderProperty);
            }
        }

        private void AddDeclaredStoredProcedures()
        {
            var properties = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => typeof(StoredProcedure).IsAssignableFrom(pi.PropertyType));

            foreach (var property in properties)
            {
                var sp = (StoredProcedure)property.GetValue(this);

                if (sp is StoredProcedureFromQuery spq)
                    sp = new StoredProcedure(QueryBuilder.Build(spq.Query), spq.SpParameters?.ToArray());

                if (sp.SchemaAndSpName == null)
                {
                    var schemaAndTableName = new SchemaAndTableName(property.Name);
                    if (string.IsNullOrEmpty(schemaAndTableName.Schema) && !string.IsNullOrEmpty(DefaultSchema))
                        schemaAndTableName.Schema = DefaultSchema;

                    sp.SchemaAndSpName = schemaAndTableName;
                }

                sp.DatabaseDefinition = this;

                StoredProcedures.Add(sp);
            }
        }
        protected static SqlTable AddTable(Action<SqlTable> configurator)
        {
            var table = new SqlTable();
            configurator.Invoke(table);
            return table;
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

                foreach (var cr in registeredIdexes)
                {
                    index.SqlColumns.Remove(cr);
                    index.SqlColumns.Add(new ColumnAndOrder(table.Columns[cr.ColumnName], cr.Order));
                }

                table.Properties.Add(index);
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

                table.Properties.Add(fk);
            }
        }

        private static void UpdateDeclaredCustomProperties(SqlTable table)
        {
            var properties = table.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi =>
                    typeof(SqlTableCustomProperty).IsAssignableFrom(pi.PropertyType)
                    && !pi.GetIndexParameters().Any());

            foreach (var property in properties)
            {
                var customProperty = (SqlTableCustomProperty)property.GetValue(table);
                customProperty.SqlTable = table;
                table.Properties.Add(customProperty);
            }
        }
    }
}