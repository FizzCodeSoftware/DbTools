namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class DatabaseDeclaration : DatabaseDefinition
    {
        public NamingStrategiesDictionary NamingStrategies { get; }

        public DatabaseDeclaration()
        {
            NamingStrategies = new NamingStrategiesDictionary();
            Tables = GetDeclaredTables();
        }

        public const char SchemaTableNameSeparator = 'ꜗ';

        public DatabaseDeclaration(params INamingStrategy[] namingStrategies)
        {
            NamingStrategies = new NamingStrategiesDictionary(namingStrategies);
            Tables = GetDeclaredTables();
        }

        private Tables GetDeclaredTables()
        {
            var tables = new Tables();

            var properties = GetType().GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public).Where(fi => fi.FieldType == typeof(LazySqlTable));

            var lazySqlTables = new List<LazySqlTable>();
            foreach (var property in properties)
            {
                var layzSqlTable = (LazySqlTable)property.GetValue(this);
                layzSqlTable.SetLazyProperties(SchemaAndTableNameFromDefinitionName(property.Name), this);

                lazySqlTables.Add(layzSqlTable);
            }

            foreach (var lazySqlTable in lazySqlTables)
            {
                var sqlTable = lazySqlTable.SqlTable;
                tables.Add(sqlTable);
            }

            foreach (var lazySqlTable in lazySqlTables)
            {
                foreach (var lazyForeignKey in lazySqlTable.SqlTable.LazyForeignKeys())
                {
                    lazySqlTable.SqlTable.Columns.Remove(lazyForeignKey.Name);
                    lazySqlTable.SqlTable.AddForeignKey(lazyForeignKey.ReferredTable, lazyForeignKey.ColumnNamePrefix, lazyForeignKey.IsNullable);
                }
            }

            DelayedNaming(tables);

            return tables;
        }

        private void DelayedNaming(Tables tables)
        {
            foreach (var table in tables)
            {
                var tableDeclaration = table as SqlTableDeclaration;
                foreach (var delayedNaming in tableDeclaration.DelayedNamingTasks)
                {
                    delayedNaming.Resolve(NamingStrategies);
                }
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
    }
}
