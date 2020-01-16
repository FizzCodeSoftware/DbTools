namespace FizzCode.DbTools.DataDefinitionReader
{
    using System;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;

    public class OracleTableReader12c
    {
        private readonly SqlExecuter _executer;
        private ILookup<string, Row> _queryResult;
        private ILookup<string, Row> QueryResult => _queryResult ?? (_queryResult = _executer.ExecuteQuery(GetStatement()).Rows.ToLookup(x => x.GetAs<string>("SCHEMAANDTABLENAME")));

        public OracleTableReader12c(SqlExecuter sqlExecuter)
        {
            _executer = sqlExecuter;
        }

        public SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName)
        {
            var sqlTable = new SqlTable(schemaAndTableName);

            var rows = QueryResult[schemaAndTableName.SchemaAndName]
                .OrderBy(r => r.GetAs<decimal>("COLUMN_ID"));

            foreach (var row in rows)
            {
                var type = MapSqlType(row.GetAs<string>("DATA_TYPE"), row);
                var column = CreateSqlColumn(type, row);

                column.Table = sqlTable;

                sqlTable.Columns.Add(column.Name, column);
            }

            return sqlTable;
        }

        private static SqlType MapSqlType(string type, Row row)
        {
            switch(type)
            {
                case "NUMBER":
                    {
                        if(row.GetAs<decimal>("DATA_SCALE") == 0
                           && row.GetAs<decimal?>("DATA_PRECISION") == 20)
                        {
                            return SqlType.Int64;
                        }

                        if (row.GetAs<decimal>("DATA_SCALE") == 0
                           && row.GetAs<decimal?>("DATA_PRECISION") == 10)
                        {
                            return SqlType.Int32;
                        }

                        if (row.GetAs<decimal>("DATA_SCALE") == 0
                          && row.GetAs<decimal?>("DATA_PRECISION") == 5)
                        {
                            return SqlType.Int16;
                        }

                        return SqlType.Decimal;
                    }
                case "FLOAT":
                    {
                        if (row.GetAs<decimal>("DATA_SCALE") == 0
                           && row.GetAs<decimal?>("DATA_PRECISION") == 53)
                        {
                            return SqlType.Double;
                        }

                        if (row.GetAs<decimal>("DATA_SCALE") == 0
                           && row.GetAs<decimal?>("DATA_PRECISION") == 24)
                        {
                            return SqlType.Single;
                        }

                        return SqlType.Double;
                    }
                case "NVARCHAR":
                    {
                        return SqlType.NVarchar;
                    }
                default:
                    throw new NotImplementedException($"Unmapped SqlType: {type}.");
            }
        }

        private static SqlColumn CreateSqlColumn(SqlType type, Row row)
        {
            SqlColumn column;
            switch (type)
            {
                case SqlType.Decimal:
                case SqlType.Money:
                    column = new SqlColumn
                    {
                        Precision = row.GetAs<int>("DATA_SCALE"),
                        Length = row.GetAs<byte>("DATA_PRECISION")
                    };
                    break;
                /*case SqlType.Int16:
                case SqlType.Int32:
                case SqlType.Int64:
                    column = new SqlColumn
                    {
                        Length = row.GetAs<byte>("NUMERIC_PRECISION")
                    };
                    break;*/
                case SqlType.DateTimeOffset:
                    column = new SqlColumn
                    {
                        Precision = row.GetAs<short>("DATA_PRECISION"),
                    };
                    break;
                case SqlType.Char:
                case SqlType.Varchar:
                case SqlType.NChar:
                case SqlType.NVarchar:
                    column = new SqlColumn
                    {
                        Length = row.GetAs<int>("CHARACTER_MAXIMUM_LENGTH")
                    };
                    break;
                default:
                    column = new SqlColumn();
                    break;
            }

            column.Name = row.GetAs<string>("COLUMN_NAME");
            column.Type = type;

            if (row.GetAs<string>("IS_NULLABLE") == "YES")
                column.IsNullable = true;

            return column;
        }

        private static string GetStatement()
        {
            return @"
SELECT CONCAT(CONCAT(owner, '.'), table_name) SchemaAndTableName,
  column_id, column_name, data_type
  /*, char_length*/, char_col_decl_length, data_precision, data_scale, nullable
  FROM all_tab_columns
 WHERE table_name IN (
	SELECT t.table_name
	FROM dba_tables t, dba_users u 
	WHERE t.owner = u.username
	AND EXISTS (SELECT 1 FROM dba_objects o
	WHERE o.owner = u.username ) AND default_tablespace not in
	('SYSTEM','SYSAUX') and ACCOUNT_STATUS = 'OPEN'
    )";
        }
    }
}