namespace FizzCode.DbTools.DataDefinition
{
    using System;
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

        private void CreateRegisteredForeignKeys()
        {
            var fkNaming = NamingStrategies.GetNamingStrategy<IForeignKeyNamingStrategy>();

            foreach (var sqlTable in Tables)
            {
                var foreignKeyRegistrationsToTableWithPrimaryKeySingleColumn = sqlTable.Properties.OfType<ForeignKeyRegistrationToTableWithPrimaryKeySingleColumn>().ToList();
                foreach (var fkRegistration in foreignKeyRegistrationsToTableWithPrimaryKeySingleColumn)
                {
                    var referredTable = Tables[fkRegistration.ReferredTableName];
                    var referredPk = referredTable.Properties.OfType<PrimaryKey>().FirstOrDefault();
                    if (referredPk == null)
                        throw new Exception("Can't define ForeignKeyRegistrationToTableWithPrimaryKey against a table without primary key!");

                    sqlTable.Properties.Remove(fkRegistration);
                    var fk = new ForeignKey(sqlTable, referredTable.SchemaAndTableName, fkRegistration.Name);
                    sqlTable.Properties.Add(fk);

                    var pkColumn = referredPk.SqlColumns.First().SqlColumn;

                    var col = new SqlColumn();
                    pkColumn.CopyTo(col);

                    col.Table = sqlTable;
                    col.IsNullable = fkRegistration.IsNullable;
                    col.Name = fkRegistration.SingleFkColumnName;

                    fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fk, col, pkColumn.Name));
                }

                var foreignKeyRegistrationsToTableWithPrimaryKey = sqlTable.Properties.OfType<ForeignKeyRegistrationToTableWithPrimaryKey>().ToList();
                foreach (var fkRegistration in foreignKeyRegistrationsToTableWithPrimaryKey)
                {
                    var referredTable = Tables[fkRegistration.ReferredTableName];
                    var referredPk = referredTable.Properties.OfType<PrimaryKey>().FirstOrDefault();
                    if (referredPk == null)
                        throw new Exception("Can't define ForeignKeyRegistrationToTableWithPrimaryKey against a table without primary key!");

                    sqlTable.Properties.Remove(fkRegistration);
                    var fk = new ForeignKey(sqlTable, referredTable.SchemaAndTableName, fkRegistration.Name);
                    sqlTable.Properties.Add(fk);

                    foreach (var pkColumn in referredPk.SqlColumns.Select(x => x.SqlColumn))
                    {
                        var col = new SqlColumn();
                        pkColumn.CopyTo(col);

                        col.Table = sqlTable;
                        col.IsNullable = fkRegistration.IsNullable;

                        col.Name = fkNaming.GetFkToPkColumnName(pkColumn, fkRegistration.NamePrefix);

                        sqlTable.Columns.Add(col.Name, col);

                        fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fk, col, pkColumn.Name));
                    }
                }

                var foreignKeyRegistrationsToTableWithPrimaryKeyExistingColumn = sqlTable.Properties.OfType<ForeignKeyRegistrationToTableWithPrimaryKeyExistingColumn>().ToList();
                foreach (var fkRegistration in foreignKeyRegistrationsToTableWithPrimaryKeyExistingColumn)
                {
                    var referredTable = Tables[fkRegistration.ReferredTableName];
                    var referredPk = referredTable.Properties.OfType<PrimaryKey>().FirstOrDefault();
                    if (referredPk == null)
                        throw new Exception("Can't define ForeignKeyRegistrationToTableWithPrimaryKey against a table without primary key!");

                    sqlTable.Properties.Remove(fkRegistration);
                    var fk = new ForeignKey(sqlTable, referredTable.SchemaAndTableName, fkRegistration.Name);
                    sqlTable.Properties.Add(fk);

                    var pkColumn = referredPk.SqlColumns.First().SqlColumn;
                    fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fk, fkRegistration.SingleFkColumn, pkColumn.Name));
                }

                var foreignKeyRegistrationToReferredTableExistingColumns = sqlTable.Properties.OfType<ForeignKeyRegistrationToReferredTableExistingColumns>().ToList();
                foreach (var fkRegistration in foreignKeyRegistrationToReferredTableExistingColumns)
                {
                    var referredTable = Tables[fkRegistration.ReferredTableName];

                    sqlTable.Properties.Remove(fkRegistration);
                    var fk = new ForeignKey(sqlTable, referredTable.SchemaAndTableName, fkRegistration.Name);
                    sqlTable.Properties.Add(fk);

                    foreach (var fkGroup in fkRegistration.Map)
                    {
                        fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fk, sqlTable.Columns[fkGroup.ColumnName], fkGroup.ReferredColumnName));
                    }
                }

                var foreignKeyRegistrationsToReferredTable = sqlTable.Properties.OfType<ForeignKeyRegistrationToReferredTable>().ToList();
                foreach (var fkRegistration in foreignKeyRegistrationsToReferredTable)
                {
                    var referredTable = Tables[fkRegistration.ReferredTableName];

                    sqlTable.Properties.Remove(fkRegistration);
                    var fk = new ForeignKey(sqlTable, referredTable.SchemaAndTableName, fkRegistration.Name);
                    sqlTable.Properties.Add(fk);

                    foreach (var fkGroup in fkRegistration.Map)
                    {
                        var col = new SqlColumn();
                        referredTable.Columns[fkGroup.ReferredColumnName].CopyTo(col);

                        col.Table = sqlTable;
                        col.IsNullable = fkRegistration.IsNullable;

                        col.Name = fkGroup.ColumnName;

                        sqlTable.Columns.Add(col.Name, col);

                        fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fk, col, fkGroup.ReferredColumnName));
                    }
                }
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
