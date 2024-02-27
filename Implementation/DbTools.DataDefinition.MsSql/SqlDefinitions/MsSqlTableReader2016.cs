using System.Linq;
using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Base.Interfaces;
using FizzCode.DbTools.DataDefinition.MsSql2016;
using FizzCode.DbTools.SqlExecuter;

namespace FizzCode.DbTools.DataDefinitionReader;
public class MsSqlTableReader2016(SqlStatementExecuter executer, ISchemaNamesToRead schemaNames)
    : GenericDataDefinitionElementReader(executer, schemaNames)
{
    private ILookup<string, Row> _queryResult = null!;
    private ILookup<string, Row> QueryResult => _queryResult ??= Executer.ExecuteQuery(GetStatement()).ToLookup(x => x.GetAs<string>("SchemaAndTableName")!);

    protected MsSql2016TypeMapper TypeMapper { get; } = new MsSql2016TypeMapper();

    public SqlView GetViewDefinition(SchemaAndTableName schemaAndTableName)
    {
        var sqlView = new SqlView(schemaAndTableName);

        var rows = QueryResult[schemaAndTableName.SchemaAndName]
            .OrderBy(r => r.GetAs<int>("ORDINAL_POSITION"));

        foreach (var row in rows)
        {
            var sqlType = GetSqlTypeFromRow(row);

            var column = new SqlViewColumn
            {
                SqlTableOrView = sqlView
            };
            column.Types.Add(Executer.Generator.SqlVersion, sqlType);
            column.Name = Throw.IfNull(row.GetAs<string>("COLUMN_NAME"));

            sqlView.Columns.Add(column.Name, column);
        }

        return sqlView;
    }

    public SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName)
    {
        var sqlTable = new SqlTable(schemaAndTableName);

        var rows = QueryResult[schemaAndTableName.SchemaAndName]
            .OrderBy(r => r.GetAs<int>("ORDINAL_POSITION"));

        foreach (var row in rows)
        {
            var sqlType = GetSqlTypeFromRow(row);

            var column = new SqlColumn
            {
                Table = sqlTable
            };
            column.Types.Add(Executer.Generator.SqlVersion, sqlType);
            column.Name = row.GetAs<string>("COLUMN_NAME");
            AddDefaultValue(column, row);

            sqlTable.Columns.Add(column.Name!, column);
        }

        return sqlTable;
    }

    private SqlType GetSqlTypeFromRow(Row row)
    {
        var type = Throw.IfNull(row.GetAs<string>("DATA_TYPE"));

        var numericPrecision = row.GetAs<byte?>("NUMERIC_PRECISION") ?? 0;
        var numericScale = row.GetAs<int?>("NUMERIC_SCALE") ?? 0;
        var characterMaximumLength = row.GetAs<int?>("CHARACTER_MAXIMUM_LENGTH") ?? 0;
        var dateTimePrecision = row.GetAs<short?>("DATETIME_PRECISION") ?? 0;

        var isNullable = row.GetAs<string>("IS_NULLABLE") == "YES";

        var sqlType = TypeMapper.MapSqlTypeFromReaderInfo(type, isNullable, numericPrecision, numericScale, characterMaximumLength, dateTimePrecision);
        return sqlType;
    }

    private void AddDefaultValue(SqlColumn column, Row row)
    {
        var value = row.GetAs<string>("COLUMN_DEFAULT");
        if (value is not null)
        { 
            var name = Throw.IfNull(row.GetAs<string>("Constraint_Name"));
            column.Properties.Add(new DefaultValue(column, value, name));
        }
    }

    private static string GetStatement()
    {
        return @"
SELECT
    CONCAT(TABLE_SCHEMA, '.', TABLE_NAME) SchemaAndTableName,
    ORDINAL_POSITION, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE, IS_NULLABLE, DATETIME_PRECISION, COLUMN_DEFAULT, dc.Name as Constraint_Name
FROM
    INFORMATION_SCHEMA.COLUMNS c
LEFT JOIN sys.default_constraints dc ON dc.parent_object_id = OBJECT_ID(c. TABLE_SCHEMA + '.' + c.TABLE_NAME) 
AND c.COLUMN_DEFAULT = dc.definition";
    }
}