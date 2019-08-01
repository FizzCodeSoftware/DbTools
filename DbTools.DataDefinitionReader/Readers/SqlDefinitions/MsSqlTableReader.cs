namespace FizzCode.DbTools.DataDefinitionReader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;

    public class MsSqlTableReader
    {
        public MsSqlTableReader(SqlExecuter sqlExecuter)
        {
            _executer = sqlExecuter;
        }

        protected readonly SqlExecuter _executer;

        private List<Row> _queryResult;
        private List<Row> QueryResult {
            get
            {
                if (_queryResult == null)
                {
                    var reader = _executer.ExecuteQuery($@"
SELECT TABLE_NAME, TABLE_SCHEMA, ORDINAL_POSITION, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE
       , IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
--WHERE TABLE_NAME = ''
--ORDER BY ORDINAL_POSITION");

                    _queryResult = reader.Rows;
                }

                return _queryResult;
            }
        }

        
        public SqlTable GetTableDefinition(string tableName)
        {
            var sqlTable = new SqlTable(tableName);

            foreach (var row in QueryResult.Where(r => r.GetAs<string>("TABLE_NAME") == tableName).OrderBy(r => r.GetAs<int>("ORDINAL_POSITION")))
            {
                var type = MapSqlType(row.GetAs<string>("DATA_TYPE"));
                var column = CreateSqlColumn(type, row);

                column.Table = sqlTable;

                sqlTable.Columns.Add(column.Name, column);
            }

            return sqlTable;
        }

        private SqlType MapSqlType(string type)
        {
            switch (type)
            {
                case "int":
                    return SqlType.Int32;
                case "smallint":
                    return SqlType.Int16;
                case "tinyint":
                    return SqlType.Byte;
                case "bigint":
                    return SqlType.Int64;
                case "decimal":
                    return SqlType.Decimal;
                case "nvarchar":
                    return SqlType.NVarchar;
                case "nchar":
                    return SqlType.NChar;
                case "varchar":
                    return SqlType.Varchar;
                case "char":
                    return SqlType.Char;
                case "datetime":
                    return SqlType.DateTime;
                case "date":
                    return SqlType.Date;
                case "bit":
                    return SqlType.Boolean;
                case "float":
                    return SqlType.Double;
                case "xml":
                    return SqlType.Xml;
                case "uniqueidentifier":
                    return SqlType.Guid;
                case "binary":
                    return SqlType.Binary;
                case "image":
                    return SqlType.Image;
                case "varbinary":
                    return SqlType.VarBinary;
                case "ntext":
                    return SqlType.NText;
                default:
                    throw new NotImplementedException($"Unmapped SqlType: {type}.");
            }
        }

        private SqlColumn CreateSqlColumn(SqlType type, Row row)
        {
            SqlColumn column;
            switch (type)
            {
                case SqlType.Decimal:
                    column = new SqlColumn
                    {
                        Precision = row.GetAs<int>("NUMERIC_SCALE"),
                        Length = row.GetAs<byte>("NUMERIC_PRECISION")
                    };
                    break;
                case SqlType.Int16:
                case SqlType.Int32:
                case SqlType.Int64:
                    column = new SqlColumn
                    {
                        Length = row.GetAs<byte>("NUMERIC_PRECISION")
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
    }
}
