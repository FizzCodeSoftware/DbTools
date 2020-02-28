namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Oracle12c;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;

    public class OracleDataDefinitionElementReader : GenericDataDefinitionElementReader
    {
        protected OracleDataDefinitionElementReader(SqlStatementExecuter executer, SchemaNamesToRead schemaNames)
            : base(executer, schemaNames)
        {
        }

        protected override void AddSchemaNamesFilter(ref string sqlStatement, string schemaColumnName)
        {
            var schemaNames = new List<string>();
            if (SchemaNames?.AllDefault != false)
            {
                if (Executer.Generator.Context.Settings.Options.ShouldUseDefaultSchema)
                    schemaNames.Add(Executer.Generator.Context.Settings.SqlVersionSpecificSettings.GetAs<string>("DefaultSchema"));
            }
            else
            {
                if (SchemaNames.AllNotSystem)
                {
                    sqlStatement += @"
AND EXISTS (SELECT 1 FROM dba_objects o
	WHERE o.owner = u.username ) AND u.default_tablespace not in
	('SYSTEM','SYSAUX') and u.ACCOUNT_STATUS = 'OPEN'";
                }

                if (!SchemaNames.All && SchemaNames.SchemaNames != null)
                {
                    schemaNames = SchemaNames.SchemaNames;
                }
            }

            if (schemaNames.Count > 0)
                sqlStatement += $" AND {schemaColumnName} IN({string.Join(',', schemaNames.Select(s => "'" + s + "'").ToList())})";
        }
    }

    public class OracleTableReader12c : OracleDataDefinitionElementReader
    {
        private readonly ILookup<string, Row> _queryResult;

        protected Oracle12cTypeMapper TypeMapper { get; } = new Oracle12cTypeMapper();

        public OracleTableReader12c(SqlStatementExecuter executer, SchemaNamesToRead schemaNames)
            : base(executer, schemaNames)
        {
            var sqlStatement = GetStatement();
            AddSchemaNamesFilter(ref sqlStatement, "all_tab_columns.owner");
            _queryResult = Executer.ExecuteQuery(sqlStatement).Rows.ToLookup(x => x.GetAs<string>("SCHEMAANDTABLENAME"));
        }

        public SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName)
        {
            var sqlTable = new SqlTable(schemaAndTableName);

            var rows = _queryResult[schemaAndTableName.SchemaAndName]
                .OrderBy(r => r.GetAs<decimal>("COLUMN_ID"));

            foreach (var row in rows)
            {
                // char_col_decl_length

                var type = row.GetAs<string>("DATA_TYPE");
                var dataPrecisionDecimal = row.GetAs<decimal?>("DATA_PRECISION");
                var dataScaleDecimal = row.GetAs<decimal?>("DATA_SCALE");
                var dataPrecision = (int?)dataPrecisionDecimal;
                var dataScale = (int?)dataScaleDecimal;

                var charLengthDecimal = row.GetAs<decimal?>("CHAR_COL_DECL_LENGTH");
                var charLength = (int?)charLengthDecimal;

                var isNullable = row.GetAs<string>("NULLABLE") != "N";

                var sqlType = TypeMapper.MapSqlTypeFromReaderInfo(type, isNullable, charLength, dataPrecision, dataScale);

                var column = new SqlColumn
                {
                    Table = sqlTable
                };
                column.Types.Add(Executer.Generator.Version, sqlType);
                column.Name = row.GetAs<string>("COLUMN_NAME");

                sqlTable.Columns.Add(column.Name, column);
            }

            return sqlTable;
        }

        private static string GetStatement()
        {
            return @"
SELECT CONCAT(CONCAT(owner, '.'), table_name) SchemaAndTableName,
  column_id, column_name, data_type
  /*, char_length*/, char_col_decl_length, data_precision, data_scale, nullable
  FROM all_tab_columns, dba_users u
 WHERE all_tab_columns.owner = u.username";
        }
    }
}