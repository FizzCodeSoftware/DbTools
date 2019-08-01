namespace FizzCode.DbTools.DataDefinitionReader
{
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
                dd.AddTable( GetTableDefinition(tableName, false));

            AddTableDocumentation(dd);

            foreach (var table in dd.GetTables())
                GetPrimaryKey(table);

            var fks = new MsSqlForeignKeyReader(_executer);
            fks.GetForeignKeys(dd);

            return dd;
        }

        public override List<string> GetTableNames()
        {
            var reader = _executer.ExecuteQuery("SELECT name FROM sysobjects WHERE xtype = 'U'");
            return reader.GetRows<string>().ToList();
        }

        private MsSqlTableReader _tableReader;
        private MsSqlTableReader TableReader
        {
            get
            {
                if (_tableReader == null)
                    _tableReader = new MsSqlTableReader(_executer);

                return _tableReader;
            }
        }

        public override SqlTable GetTableDefinition(string tableName, bool fullDefinition = true)
        {
            var sqlTable = TableReader.GetTableDefinition(tableName);

            if (fullDefinition)
            {
                GetPrimaryKey(sqlTable);
                new MsSqlForeignKeyReader(_executer).GetForeignKeys(sqlTable, null);
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
