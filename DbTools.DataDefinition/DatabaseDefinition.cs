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

        public SqlTable GetTable(SchemaAndTableName schemaAndTableName)
        {
            return Tables[schemaAndTableName];
        }

        public IEnumerable<string> GetSchemaNames()
        {
            var schemas = GetTables().Select(t => t.SchemaAndTableName.Schema).Distinct().Where(sn => !string.IsNullOrEmpty(sn));
            return schemas;
        }
    }
}