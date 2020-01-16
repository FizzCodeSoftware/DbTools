namespace FizzCode.DbTools.DataDefinitionReader
{
    using System;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;

    public abstract class TypeMapper
    {
        protected TypeMapper(SqlTypeInfos sqlTypeInfos)
        {
            SqlTypeInfos = sqlTypeInfos;
        }

        protected SqlTypeInfos SqlTypeInfos { get;  }

        public virtual SqlType MapSqlType(string type, bool IsNullable, int length = 0, int scale = 0)
        {
            var typeName = type.ToUpper();
            SqlTypeInfos.TryGetValue(typeName, out var sqlTypeInfo);

            if (sqlTypeInfo == null)
                throw new NotImplementedException($"Unmapped SqlType: {type}.");

            var sqlType = new SqlType
            {
                Length = length,
                Scale = scale,
                IsNullable = IsNullable,
                SqlTypeInfo = sqlTypeInfo
            };

            return sqlType;
        }
    }

    public class MsSqlTypeMapper2016TypeMapper : TypeMapper
    {
        public MsSqlTypeMapper2016TypeMapper() : base(MsSqlInfo.Get(new MsSql2016()))
        {
        }

        public SqlType MapSqlType(string type, bool isNullable, int numericPrecision, int numericSale, int characterMaximumLength, int datetimePrecision)
        {
            switch (type.ToUpper())
            {
                case "CHAR":
                case "NCHAR":
                case "VARCHAR": // TODO max length allowed - what is in Row?
                case "NVARCHAR": // TODO max length allowed - what is in Row?
                    return MapSqlType(type, isNullable, characterMaximumLength);
                case "BIT":
                case "TINYINT":
                case "SMALLINT":
                case "INT":
                case "BIGINT":
                    return MapSqlType(type, isNullable);
                case "DECIMAL":
                case "NUMERIC":
                    return MapSqlType(type, isNullable, numericPrecision, numericSale);
                case "MONEY":
                case "SMALLMONEY":
                    return MapSqlType(type, isNullable);
                case "FLOAT":
                case "REAL":
                    return MapSqlType(type, isNullable);
                case "DATE":
                case "DATETIME":
                case "SMALLDATETIME":
                    return MapSqlType(type, isNullable);
                case "TIME":
                case "DATETIME2":
                case "DATETIMEOFFSET":
                    return MapSqlType(type, isNullable, datetimePrecision);
                case "BINARY":
                    return MapSqlType(type, isNullable, characterMaximumLength); // TODO which length?
                case "VARBINARY": // TODO max length allowed - what is in Row?
                    return MapSqlType(type, isNullable, characterMaximumLength); // TODO which length?
                case "IMAGE":
                case "XML":
                    return MapSqlType(type, isNullable);
                default:
                    throw new NotImplementedException($"Unmapped SqlType: {type}.");
            }
        }
    }

    public class OracleTypeMapper12c : TypeMapper
    {
        public OracleTypeMapper12c() : base(MsSqlInfo.Get(new MsSql2016()))
        {
        }

        public SqlType MapSqlType(string type, bool isNullable, int dataPrecision, int dataScale)
        {
            // TODO VARCHAR2(20 BYTE) VS VARCHAR2(20 CHAR)

            switch (type.ToUpper())
            {
                case "CHAR":
                case "NCHAR":
                case "VARCHAR":
                case "NVARCHAR":
                case "VARCHAR2":
                case "NVARCHAR2":
                    return MapSqlType(type, isNullable, dataPrecision);

                case "BLOB":
                case "CLOB":
                case "NCLOB":
                case "BFILE":
                case "LONG": // TODO handle deprecated
                case "LONG RAW": // TODO handle deprecated
                    return MapSqlType(type, isNullable, dataPrecision);

                case "NUMBER":
                    return MapSqlType(type, isNullable, dataPrecision, dataScale);

                case "BINARY_FLOAT":
                case "BINARY_DOUBLE":
                    return MapSqlType(type, isNullable);

                case "DATE":
                case "TIMESTAMP WITH TIME ZONE":
                case "TIMESTAMP WITH LOCAL TIME ZONE":
                    return MapSqlType(type, isNullable);

                case "XMLTYPE":
                case "URITYPE":
                    return MapSqlType(type, isNullable);

                default:
                    throw new NotImplementedException($"Unmapped SqlType: {type}.");
            }
        }
    }

    public class SqLiteTypeMapper : TypeMapper
    {
        public SqLiteTypeMapper() : base(MsSqlInfo.Get(new MsSql2016()))
        {
        }
    }

    public class MsSqlTableReader2016
    {
        private readonly SqlExecuter _executer;
        private ILookup<string, Row> _queryResult;
        private ILookup<string, Row> QueryResult => _queryResult ?? (_queryResult = _executer.ExecuteQuery(GetStatement()).Rows.ToLookup(x => x.GetAs<string>("SchemaAndTableName")));

        protected MsSqlTypeMapper2016TypeMapper TypeMapper { get; } = new MsSqlTypeMapper2016TypeMapper();

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

                var numericPrecision = row.GetAs<byte>("NUMERIC_PRECISION");
                var numericScale = row.GetAs<int>("NUMERIC_SCALE");
                var characterMaximumLength = row.GetAs<int>("CHARACTER_MAXIMUM_LENGTH");
                var dateTimePrecision = row.GetAs<short>("DATETIME_PRECISION");

                var isNullable = row.GetAs<string>("IS_NULLABLE") == "YES";

                var sqlType = TypeMapper.MapSqlType(type, isNullable, numericPrecision, numericScale, characterMaximumLength, dateTimePrecision);

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