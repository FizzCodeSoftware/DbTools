using System;
using System.Collections.Generic;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.Common.Logger;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinitionReader;
using FizzCode.LightWeight;
using FizzCode.DbTools.SqlExecuter.MsSql;
using FizzCode.DbTools.DataDefinition.Base.Interfaces;

namespace FizzCode.DbTools.DataDefinition.MsSql2016;
public class MsSql2016DataDefinitionReader(NamedConnectionString connectionString, ContextWithLogger context, ISchemaNamesToRead? schemaNames)
    : GenericDataDefinitionReader(new MsSql2016Executer(context, connectionString, new MsSql2016Generator(context)), schemaNames)
{
    public override DatabaseDefinition GetDatabaseDefinition()
    {
        return GetDatabaseDefinition(new DatabaseDefinition(MsSqlVersion.MsSql2016, GenericVersion.Generic1));
    }

    public override DatabaseDefinition GetDatabaseDefinition(IDatabaseDefinition dd)
    {
        var dd2 = (DatabaseDefinition)dd;
        Log(LogSeverity.Debug, "Reading table definitions from database.");

        var module = "Reader/" + Executer.Generator.SqlVersion.UniqueName;
        var logTimer = new LogTimer(Logger, LogSeverity.Debug, "Reading definitions from database.", module);

        foreach (var schemaAndTableName in GetSchemaAndTableNames())
            dd2.AddTable(GetTableDefinition(schemaAndTableName, false));

        Log(LogSeverity.Debug, "Reading table documentation from database.");
        AddTableDocumentation(dd2);
        Log(LogSeverity.Debug, "Reading table identities from database.");
        new MsSqlIdentityReader2016(Executer, SchemaNames).GetIdentity(dd2);
        Log(LogSeverity.Debug, "Reading table indexes including primary keys and unique constraints from database.");
        new MsSqlIndexReader2016(Executer, SchemaNames).GetIndexes(dd2);
        Log(LogSeverity.Debug, "Reading table foreign keys from database.", "Reader");
        new MsSqlForeignKeyReader2016(Executer, SchemaNames).GetForeignKeys(dd2);

        Log(LogSeverity.Debug, "Reading views from database.");
        foreach (var schemaAndTableName in GetViews())
            dd2.AddView(GetViewDefinition(schemaAndTableName, false));

        Log(LogSeverity.Debug, "Reading view indexes from database.");
        new MsSqlViewIndexReader2016(Executer, SchemaNames).GetIndexes(dd2);

        logTimer.Done();

        return dd2;
    }

    private static string SqlTables(char typeFilter)
    {
        return @$"
SELECT ss.name schemaName, so.name tableName FROM sys.objects so
INNER JOIN sys.schemas ss ON ss.schema_id = so.schema_id
WHERE type = '{typeFilter}'";
    }

    public override List<SchemaAndTableName> GetSchemaAndTableNames()
    {
        var sqlStatement = SqlTables('U');

        AddSchemaNamesFilter(ref sqlStatement, "ss.name");

        return Executer.ExecuteQuery(sqlStatement).ConvertAll(row => new SchemaAndTableName(row.GetAs<string>("schemaName"), row.GetAs<string>("tableName")!))
;
    }

    public override List<SchemaAndTableName> GetViews()
    {
        var sqlStatement = SqlTables('V');

        AddSchemaNamesFilter(ref sqlStatement, "ss.name");

        return Executer.ExecuteQuery(sqlStatement)
            .ConvertAll(row => new SchemaAndTableName(row.GetAs<string>("schemaName"), row.GetAs<string>("tableName")!))
;
    }

    private MsSqlTableReader2016 _tableReader = null!;
    private MsSqlTableReader2016 TableReader => _tableReader ??= new MsSqlTableReader2016(Executer, SchemaNames);

    private MsSqlColumnDocumentationReader2016 _columnDocumentationReader = null!;
    private MsSqlColumnDocumentationReader2016 ColumnDocumentationReader => _columnDocumentationReader ??= new MsSqlColumnDocumentationReader2016(Executer);

    public override SqlView GetViewDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition = true)
    {
        var sqlView = TableReader.GetViewDefinition(schemaAndTableName);

        if (fullDefinition)
        {
            // TODO
            // AddTableDocumentation(sqlView);
        }

        // TODO
        // ColumnDocumentationReader.GetColumnDocumentation(sqlView);

        sqlView.SchemaAndTableName = GetSchemaAndTableNameAsToStore(sqlView.SchemaAndTableNameSafe, Executer.Context);
        return sqlView;
    }

    public override SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition = true)
    {
        var sqlTable = TableReader.GetTableDefinition(schemaAndTableName);

        if (fullDefinition)
        {
            new MsSqlIndexReader2016(Executer, SchemaNames).GetPrimaryKey((SqlTable)sqlTable);
            new MsSqlForeignKeyReader2016(Executer, SchemaNames).GetForeignKeys((SqlTable)sqlTable);
            AddTableDocumentation(sqlTable);
        }

        ColumnDocumentationReader.GetColumnDocumentation(sqlTable);

        sqlTable.SchemaAndTableName = GetSchemaAndTableNameAsToStore(sqlTable.SchemaAndTableNameSafe, Executer.Context);
        return (SqlTable)sqlTable;
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
        var rowSet = Executer.ExecuteQuery(new SqlStatementWithParameters(
        SqlGetTableDocumentation + " AND SCHEMA_NAME(t.schema_id) = @SchemaName AND t.name = @TableName", table.SchemaAndTableNameSafe.Schema, table.SchemaAndTableNameSafe.TableName));

        foreach (var row in rowSet)
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
        var rowSet = Executer.ExecuteQuery(@"
SELECT
    SCHEMA_NAME(t.schema_id) as SchemaName,
    t.name AS TableName, 
    p.value AS Property
FROM
    sys.tables AS t
    INNER JOIN sys.extended_properties AS p ON p.major_id = t.object_id AND p.minor_id = 0 AND p.class = 1
    AND p.name = 'MS_Description'");

        var tables = dd.GetTables();

        foreach (var row in rowSet)
        {
            // TODO SchemaAndTableName.Schema might be null on default schema?
            var table = tables.Find(t => t.SchemaAndTableNameSafe.Schema == row.GetAs<string>("SchemaName") && t.SchemaAndTableNameSafe.TableName == row.GetAs<string>("TableName"));
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
