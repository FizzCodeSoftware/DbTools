namespace FizzCode.DbTools.DataDefinitionReader
{
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

            new MsSqlIdentityReader(_executer).GetIdentity(dd);
            new MsSqlPrimaryKeyReader(_executer).GetPrimaryKey(dd);
            new MsSqlForeignKeyReader(_executer).GetForeignKeys(dd);

            return dd;
        }

        public override List<SchemaAndTableName> GetSchemaAndTableNames()
        {
            var reader = _executer.ExecuteQuery(@"
SELECT ss.name as 'schema', so.name FROM sys.objects so
INNER JOIN sys.schemas ss ON ss.schema_id = so.schema_id
WHERE type = 'U'");
            return reader.GetRows<string, string>().Select((item) => new SchemaAndTableName(item.Item1, item.Item2)).ToList();
        }

        private MsSqlTableReader _tableReader;

        private MsSqlTableReader TableReader
        {
            get
            {
                return _tableReader ?? (_tableReader = new MsSqlTableReader(_executer));
            }
        }

        public override SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition = true)
        {
            var sqlTable = TableReader.GetTableDefinition(schemaAndTableName);

            if (fullDefinition)
            {
                new MsSqlPrimaryKeyReader(_executer).
                GetPrimaryKey(sqlTable);
                new MsSqlForeignKeyReader(_executer).GetForeignKeys(sqlTable, null);
                AddTableDocumentation(sqlTable);
            }

            AddColumnDocumentation(sqlTable);

            return sqlTable;
        }

        public void AddColumnDocumentation(SqlTable table)
        {
            var defaultSchema = _executer.GetSettings().SqlDialectSpecificSettings.GetAs<string>("DefaultSchema");

            var reader = _executer.ExecuteQuery(new SqlStatementWithParameters(@"
SELECT
    c.name ColumnName,
    p.value Property
FROM
    sys.tables t
    INNER JOIN sys.all_columns c ON c.object_id = t.object_id
    INNER JOIN sys.extended_properties p ON p.major_id = t.object_id AND p.minor_id = c.column_id AND p.class = 1
WHERE
    p.name = 'MS_Description'
    AND SCHEMA_NAME(t.schema_id) = @SchemaName
    AND t.name = @TableName", table.SchemaAndTableName.Schema ?? defaultSchema, table.SchemaAndTableName.TableName));

            foreach (var row in reader.Rows)
            {
                var column = table.Columns.FirstOrDefault(c => c.Key == row.GetAs<string>("ColumnName")).Value;
                if (column != null)
                {
                    var description = row.GetAs<string>("Property");
                    var descriptionProperty = new SqlColumnDescription(column, description);
                    column.Properties.Add(descriptionProperty);
                }
            }
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
            var reader = _executer.ExecuteQuery(new SqlStatementWithParameters(
            SqlGetTableDocumentation + " AND SCHEMA_NAME(t.schema_id) = @SchemaName AND t.name = @TableName", table.SchemaAndTableName.Schema, table.SchemaAndTableName.TableName));

            foreach (var row in reader.Rows)
            {
                var description = row.GetAs<string>("Property");
                var descriptionProperty = new SqlTableDescription(table, description);

                table.Properties.Add(descriptionProperty);
            }
        }

        public void AddTableDocumentation(DatabaseDefinition dd)
        {
            var reader = _executer.ExecuteQuery(@"
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
                    var descriptionProperty = new SqlTableDescription(table, description);

                    table.Properties.Add(descriptionProperty);
                }
            }
        }
    }
}
