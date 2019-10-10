namespace FizzCode.DbTools.DataDefinitionReader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;

    public class MsSqlTableReader
    {
        private readonly SqlExecuter _executer;
        private List<Row> _queryResult;
        private List<Row> QueryResult => _queryResult ?? (_queryResult = _executer.ExecuteQuery(GetStatement()).Rows);

        public MsSqlTableReader(SqlExecuter sqlExecuter)
        {
            _executer = sqlExecuter;
        }

        public SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName)
        {
            var sqlTable = new SqlTable(schemaAndTableName);

            foreach (var row in QueryResult.Where(r => r.GetAs<string>("TABLE_NAME") == schemaAndTableName.TableName
                && (string.IsNullOrEmpty(schemaAndTableName.Schema) || r.GetAs<string>("TABLE_SCHEMA") == schemaAndTableName.Schema)).OrderBy(r => r.GetAs<int>("ORDINAL_POSITION")))
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
            return type switch
            {
                "int" => SqlType.Int32,
                "smallint" => SqlType.Int16,
                "tinyint" => SqlType.Byte,
                "bigint" => SqlType.Int64,
                "decimal" => SqlType.Decimal,
                "money" => SqlType.Money,
                "nvarchar" => SqlType.NVarchar,
                "nchar" => SqlType.NChar,
                "varchar" => SqlType.Varchar,
                "char" => SqlType.Char,
                "datetime" => SqlType.DateTime,
                "datetimeoffset" => SqlType.DateTimeOffset,
                "date" => SqlType.Date,
                "bit" => SqlType.Boolean,
                "real" => SqlType.Single,
                "float" => SqlType.Double,
                "xml" => SqlType.Xml,
                "uniqueidentifier" => SqlType.Guid,
                "binary" => SqlType.Binary,
                "image" => SqlType.Image,
                "varbinary" => SqlType.VarBinary,
                "ntext" => SqlType.NText,
                _ => throw new NotImplementedException($"Unmapped SqlType: {type}."),
            };
        }

        private SqlColumn CreateSqlColumn(SqlType type, Row row)
        {
            SqlColumn column;
            switch (type)
            {
                case SqlType.Decimal:
                case SqlType.Money:
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
                case SqlType.DateTimeOffset:
                    column = new SqlColumn
                    {
                        Precision = row.GetAs<short>("DATETIME_PRECISION"),
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
SELECT TABLE_NAME, TABLE_SCHEMA, ORDINAL_POSITION, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE, IS_NULLABLE, DATETIME_PRECISION
FROM INFORMATION_SCHEMA.COLUMNS";
        }
    }
}