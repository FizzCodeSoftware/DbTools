namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Linq;

    public class DatabaseDefinition
    {
        internal Tables Tables { get; } = new Tables();

        public void AddTable(SqlTable sqlTable)
        {
            sqlTable.DatabaseDefinition = this;

            Tables.Add(sqlTable);
        }

        public virtual List<SqlTable> GetTables()
        {
            return Tables.ToList();
        }

        public SqlTable GetTable(string schema, string tableName)
        {
            return Tables[SchemaAndTableName.Concat(schema, tableName)];
        }

        public SqlTable GetTable(string tableName)
        {
            return Tables[tableName];
        }

        public SqlTable GetTable(SchemaAndTableName schemaAndTableName)
        {
            return Tables[schemaAndTableName.SchemaAndName];
        }

        public IEnumerable<string> GetSchemaNames()
        {
            var schemas = GetTables()
                .Select(t => t.SchemaAndTableName.Schema)
                .Distinct()
                .Where(sn => !string.IsNullOrEmpty(sn));

            return schemas;
        }
    }
}