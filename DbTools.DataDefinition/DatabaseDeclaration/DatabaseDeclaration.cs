namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class DatabaseDeclaration : DatabaseDefinition
    {
        public NamingStrategiesDictionary NamingStrategies { get; }
        public const char SchemaTableNameSeparator = 'ꜗ';

        public DatabaseDeclaration() : this(new NamingStrategiesDictionary())
        {
        }

        public DatabaseDeclaration(params INamingStrategy[] namingStrategies) : this(new NamingStrategiesDictionary(namingStrategies))
        {
        }

        protected DatabaseDeclaration(NamingStrategiesDictionary namingStrategies)
        {
            NamingStrategies = namingStrategies;

            AddDeclaredTables();
            CreateRegisteredForeignKeys();
            AddAutoNaming();
        }

        private IEnumerable<T> GetProperties<T>(SqlTable sqlTable)
        {
            return sqlTable.Properties.OfType<T>().ToList();
        }

        private void CreateRegisteredForeignKeys()
        {
            var fkNaming = NamingStrategies.GetNamingStrategy<IForeignKeyNamingStrategy>();

            foreach (var sqlTable in Tables)
            {
                foreach (var fkRegistration in GetProperties<ForeignKeyRegistrationToTableWithPrimaryKeySingleColumn>(sqlTable))
                    RegisteredForeignKeysCreator.PrimaryKeySingleColum(Tables, sqlTable, fkRegistration);

                foreach (var fkRegistration in GetProperties<ForeignKeyRegistrationToTableWithPrimaryKey>(sqlTable))
                    RegisteredForeignKeysCreator.PrimaryKey(Tables, sqlTable, fkRegistration, fkNaming);

                foreach (var fkRegistration in
                    GetProperties<ForeignKeyRegistrationToTableWithPrimaryKeyExistingColumn>(sqlTable))
                    RegisteredForeignKeysCreator.PrimaryKeyExistingColumn(Tables, sqlTable, fkRegistration);

                foreach (var fkRegistration in GetProperties<ForeignKeyRegistrationToReferredTableExistingColumns>(sqlTable))
                    RegisteredForeignKeysCreator.ReferredTableExistingColumns(Tables, sqlTable, fkRegistration);

                foreach (var fkRegistration in GetProperties<ForeignKeyRegistrationToReferredTable>(sqlTable))
                    RegisteredForeignKeysCreator.ReferredTable(Tables, sqlTable, fkRegistration);
            }
        }

        private void AddAutoNaming()
        {
            var pkNaming = NamingStrategies.GetNamingStrategy<IPrimaryKeyNamingStrategy>();
            var indexNaming = NamingStrategies.GetNamingStrategy<IIndexNamingStrategy>();
            var fkNaming = NamingStrategies.GetNamingStrategy<IForeignKeyNamingStrategy>();

            foreach (var sqlTable in Tables)
            {
                if (pkNaming != null)
                {
                    foreach (var pk in sqlTable.Properties.OfType<PrimaryKey>().Where(pk => string.IsNullOrEmpty(pk.Name)))
                    {
                        pkNaming.SetPrimaryKeyName(pk);
                    }
                }

                if (indexNaming != null)
                {
                    foreach (var index in sqlTable.Properties.OfType<Index>().Where(idx => string.IsNullOrEmpty(idx.Name)))
                    {
                        indexNaming.SetIndexName(index);
                    }
                }

                if (fkNaming != null)
                {
                    foreach (var fk in sqlTable.Properties.OfType<ForeignKey>().Where(fk => string.IsNullOrEmpty(fk.Name)))
                    {
                        fkNaming.SetFKName(fk);
                    }
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
                    var schemaAndTableName = SchemaAndTableNameFromDefinitionName(property.Name);
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

        private SchemaAndTableName SchemaAndTableNameFromDefinitionName(string methodName)
        {
            var schemaAndTableName = methodName.Split(SchemaTableNameSeparator);

            if (schemaAndTableName.Length == 1)
                return new SchemaAndTableName(schemaAndTableName[0]);

            if (schemaAndTableName.Length == 2)
                return new SchemaAndTableName(schemaAndTableName[0], schemaAndTableName[1]);

            throw new ArgumentException("Method name contains invalid number of SchemaTableNameSeparator", nameof(methodName));
        }

        protected static SqlTable AddTable(Action<SqlTable> configurator)
        {
            var table = new SqlTable();
            configurator.Invoke(table);
            return table;
        }
    }
}
