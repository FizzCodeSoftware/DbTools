namespace FizzCode.DbTools.DataDefinitionReader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class MsSqlDataDefinitionReader : GenericDataDefinitionReader
    {
        public MsSqlDataDefinitionReader(SqlExecuter sqlExecuter)
            : base(sqlExecuter)
        {
        }

        public override DatabaseDefinition GetDatabaseDefinition()
        {
            var dd = new DatabaseDefinition();

            foreach (var schemaAndTableName in GetSchemaAndTableNames())
                dd.AddTable(GetTableDefinition(schemaAndTableName, false));

            AddTableDocumentation(dd);

            new MsSqlIdentityReader(Executer).GetIdentity(dd);
            new MsSqlPrimaryKeyReader(Executer).GetPrimaryKey(dd);
            new MsSqlForeignKeyReader(Executer).GetForeignKeys(dd);

            return dd;
        }

        public override List<SchemaAndTableName> GetSchemaAndTableNames()
        {
            return Executer.ExecuteQuery(@"
SELECT ss.name schemaName, so.name tableName FROM sys.objects so
INNER JOIN sys.schemas ss ON ss.schema_id = so.schema_id
WHERE type = 'U'").Rows
                .Select(row => new SchemaAndTableName(row.GetAs<string>("schemaName"), row.GetAs<string>("tableName")))
                .ToList();
        }

        private MsSqlTableReader _tableReader;
        private MsSqlTableReader TableReader => _tableReader ?? (_tableReader = new MsSqlTableReader(Executer));

        private MsSqlColumnDocumentationReader _columnDocumentationReader;
        private MsSqlColumnDocumentationReader ColumnDocumentationReader => _columnDocumentationReader ?? (_columnDocumentationReader = new MsSqlColumnDocumentationReader(Executer));

        public override SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition = true)
        {
            var sqlTable = TableReader.GetTableDefinition(schemaAndTableName);

            if (fullDefinition)
            {
                new MsSqlPrimaryKeyReader(Executer).
                GetPrimaryKey(sqlTable);
                new MsSqlForeignKeyReader(Executer).GetForeignKeys(sqlTable);
                AddTableDocumentation(sqlTable);
            }

            var defaultSchema = Executer.Generator.Settings.SqlDialectSpecificSettings.GetAs<string>("DefaultSchema");
            ColumnDocumentationReader.GetColumnDocumentation(defaultSchema, sqlTable);

            return sqlTable;
        }

        private readonly string SqlGetTableDocumentation = @"
SELECT
    t.name TableName, 
    p.value Property
FROM
    sys.tables AS t
    INNER JOIN sys.extended_properties AS p ON p.major_id = t.object_id AND p.minor_id = 0 AND p.class = 1
    WHERE p.name = 'MS_Description'";

        public void AddTableDocumentation(SqlTable table)
        {
            var reader = Executer.ExecuteQuery(new SqlStatementWithParameters(
            SqlGetTableDocumentation + " AND SCHEMA_NAME(t.schema_id) = @SchemaName AND t.name = @TableName", table.SchemaAndTableName.Schema, table.SchemaAndTableName.TableName));

            foreach (var row in reader.Rows)
            {
                var description = row.GetAs<string>("Property");
                if (!string.IsNullOrEmpty(description))
                {
                    description = description.Replace("\\n", "\n", StringComparison.OrdinalIgnoreCase).Trim();
                    var descriptionProperty = new SqlTableDescription(table, description);
                    table.Properties.Add(descriptionProperty);
                }
            }
        }

        public void AddTableDocumentation(DatabaseDefinition dd)
        {
            var reader = Executer.ExecuteQuery(@"
SELECT
    SCHEMA_NAME(t.schema_id) as SchemaName,
    t.name AS TableName, 
    p.value AS Property
FROM
    sys.tables AS t
    INNER JOIN sys.extended_properties AS p ON p.major_id = t.object_id AND p.minor_id = 0 AND p.class = 1
    AND p.name = 'MS_Description'");

            var tables = dd.GetTables();

            foreach (var row in reader.Rows)
            {
                // TODO SchemaAndTableName.Schema might be null on default schema?
                var table = tables.Find(t => t.SchemaAndTableName.Schema == row.GetAs<string>("SchemaName") && t.SchemaAndTableName.TableName == row.GetAs<string>("TableName"));
                if (table != null)
                {
                    var description = row.GetAs<string>("Property");
                    if (!string.IsNullOrEmpty(description))
                    {
                        description = description.Replace("\\n", "\n", StringComparison.OrdinalIgnoreCase).Trim();
                        var descriptionProperty = new SqlTableDescription(table, description);
                        table.Properties.Add(descriptionProperty);
                    }
                }
            }
        }
    }
}
