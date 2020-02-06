namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Configuration;

    public class DatabaseDefinition
    {
        public Dictionary<SqlVersion, TypeMapper> TypeMappers { get; set; } = new Dictionary<SqlVersion, TypeMapper>();

        public DatabaseDefinition() : this(null, Configuration.SqlVersions.MsSql2016, Configuration.SqlVersions.Oracle12c, Configuration.SqlVersions.SqLite3)
        {
        }

        public List<SqlVersion> SqlVersions { get; } = new List<SqlVersion>();

        public SqlVersion MainVersion { get; protected set; }

        public DatabaseDefinition(SqlVersion mainVersion)
        {
            MainVersion = mainVersion;
        }

        public DatabaseDefinition(SqlVersion mainVersion, params SqlVersion[] versions)
        {
            SetVersions(mainVersion, versions);
        }

        public void SetVersions(SqlVersion mainVersion, params SqlVersion[] versions)
        {
            MainVersion = mainVersion;
            SqlVersions.AddRange(versions);

            foreach (var version in SqlVersions)
            {
                if (!TypeMappers.ContainsKey(version))
                    TypeMappers.Add(version, TypeMapperFactory.GetTypeMapper(version));
            }
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
                    if (!column.Types.ContainsKey(Configuration.SqlVersions.Generic1))
                        continue;

                    var othertype = typeMapper.Value.MapFromGeneric1(column.Types[Configuration.SqlVersions.Generic1]);
                    SqlColumnHelper.Add(typeMapper.Key, sqlTable, column.Name, othertype);
                }
            }

            if (MainVersion != null && MainVersion != Configuration.SqlVersions.Generic1)
            {
                var typeMapper = TypeMapperFactory.GetTypeMapper(MainVersion);
                foreach (var column in sqlTable.Columns)
                {
                    if (column is SqlColumnFKRegistration)
                        continue;

                    // map TO Gen1 if not present
                    if (!column.Types.ContainsKey(Configuration.SqlVersions.Generic1))
                    {
                        var genericType = typeMapper.MapToGeneric1(column.Types[MainVersion]);
                        SqlColumnHelper.Add(Configuration.SqlVersions.Generic1, sqlTable, column.Name, genericType);
                    }
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