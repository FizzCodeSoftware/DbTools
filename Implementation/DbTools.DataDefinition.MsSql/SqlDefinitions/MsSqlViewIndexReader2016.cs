namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.Base.Interfaces;
    using FizzCode.DbTools.SqlExecuter;

    public class MsSqlViewIndexReader2016 : GenericDataDefinitionElementReader
    {
        private const string Is_primary_key = "is_primary_key";
        private const string Is_unique_constraint = "is_unique_constraint";
        private const string Index_name = "index_name";
        private const string Index_column_id = "index_column_id";

        private List<Row> _queryResult;

        private List<Row> QueryResult => _queryResult ??= Executer.ExecuteQuery(GetKeySql()).Rows
                        .OrderBy(row => row.GetAs<string>("schema_name"))
                        .ThenBy(row => row.GetAs<string>(Index_name))
                        .ThenBy(row => row.GetAs<int>(Index_column_id))
                        .ToList();

        public MsSqlViewIndexReader2016(SqlStatementExecuter executer, ISchemaNamesToRead schemaNames)
            : base(executer, schemaNames)
        {
        }

        public void GetIndexes(DatabaseDefinition dd)
        {
            foreach (var view in dd.GetViews())
            {
                GetIndexes(view);
            }
        }

        public void GetIndexes(SqlView view)
        {
            // TODO
        }

        public void GetIndexes(SqlTable table)
        {
            Index index = null;
            var rows = QueryResult
                .Where(row => !row.GetAs<bool>(Is_primary_key) && !row.GetAs<bool>(Is_unique_constraint) && DataDefinitionReaderHelper.SchemaAndTableNameEquals(row, table))
                .OrderBy(row => row.GetAs<string>(Index_name)).ThenBy(row => row.GetAs<int>(Index_column_id))
                .ToList();

            foreach (var row in rows)
            {
                if (row.GetAs<int>("index_column_id") == 1)
                {
                    index = new Index(table, row.GetAs<string>(Index_name))
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
SELECT SCHEMA_NAME(v.schema_id) AS schema_name
    , i.[name] index_name
    , ic.index_column_id
    , col.[name] as column_name
	, v.name as view_name
	, i.type -- 1 CLUSTERED, 2 NONCLUSTERED
	, is_unique, is_primary_key
	, is_included_column, is_descending_key
    , i.is_unique_constraint
FROM sys.views v
	INNER JOIN sys.indexes i
		ON i.object_id = v.object_id
    INNER JOIN sys.index_columns ic
        ON ic.object_id = i.object_id
        and ic.index_id = i.index_id
    INNER JOIN sys.columns col
        ON i.object_id = col.object_id
        and col.column_id = ic.column_id";
        }
    }
}
