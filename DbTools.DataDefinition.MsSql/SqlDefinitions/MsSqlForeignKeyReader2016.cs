namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqlExecuter;

    public class MsSqlForeignKeyReader2016 : GenericDataDefinitionElementReader
    {
        private List<Row> _queryResult;
        private List<Row> QueryResult => _queryResult ??= Executer.ExecuteQuery(GetStatement()).Rows;

        public MsSqlForeignKeyReader2016(SqlStatementExecuter executer, SchemaNamesToRead schemaNames)
            : base(executer, schemaNames)
        {
        }

        public void GetForeignKeys(DatabaseDefinition dd)
        {
            foreach (var table in dd.GetTables())
                GetForeignKeys(table);
        }

        private static string GetStatement()
        {
            return @"
SELECT
     KCU1.CONSTRAINT_NAME AS FK_CONSTRAINT_NAME
    ,KCU1.CONSTRAINT_SCHEMA as FK_CONSTRAINT_SCHEMA
    ,KCU1.TABLE_NAME AS FK_TABLE_NAME
    ,KCU1.COLUMN_NAME AS FK_COLUMN_NAME
    ,KCU1.ORDINAL_POSITION AS FK_ORDINAL_POSITION
    ,KCU2.CONSTRAINT_NAME AS REFERENCED_CONSTRAINT_NAME
    ,KCU2.CONSTRAINT_SCHEMA AS REFERENCED_CONSTRAINT_SCHEMA
    ,KCU2.TABLE_NAME AS REFERENCED_TABLE_NAME
    ,KCU2.COLUMN_NAME AS REFERENCED_COLUMN_NAME
    ,KCU2.ORDINAL_POSITION AS REFERENCED_ORDINAL_POSITION
	,OBJECTPROPERTY(OBJECT_ID(QUOTENAME(RC.CONSTRAINT_SCHEMA) + '.' + QUOTENAME(RC.CONSTRAINT_NAME)), 'CnstIsDisabled') AS [IsDisabled]
	,OBJECTPROPERTY(OBJECT_ID(QUOTENAME(RC.CONSTRAINT_SCHEMA) + '.' + QUOTENAME(RC.CONSTRAINT_NAME)), 'CnstIsNotTrusted') AS [IsNotTrusted]	
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

--WHERE KCU1.TABLE_NAME = ''
--ORDER BY KCU1.ORDINAL_POSITION";
        }

        public void GetForeignKeys(SqlTable table)
        {
            var rows = QueryResult
                .Where(r => DataDefinitionReaderHelper.SchemaAndTableNameEquals(r, table, "FK_CONSTRAINT_SCHEMA", "FK_TABLE_NAME"))
                .OrderBy(row => row.GetAs<string>("FK_CONSTRAINT_NAME"))
                .ThenBy(row => row.GetAs<int>("FK_ORDINAL_POSITION"));

            foreach (var row in rows)
            {
                var fkColumn = table.Columns[row.GetAs<string>("FK_COLUMN_NAME")];

                var referencedSchema = row.GetAs<string>("REFERENCED_CONSTRAINT_SCHEMA");
                var referencedTable = row.GetAs<string>("REFERENCED_TABLE_NAME");
                var referencedSchemaAndTableName = new SchemaAndTableName(referencedSchema, referencedTable);
                var referencedColumn = row.GetAs<string>("REFERENCED_COLUMN_NAME");
                var fkName = row.GetAs<string>("FK_CONSTRAINT_NAME");

                var referencedSqlTableSchemaAndTableNameAsToStore = GenericDataDefinitionReader.GetSchemaAndTableNameAsToStore(referencedSchemaAndTableName, Executer.Generator.Context);

                var referencedSqlTable = table.DatabaseDefinition.GetTable(referencedSqlTableSchemaAndTableNameAsToStore);

                ForeignKey fk;

                if (row.GetAs<int>("FK_ORDINAL_POSITION") == 1)
                {
                    fk = new ForeignKey(table, referencedSqlTable, fkName);
                    table.Properties.Add(fk);

                    if (row.GetAs<int>("IsNotTrusted") == 1)
                        fk.SqlEngineVersionSpecificProperties.Add(new SqlEngineVersionSpecificProperty(MsSqlVersion.MsSql2016, "Nocheck", "true"));
                    else if (row.GetAs<int>("IsNotTrusted") == 0)
                        fk.SqlEngineVersionSpecificProperties.Add(new SqlEngineVersionSpecificProperty(MsSqlVersion.MsSql2016, "Nocheck", "false"));
                }
                else
                {
                    fk = table.Properties.OfType<ForeignKey>().First(fk1 => fk1.ReferredTable.SchemaAndTableName == referencedSqlTableSchemaAndTableNameAsToStore && fk1.Name == fkName);
                }

                var referencedSqlColumn = referencedSqlTable[referencedColumn];

                fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fkColumn, referencedSqlColumn));
            }
        }
    }
}
