namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using System.Linq;
    using System.Reflection;

    public class DatabaseDeclaration : DatabaseDefinition
    {
        public NamingStrategiesDictionary NamingStrategies { get; }
        public const char SchemaTableNameSeparator = 'ꜗ';

        public DatabaseDeclaration()
        {
            NamingStrategies = new NamingStrategiesDictionary();

            AddDeclaredTables();
            AddAutoNaming();
        }

        public DatabaseDeclaration(params INamingStrategy[] namingStrategies)
        {
            NamingStrategies = new NamingStrategiesDictionary(namingStrategies);

            AddDeclaredTables();
            AddAutoNaming();
        }

        private void AddAutoNaming()
        {
            var pkNaming = NamingStrategies.GetNamingStrategy<IPrimaryKeyNamingStrategy>();
            var indexNaming = NamingStrategies.GetNamingStrategy<IIndexNamingStrategy>();
            var fkNaming = NamingStrategies.GetNamingStrategy<IForeignKeyNamingStrategy>();

            foreach (var sqlTable in Tables)
            {
                var properties = sqlTable.Properties.OfType<ForeignKeyToPrimaryKey>().ToList();
                foreach (var pkfk in properties)
                {
                    var referredTable = pkfk.ReferredTable;
                    var referredPk = referredTable.Properties.OfType<PrimaryKey>().FirstOrDefault();
                    if (referredPk == null)
                        throw new Exception("Can't define ForeignKeyToPrimaryKey against a table without primary key!");

                    sqlTable.Properties.Remove(pkfk);
                    var fk = new ForeignKey(sqlTable, referredTable.SchemaAndTableName, pkfk.Name);
                    sqlTable.Properties.Add(fk);

                    foreach (var pkColumn in referredPk.SqlColumns.OrderBy(x => x.Order).Select(x => x.SqlColumn))
                    {
                        var col = new SqlColumn();
                        pkColumn.CopyTo(col);

                        col.Table = sqlTable;
                        col.IsNullable = pkfk.IsNullable;

                        if (pkfk.Map == null)
                        {
                            col.Name = fkNaming != null
                                ? fkNaming.GetFkToPkColumnName(pkColumn, pkfk.NamePrefix)
                                : pkfk.NamePrefix + referredTable.SchemaAndTableName.SchemaAndName + pkColumn.Name;
                        }
                        else
                        {
                            col.Name = pkfk.Map.Find(x => x.ReferredColumn == pkColumn.Name).ColumnName;
                            if (string.IsNullOrEmpty(col.Name))
                                throw new Exception("ForeignKeyToPrimaryKey map does not match to the target table's primary key!");
                        }

                        sqlTable.Columns.Add(col.Name, col);

                        fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fk, col, pkColumn.Name));
                    }
                }
            }

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
