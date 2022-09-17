namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.SqlExecuter;

    public class MsSqlIndexReader2016 : GenericDataDefinitionElementReader
    {
        private List<Row> _queryResult;

        private List<Row> QueryResult => _queryResult ??= Executer.ExecuteQuery(GetKeySql()).Rows
                        .OrderBy(row => row.GetAs<string>("schema_name"))
                        .ThenBy(row => row.GetAs<string>("index_name"))
                        .ThenBy(row => row.GetAs<int>("index_column_id"))
                        .ToList();

        public MsSqlIndexReader2016(SqlStatementExecuter executer, SchemaNamesToRead schemaNames)
            : base(executer, schemaNames)
        {
        }

        public void GetIndexes(DatabaseDefinition dd)
        {
            foreach (var table in dd.GetTables())
            {
                GetPrimaryKey(table);
                GetUniqueConstraints(table);
                GetIndexes(table);
            }
        }

        public void GetPrimaryKey(SqlTable table)
        {
            PrimaryKey pk = null;

            var rows = QueryResult
                .Where(row => row.GetAs<bool>("is_primary_key") && DataDefinitionReaderHelper.SchemaAndTableNameEquals(row, table)).OrderBy(row => row.GetAs<int>("index_column_id"));

            foreach (var row in rows)
            {
                if (row.GetAs<int>("index_column_id") == 1)
                {
                    pk = new PrimaryKey(table, row.GetAs<string>("index_name"))
                    {
                        Clustered = row.GetAs<byte>("type") == 1,
                    };

                    table.Properties.Add(pk);
                }

                var column = table.Columns[row.GetAs<string>("column_name")];

                var ascDesc = row.GetAs<bool>("is_descending_key")
                    ? AscDesc.Desc
                    : AscDesc.Asc;

                pk.SqlColumns.Add(new ColumnAndOrder(column, ascDesc));
            }
        }

        public void GetUniqueConstraints(SqlTable table)
        {
            UniqueConstraint uniqueConstraint = null;
            var rows = QueryResult
                .Where(row => row.GetAs<bool>("is_unique_constraint") && DataDefinitionReaderHelper.SchemaAndTableNameEquals(row, table)).OrderBy(row => row.GetAs<int>("index_column_id"));

            foreach (var row in rows)
            {
                if (row.GetAs<int>("index_column_id") == 1)
                {
                    uniqueConstraint = new UniqueConstraint(table, row.GetAs<string>("index_name"))
                    {
                        Clustered = row.GetAs<byte>("type") == 1 // in MsSql, UCs are also Indexes
                    };

                    table.Properties.Add(uniqueConstraint);
                }

                var column = table.Columns[row.GetAs<string>("column_name")];

                var ascDesc = row.GetAs<bool>("is_descending_key")
                    ? AscDesc.Desc
                    : AscDesc.Asc;

                uniqueConstraint.SqlColumns.Add(new ColumnAndOrder(column, ascDesc));
            }
        }

        public void GetIndexes(SqlTable table)
        {
            Index index = null;
            /*var rowsX = QueryResult
                .Where(row => DataDefinitionReaderHelper.SchemaAndTableNameEquals(row, table, "schema_name", "view_name")).OrderBy(row => row.GetAs<string>("index_name")).ThenBy(row => row.GetAs<int>("index_column_id"));*/

            var rows = QueryResult
                .Where(row => !row.GetAs<bool>("is_primary_key") && !row.GetAs<bool>("is_unique_constraint") && DataDefinitionReaderHelper.SchemaAndTableNameEquals(row, table)).OrderBy(row => row.GetAs<string>("index_name")).ThenBy(row => row.GetAs<int>("index_column_id"));

            foreach (var row in rows)
            {
                if (row.GetAs<int>("index_column_id") == 1)
                {
                    index = new Index(table, row.GetAs<string>("index_name"))
                    {
                        Unique = row.GetAs<bool>("is_unique"),
                        Clustered = row.GetAs<byte>("type") == 1,
                    };

                    table.Properties.Add(index);
                }

                var column = table.Columns[row.GetAs<string>("column_name")];

                if (row.GetAs<bool>("is_included_column"))
                {
                    index.Includes.Add(column);
                }
                else
                {
                    var ascDesc = row.GetAs<bool>("is_descending_key")
                        ? AscDesc.Desc
                        : AscDesc.Asc;

                    index.SqlColumns.Add(new ColumnAndOrder(column, ascDesc));
                }
            }
        }

        private static string GetKeySql()
        {
            return @"
SELECT SCHEMA_NAME(tab.schema_id) schema_name
    , i.[name] index_name
    , ic.index_column_id
    , col.[name] as column_name
    , tab.[name] as table_name
	, i.type -- 1 CLUSTERED, 2 NONCLUSTERED
	, is_unique, is_primary_key
	, is_included_column, is_descending_key
    , i.is_unique_constraint
FROM sys.tables tab
    INNER JOIN sys.indexes i
        ON tab.object_id = i.object_id 
    INNER JOIN sys.index_columns ic
        ON ic.object_id = i.object_id
        and ic.index_id = i.index_id
    INNER JOIN sys.columns col
        ON i.object_id = col.object_id
        and col.column_id = ic.column_id";
        }
    }
}
