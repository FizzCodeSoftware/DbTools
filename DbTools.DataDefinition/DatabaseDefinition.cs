namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Configuration;

    public class DatabaseDefinition
    {
        public Dictionary<SqlVersion, TypeMapper> TypeMappers { get; set; } = new Dictionary<SqlVersion, TypeMapper>();

        public DatabaseDefinition()
        {
            // TODO expandable
            if(!TypeMappers.ContainsKey(new Configuration.MsSql2016()))
                TypeMappers.Add(new Configuration.MsSql2016(), new MsSqlTypeMapper2016());

            if (!TypeMappers.ContainsKey(new Configuration.Oracle12c()))
                TypeMappers.Add(new Configuration.Oracle12c(), new OracleTypeMapper12c());

            if (!TypeMappers.ContainsKey(new Configuration.SqLite3()))
                TypeMappers.Add(new Configuration.SqLite3(), new SqLiteTypeMapper3());
        }

        internal Tables Tables { get; } = new Tables();

        public void AddTable(SqlTable sqlTable)
        {
            sqlTable.DatabaseDefinition = this;

            foreach (var typeMapper in TypeMappers)
            {
                foreach (var column in sqlTable.Columns)
                {
                    if (column is SqlColumnFKRegistration)
                        continue;

                    // TODO only map FROM Gen1 for now
                    if (!column.Types.ContainsKey(new Configuration.Generic1()))
                        continue;

                    var othertype = typeMapper.Value.MapFromGeneric1(column.Types[new Configuration.Generic1()]);
                    SqlColumnHelper.Add(typeMapper.Key, sqlTable, column.Name, othertype);
                }
            }

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

        public bool Contains(SchemaAndTableName schemaAndTableName)
        {
            return Tables.ContainsKey(schemaAndTableName.SchemaAndName);
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