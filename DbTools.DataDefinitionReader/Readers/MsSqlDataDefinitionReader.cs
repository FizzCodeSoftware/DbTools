namespace FizzCode.DbTools.DataDefinitionReader
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;

    public class MsSqlDataDefinitionReader : GenericDataDefinitionReader
    {
        public MsSqlDataDefinitionReader(SqlExecuter sqlExecuter)
            : base(sqlExecuter)
        {
        }

        public override DatabaseDefinition GetDatabaseDefinition()
        {
            var dd = new DatabaseDefinition();

            foreach (var tableName in GetTableNames())
                dd.AddTable(GetTableDefinition(tableName, false));

            AddTableDocumentation(dd);

            foreach (var table in dd.GetTables())
                GetPrimaryKey(table);

            foreach (var table in dd.GetTables())
                GetForeignKeys(table, dd);

            return dd;
        }

        public override List<string> GetTableNames()
        {
            var reader = _executer.ExecuteQuery("SELECT name FROM sysobjects WHERE xtype = 'U'");
            return reader.GetRows<string>().ToList();
        }

        public override SqlTable GetTableDefinition(string tableName, bool fullDefinition = true)
        {
            var sqlTable = new SqlTable(tableName);

            var reader = _executer.ExecuteQuery($@"
SELECT TABLE_SCHEMA, ORDINAL_POSITION, COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, NUMERIC_SCALE
       , IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = '{tableName}'
ORDER BY ORDINAL_POSITION");

            foreach (var row in reader.Rows)
            {
                var type = MapSqlType(row.GetAs<string>("DATA_TYPE"));
                var column = CreateSqlColumn(type, row);

                column.Table = sqlTable;

                sqlTable.Columns.Add(column.Name, column);
            }

            if (fullDefinition)
            {
                GetPrimaryKey(sqlTable);
                GetForeignKeys(sqlTable, null);
                AddTableDocumentation(sqlTable);
            }

            AddColumnDocumentation(sqlTable);

            return sqlTable;
        }

        public void GetPrimaryKey(SqlTable table)
        {
            var reader = _executer.ExecuteQuery(GetKeySql(true, table.Name));
            var pk = new PrimaryKey(table, null);
            foreach (var row in reader.Rows)
            {
                if (row.GetAs<int>("index_column_id") == 1)
                {
                    pk = new PrimaryKey(table, row.GetAs<string>("index_name"));

                    if (row.GetAs<byte>("type") == 1)
                        pk.Clustered = true;

                    table.Properties.Add(pk);
                }

                var column = table.Columns[row.GetAs<string>("column_name")];

                if (row.GetAs<bool>("is_identity"))
                    column.Properties.Add(new Identity(column));

                var ascDesc = AscDesc.Asc;
                if (row.GetAs<bool>("is_descending_key"))
                    ascDesc = AscDesc.Desc;

                var columnAndOrder = new ColumnAndOrder(column, ascDesc);

                pk.SqlColumns.Add(columnAndOrder);
            }
        }

        private string GetKeySql(bool isPrimaryKey, string tableName)
        {
            return $@"
SELECT schema_name(tab.schema_id) schema_name, 
    i.[name] index_name,
    ic.index_column_id,
    col.[name] as column_name, 
    tab.[name] as table_name
	, i.type-- 1 CLUSTERED, 2 NONCLUSTERED
	, is_unique, is_primary_key, is_identity
	, is_included_column, is_descending_key
FROM sys.tables tab
    INNER JOIN sys.indexes i
        ON tab.object_id = i.object_id 
    INNER JOIN sys.index_columns ic
        ON ic.object_id = i.object_id
        and ic.index_id = i.index_id
    INNER JOIN sys.columns col
        ON i.object_id = col.object_id
        and col.column_id = ic.column_id
WHERE is_primary_key = {(isPrimaryKey ? 1 : 0)}
    AND tab.[name] = '{tableName}'
ORDER BY schema_name(tab.schema_id),
    i.[name],
    ic.index_column_id";
        }

        public DatabaseDefinition GetForeignKeys(SqlTable table, DatabaseDefinition dd)
        {
            var fakePKs = new Dictionary<string, SqlTable>();

            var reader = _executer.ExecuteQuery($@"
SELECT
     KCU1.CONSTRAINT_NAME AS FK_CONSTRAINT_NAME
    ,KCU1.TABLE_NAME AS FK_TABLE_NAME
    ,KCU1.COLUMN_NAME AS FK_COLUMN_NAME
    ,KCU1.ORDINAL_POSITION AS FK_ORDINAL_POSITION
    ,KCU2.CONSTRAINT_NAME AS REFERENCED_CONSTRAINT_NAME
    ,KCU2.TABLE_NAME AS REFERENCED_TABLE_NAME
    ,KCU2.COLUMN_NAME AS REFERENCED_COLUMN_NAME
    ,KCU2.ORDINAL_POSITION AS REFERENCED_ORDINAL_POSITION
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS AS RC

INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU1
    ON KCU1.CONSTRAINT_CATALOG = RC.CONSTRAINT_CATALOG
    AND KCU1.CONSTRAINT_SCHEMA = RC.CONSTRAINT_SCHEMA
    AND KCU1.CONSTRAINT_NAME = RC.CONSTRAINT_NAME

INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS KCU2
    ON KCU2.CONSTRAINT_CATALOG = RC.UNIQUE_CONSTRAINT_CATALOG
    AND KCU2.CONSTRAINT_SCHEMA = RC.UNIQUE_CONSTRAINT_SCHEMA
    AND KCU2.CONSTRAINT_NAME = RC.UNIQUE_CONSTRAINT_NAME
    AND KCU2.ORDINAL_POSITION = KCU1.ORDINAL_POSITION

WHERE KCU1.TABLE_NAME = '{table.Name}'
ORDER BY KCU1.ORDINAL_POSITION");

            foreach (var row in reader.Rows)
            {
                var fkColumn = table.Columns[row.GetAs<string>("FK_COLUMN_NAME")];

                var pkTableName = row.GetAs<string>("REFERENCED_TABLE_NAME");
                var pkColumnName = row.GetAs<string>("REFERENCED_COLUMN_NAME");
                var fkName = row.GetAs<string>("FK_CONSTRAINT_NAME");

                if (row.GetAs<int>("FK_ORDINAL_POSITION") == 1)
                {
                    PrimaryKey pk;
                    if (dd == null)
                    {
                        if (!fakePKs.ContainsKey(pkTableName))
                            fakePKs.Add(pkTableName, new SqlTable(row.GetAs<string>(pkTableName)));

                        pk = fakePKs[pkTableName].Properties.OfType<PrimaryKey>().First();
                    }
                    else
                    {
                        pk = dd.GetTable(pkTableName).Properties.OfType<PrimaryKey>().First();
                    }

                    var newFk = new ForeignKey(table, pk, fkName);
                    table.Properties.Add(newFk);
                }

                PrimaryKey pk2;
                if (dd == null)
                {
                    pk2 = fakePKs[pkTableName].Properties.OfType<PrimaryKey>().First();
                    var fakePkColumn = fkColumn.CopyTo(new SqlColumn());
                    fakePkColumn.Table = fakePKs[pkTableName];

                    fakePKs[pkTableName].Columns.Add(fakePkColumn.Name, fakePkColumn);

                    fakePKs[pkTableName].Properties.Add(new PrimaryKey(fakePKs[pkTableName], null));
                }
                else
                {
                    pk2 = dd.GetTable(pkTableName).Properties.OfType<PrimaryKey>().First();
                }

                var fk = table.Properties.OfType<ForeignKey>().First(fk1 => fk1.PrimaryKey.SqlTable.Name == pkTableName);
                fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fkColumn, pk2.SqlColumns.First(co => co.SqlColumn.Name == pkColumnName).SqlColumn));
            }

            return dd;
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

        public void AddColumnDocumentation(SqlTable table)
        {
            var reader = _executer.ExecuteQuery($@"
SELECT
    c.name ColumnName,
    p.value Property
FROM
    sys.tables t
    INNER JOIN sys.all_columns c ON c.object_id = t.object_id
    INNER JOIN sys.extended_properties p ON p.major_id = t.object_id AND p.minor_id = c.column_id AND p.class = 1
WHERE
    SCHEMA_NAME(t.schema_id) = 'dbo'
    AND t.name = '{table.Name}'
    AND p.name = 'MS_Description'");

            foreach (var row in reader.Rows)
            {
                var column = table.Columns.FirstOrDefault(c => c.Key == row.GetAs<string>("ColumnName")).Value;
                if (column != null)
                {
                    var description = row.GetAs<string>("Property");
                    var descriptionProperty = new SqlColumnDescription(column, description);
                    
                    column.Properties.Add(descriptionProperty);
                }
            }
        }

        private readonly string SqlGetTableDocumentation = @"
SELECT
    t.name TableName, 
    p.value Property
FROM
    sys.tables AS t
    INNER JOIN sys.extended_properties AS p ON p.major_id = t.object_id AND p.minor_id = 0 AND p.class = 1
    WHERE p.name = 'MS_Description'
    AND SCHEMA_NAME(t.schema_id) = 'dbo'";

        public void AddTableDocumentation(SqlTable table)
        {

            var reader = _executer.ExecuteQuery(
            SqlGetTableDocumentation + $" AND t.name='{table.Name}");

            foreach (var row in reader.Rows)
            {
                var description = row.GetAs<string>("Property");
                var descriptionProperty = new SqlTableDescription(table, description);

                table.Properties.Add(descriptionProperty);
            }
        }

        public void AddTableDocumentation(DatabaseDefinition dd)
        {
            var reader = _executer.ExecuteQuery($@"
SELECT
    t.name TableName, 
    p.value Property
FROM
    sys.tables AS t
    INNER JOIN sys.extended_properties AS p ON p.major_id = t.object_id AND p.minor_id = 0 AND p.class = 1
    -- WHERE SCHEMA_NAME(t.schema_id) = 'dbo'
    -- AND t.name=''
    AND p.name = 'MS_Description'");

            foreach (var row in reader.Rows)
            {
                var table = dd.GetTables().FirstOrDefault(t => t.Name == row.GetAs<string>("TableName"));
                if (table != null)
                {
                    var description = row.GetAs<string>("Property");
                    var descriptionProperty = new SqlTableDescription(table, description);

                    table.Properties.Add(descriptionProperty);
                }
            }
        }
    }
}
