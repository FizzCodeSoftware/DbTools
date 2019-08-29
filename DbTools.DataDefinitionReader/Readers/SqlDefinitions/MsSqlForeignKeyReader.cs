namespace FizzCode.DbTools.DataDefinitionReader
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionExecuter;

    public class MsSqlForeignKeyReader
    {
        public MsSqlForeignKeyReader(SqlExecuter sqlExecuter)
        {
            _executer = sqlExecuter;
        }

        protected readonly SqlExecuter _executer;

        public void GetForeignKeys(DatabaseDefinition dd)
        {
            foreach (var table in dd.GetTables())
                GetForeignKeys(table, dd);
        }

        private List<Row> _queryResult;

        private List<Row> QueryResult {
            get
            {
                if (_queryResult == null)
                {
                    var reader = _executer.ExecuteQuery(@"
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
--ORDER BY KCU1.ORDINAL_POSITION");

                    _queryResult = reader.Rows;
                }

                return _queryResult;
            }
        }

        public DatabaseDefinition GetForeignKeys(SqlTable table, DatabaseDefinition dd)
        {
            var fakePKs = new Dictionary<SchemaAndTableName, SqlTable>();

            foreach (var row in QueryResult.Where(r =>
            DataDefinitionReaderHelper.SchemaAndTableNameEquals(r, table, "FK_CONSTRAINT_SCHEMA", "FK_TABLE_NAME")).OrderBy(row => row.GetAs<int>("FK_ORDINAL_POSITION")))
            {
                var fkColumn = table.Columns[row.GetAs<string>("FK_COLUMN_NAME")];

                var pkSchema = row.GetAs<string>("REFERENCED_CONSTRAINT_SCHEMA");
                var pkTableName = row.GetAs<string>("REFERENCED_TABLE_NAME");
                var pkSchemaAndTableName = new SchemaAndTableName(pkSchema, pkTableName);
                var pkColumnName = row.GetAs<string>("REFERENCED_COLUMN_NAME");
                var fkName = row.GetAs<string>("FK_CONSTRAINT_NAME");

                if (row.GetAs<int>("FK_ORDINAL_POSITION") == 1)
                {
                    PrimaryKey pk;
                    if (dd == null)
                    {
                        if (!fakePKs.ContainsKey(pkSchemaAndTableName))
                            fakePKs.Add(pkSchemaAndTableName, new SqlTable(row.GetAs<string>(pkTableName)));

                        pk = fakePKs[pkSchemaAndTableName].Properties.OfType<PrimaryKey>().First();
                    }
                    else
                    {
                        pk = dd.GetTable(pkSchemaAndTableName).Properties.OfType<PrimaryKey>().First();
                    }

                    var newFk = new ForeignKey(table, pk, fkName);
                    table.Properties.Add(newFk);
                }

                PrimaryKey pk2;
                if (dd == null)
                {
                    pk2 = fakePKs[pkSchemaAndTableName].Properties.OfType<PrimaryKey>().First();
                    var fakePkColumn = fkColumn.CopyTo(new SqlColumn());
                    fakePkColumn.Table = fakePKs[pkSchemaAndTableName];

                    fakePKs[pkSchemaAndTableName].Columns.Add(fakePkColumn.Name, fakePkColumn);

                    fakePKs[pkSchemaAndTableName].Properties.Add(new PrimaryKey(fakePKs[pkSchemaAndTableName], null));
                }
                else
                {
                    pk2 = dd.GetTable(pkSchemaAndTableName).Properties.OfType<PrimaryKey>().First();
                }

                var fk = table.Properties.OfType<ForeignKey>().First(fk1 => fk1.PrimaryKey.SqlTable.SchemaAndTableName == pkSchemaAndTableName);
                fk.ForeignKeyColumns.Add(new ForeignKeyColumnMap(fkColumn, pk2.SqlColumns.First(co => co.SqlColumn.Name == pkColumnName).SqlColumn));
            }

            return dd;
        }
    }
}
