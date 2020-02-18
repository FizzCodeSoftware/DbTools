namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;

    public class MsSqlTableReader2016
    {
        private readonly SqlExecuter _executer;
        private ILookup<string, Row> _queryResult;
        private ILookup<string, Row> QueryResult => _queryResult ?? (_queryResult = _executer.ExecuteQuery(GetStatement()).Rows.ToLookup(x => x.GetAs<string>("SchemaAndTableName")));

        protected MsSqlTypeMapper2016 TypeMapper { get; } = new MsSqlTypeMapper2016();

        public MsSqlTableReader2016(SqlExecuter sqlExecuter)
        {
            _executer = sqlExecuter;
        }

        public SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName)
        {
            var sqlTable = new SqlTable(schemaAndTableName);

            var rows = QueryResult[schemaAndTableName.SchemaAndName]
                .OrderBy(r => r.GetAs<int>("ORDINAL_POSITION"));

            foreach (var row in rows)
            {
                var type = row.GetAs<string>("DATA_TYPE");

                var numericPrecision = row.GetAs<byte?>("NUMERIC_PRECISION") ?? 0;
                var numericScale = row.GetAs<int?>("NUMERIC_SCALE") ?? 0;
                var characterMaximumLength = row.GetAs<int?>("CHARACTER_MAXIMUM_LENGTH") ?? 0;
                var dateTimePrecision = row.GetAs<short?>("DATETIME_PRECISION") ?? 0;

                var isNullable = row.GetAs<string>("IS_NULLABLE") == "YES";

                var sqlType = TypeMapper.MapSqlTypeFromReaderInfo(type, isNullable, numericPrecision, numericScale, characterMaximumLength, dateTimePrecision);

                var column = new SqlColumn
                {
                    Table = sqlTable
                };
                column.Types.Add(_executer.Generator.Version, sqlType);
                column.Name = row.GetAs<string>("COLUMN_NAME");

                sqlTable.Columns.Add(column.Name, column);
            }

            return sqlTable;
        }

        private static string GetStatement()
        {
            return @"
SELECT
    CONCAT(TABLE_SCHEMA, '.', TABLE_NAME) SchemaAndTableName,
    ORDINAL_POSITION, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE, IS_NULLABLE, DATETIME_PRECISION
FROM
    INFORMATION_SCHEMA.COLUMNS";
        }
    }
}