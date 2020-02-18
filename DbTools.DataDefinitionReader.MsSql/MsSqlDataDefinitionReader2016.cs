namespace FizzCode.DbTools.DataDefinitionReader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Common.Logger;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;
    using FizzCode.DbTools.DataDefinitionGenerator;

    public class MsSqlDataDefinitionReader2016 : GenericDataDefinitionReader
    {
        public MsSqlDataDefinitionReader2016(ConnectionStringWithProvider connectionStringWithProvider, Context context, List<string> schemaNames = null)
            : base(new MsSqlExecuter2016(connectionStringWithProvider, new MsSqlGenerator2016(context)), schemaNames)
        {
        }

        public override DatabaseDefinition GetDatabaseDefinition()
        {
            var dd = new DatabaseDefinition(Executer.ConnectionStringWithProvider.SqlEngineVersion, SqlVersions.Generic1);

            Log(LogSeverity.Debug, "Reading table definitions from database.");

            foreach (var schemaAndTableName in GetSchemaAndTableNames())
                dd.AddTable(GetTableDefinition(schemaAndTableName, false));

            Log(LogSeverity.Debug, "Reading table documentetion from database.");
            AddTableDocumentation(dd);

            Log(LogSeverity.Debug, "Reading table identities from database.");
            new MsSqlIdentityReader2016(Executer).GetIdentity(dd);
            Log(LogSeverity.Debug, "Reading table primary keys from database.");
            new MsSqlPrimaryKeyReader2016(Executer).GetPrimaryKey(dd);
            Logger.Log(LogSeverity.Debug, "Reading table foreign keys from database.", "Reader");
            new MsSqlForeignKeyReader2016(Executer).GetForeignKeys(dd);

            return dd;
        }

        public override List<SchemaAndTableName> GetSchemaAndTableNames()
        {
            var sqlStatement = @"
SELECT ss.name schemaName, so.name tableName FROM sys.objects so
INNER JOIN sys.schemas ss ON ss.schema_id = so.schema_id
WHERE type = 'U'";
            if (SchemaNames != null)
            {
                var schemaNames = SchemaNames;
                if (Executer.Generator.Context.Settings.Options.ShouldUseDefaultSchema)
                    schemaNames.Add(Executer.Generator.Context.Settings.SqlVersionSpecificSettings.GetAs<string>("DefaultSchema"));

                sqlStatement += $" AND ss.name IN({string.Join(',', schemaNames.Select(s => "'" + s + "'").ToList())})";
            }

            return Executer.ExecuteQuery(sqlStatement).Rows
                .Select(row => new SchemaAndTableName(row.GetAs<string>("schemaName"), row.GetAs<string>("tableName")))
                .ToList();
        }

        private MsSqlTableReader2016 _tableReader;
        private MsSqlTableReader2016 TableReader => _tableReader ?? (_tableReader = new MsSqlTableReader2016(Executer));

        private MsSqlColumnDocumentationReader2016 _columnDocumentationReader;
        private MsSqlColumnDocumentationReader2016 ColumnDocumentationReader => _columnDocumentationReader ?? (_columnDocumentationReader = new MsSqlColumnDocumentationReader2016(Executer));

        public override SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition = true)
        {
            var sqlTable = TableReader.GetTableDefinition(schemaAndTableName);

            if (fullDefinition)
            {
                new MsSqlPrimaryKeyReader2016(Executer).
                GetPrimaryKey(sqlTable);
                new MsSqlForeignKeyReader2016(Executer).GetForeignKeys(sqlTable);
                AddTableDocumentation(sqlTable);
            }

            ColumnDocumentationReader.GetColumnDocumentation(sqlTable);

            sqlTable.SchemaAndTableName = GetSchemaAndTableNameAsToStore(sqlTable.SchemaAndTableName, Executer.Generator.Context);
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
