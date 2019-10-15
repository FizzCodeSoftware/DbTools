namespace FizzCode.DbTools.DataDefinitionReader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;

    public class SqLiteDataDefinitionReader : GenericDataDefinitionReader
    {
        public SqLiteDataDefinitionReader(SqlExecuter sqlExecuter)
            : base(sqlExecuter)
        {
        }

        public override DatabaseDefinition GetDatabaseDefinition()
        {
            var dd = new DatabaseDefinition();

            foreach (var tableName in GetSchemaAndTableNames())
                dd.AddTable(GetTableDefinition(tableName, false));

            // TODO
            /*
            foreach (var table in dd.GetTables().Values)
                GetPrimaryKey(table);

            foreach (var table in dd.GetTables().Values)
                GetForeignKeys(table, dd);
                */

            return dd;
        }

        public override List<SchemaAndTableName> GetSchemaAndTableNames()
        {
            // TODO this is not working in in memory mode?
            return _executer
                .ExecuteQuery("SELECT name FROM sqlite_master WHERE type = 'table'").Rows
                .Select(row => new SchemaAndTableName(row.GetAs<string>("name")))
                .ToList();
        }

        public override SqlTable GetTableDefinition(SchemaAndTableName schemaAndTableName, bool fullDefinition = true)
        {
            var sqlTable = new SqlTable(schemaAndTableName);

            // TODO not working / not possible with in memory
            var reader = _executer.ExecuteQuery($"PRAGMA table_info({schemaAndTableName.TableName}");
            // cid, name, type, notnull, dflt_value, pk
            // type in the form varchar(20)

            foreach (var row in reader.Rows)
            {
                var type = MapSqlType(row.GetAs<string>("type"));
                var column = CreateSqlColumn(type, row);

                column.Table = sqlTable;

                sqlTable.Columns.Add(column.Name, column);
            }

            if (fullDefinition)
            {
                // TODO
                // GetPrimaryKey(sqlTable);
                // GetForeignKeys(sqlTable, null);
            }

            return sqlTable;
        }

        private SqlColumn CreateSqlColumn(SqlType type, Row row)
        {
            var column = new SqlColumn();
            switch (type)
            {
                case SqlType.Decimal:
                    column.Precision = row.GetAs<int>("NUMERIC_SCALE");
                    column.Length = row.GetAs<byte>("NUMERIC_PRECISION");
                    break;
                case SqlType.Int16:
                case SqlType.Int32:
                case SqlType.Int64:
                    column.Length = row.GetAs<byte>("NUMERIC_PRECISION");
                    break;
                case SqlType.Char:
                case SqlType.Varchar:
                case SqlType.NChar:
                case SqlType.NVarchar:
                    column.Length = row.GetAs<int>("CHARACTER_MAXIMUM_LENGTH");
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

        private SqlType MapSqlType(string type)
        {
            return type switch
            {
                "int" => SqlType.Int32,
                "smallint" => SqlType.Int16,
                "decimal" => SqlType.Decimal,
                "nvarchar" => SqlType.NVarchar,
                "nchar" => SqlType.NChar,
                _ => throw new NotImplementedException($"Unmapped SqlType: {type}."),
            };
        }
    }
}
