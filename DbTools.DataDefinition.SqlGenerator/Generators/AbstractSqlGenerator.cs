namespace FizzCode.DbTools.DataDefinition.SqlGenerator
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;

    public abstract class AbstractSqlGenerator : ISqlGenerator
    {
        public Context Context { get; }

        public SqlEngineVersion Version { get; protected set; }

        protected AbstractSqlGenerator(Context context)
        {
            Context = context;
        }

        public virtual string CreateTable(SqlTable table)
        {
            return CreateTableInternal(table, false);
        }

        public virtual SqlStatementWithParameters CreateSchema(string schemaName)
        {
            if (!string.IsNullOrEmpty(schemaName))
                return $"CREATE SCHEMA {GetSchema(schemaName)}";

            return "";
        }

        protected string CreateTableInternal(SqlTable table, bool withForeignKey)
        {
            var sb = new StringBuilder();
            sb.Append("CREATE TABLE ")
                .Append(GetSimplifiedSchemaAndTableName(table.SchemaAndTableName))
                .AppendLine(" (");

            var idx = 0;
            foreach (var column in table.Columns)
            {
                if (idx++ > 0)
                    sb.AppendLine(",");

                sb.Append(GenerateCreateColumn(column));
            }

            sb.AppendLine();

            CreateTablePrimaryKey(table, sb);
            if (withForeignKey)
                CreateTableForeignKey(table, sb);

            sb.AppendLine(")");

            return sb.ToString();
        }

        public virtual SqlStatementWithParameters CreateDbColumnDescription(SqlColumn column)
        {
            var sqlColumnDescription = column.Properties.OfType<SqlColumnDescription>().FirstOrDefault();
            if (sqlColumnDescription == null)
                return null;

            var sqlStatementWithParameters = new SqlStatementWithParameters("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value = @Description, @level0type=N'SCHEMA', @level0name=@SchemaName, @level1type=N'TABLE', @level1name = @TableName, @level2type=N'COLUMN', @level2name= @ColumnName");

            sqlStatementWithParameters.Parameters.Add("@Description", sqlColumnDescription.Description);
            sqlStatementWithParameters.Parameters.Add("@SchemaName", column.Table.SchemaAndTableName.Schema);
            sqlStatementWithParameters.Parameters.Add("@TableName", column.Table.SchemaAndTableName.TableName);
            sqlStatementWithParameters.Parameters.Add("@ColumnName", column.Name);

            return sqlStatementWithParameters;
        }

        public virtual SqlStatementWithParameters CreateDbTableDescription(SqlTable table)
        {
            var sqlTableDescription = table.Properties.OfType<SqlTableDescription>().FirstOrDefault();
            if (sqlTableDescription == null)
                return null;

            var sqlStatementWithParameters = new SqlStatementWithParameters("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value = @Description, @level0type=N'SCHEMA', @level0name=@SchemaName, @level1type=N'TABLE', @level1name = @TableName");

            sqlStatementWithParameters.Parameters.Add("@Description", sqlTableDescription.Description);
            sqlStatementWithParameters.Parameters.Add("@SchemaName", table.SchemaAndTableName.Schema);
            sqlStatementWithParameters.Parameters.Add("@TableName", table.SchemaAndTableName.TableName);

            return sqlStatementWithParameters;
        }

        public string CreateIndexes(SqlTable table)
        {
            var sb = new StringBuilder();
            foreach (var index in table.Properties.OfType<Index>().ToList())
                sb.AppendLine(CreateIndex(index));

            return sb.ToString();
        }

        public virtual string CreateIndex(Index index)
        {
            var clusteredPrefix = index.Clustered != null
                ? index.Clustered == true
                ? "CLUSTERED "
                : "NONCLUSTERED "
                : null;

            var sb = new StringBuilder();
            sb.Append("CREATE ")
                .Append(index.Unique ? "UNIQUE " : "")
                .Append(clusteredPrefix)
                .Append("INDEX ")
                .Append(GuardKeywords(index.Name))
                .Append(" ON ")
                .Append(GetSimplifiedSchemaAndTableName(index.SqlTable.SchemaAndTableName))
                .AppendLine(" (")
                .AppendLine(string.Join(", \r\n", index.SqlColumns.Select(c => $"{GuardKeywords(c.SqlColumn.Name)} {c.OrderAsKeyword}"))) // Index column list + asc desc
                .AppendLine(");");

            return sb.ToString();
        }

        protected virtual void CreateTablePrimaryKey(SqlTable table, StringBuilder sb)
        {
            // example: CONSTRAINT [PK_dbo.AddressShort] PRIMARY KEY CLUSTERED ([Id] ASC)
            var pk = table.Properties.OfType<PrimaryKey>().FirstOrDefault();
            if (pk == null || pk.SqlColumns.Count == 0)
                return;

            var clusteredPrefix = pk.Clustered != null
                ? pk.Clustered == true
                    ? "CLUSTERED "
                    : "NONCLUSTERED "
                : null;

            sb.Append(", CONSTRAINT ")
                .Append(GuardKeywords(pk.Name))
                .Append(" PRIMARY KEY ")
                .AppendLine(clusteredPrefix)
                .AppendLine("(")
                .AppendLine(string.Join(", \r\n", pk.SqlColumns.Select(c => $"{GuardKeywords(c.SqlColumn.Name)} {c.OrderAsKeyword}"))) // PK column list + asc desc
                .Append(')');
        }

        public virtual string CreateForeignKeys(SqlTable table)
        {
            var allFks = table.Properties.OfType<ForeignKey>().ToList();

            if (allFks.Count == 0)
                return null;

            var sb = new StringBuilder();

            foreach (var fk in allFks)
            {
                sb.Append(CreateForeignKey(fk));
            }

            return sb.ToString();
        }

        public abstract string CreateForeignKey(ForeignKey fk);

        protected string FKConstraint(ForeignKey fk)
        {
            var sb = new StringBuilder();

            sb.Append("CONSTRAINT ")
                .Append(GuardKeywords(fk.Name))
                .Append(" FOREIGN KEY ")
                .Append('(')
                .Append(string.Join(", \r\n", fk.ForeignKeyColumns.Select(fkc => $"{GuardKeywords(fkc.ForeignKeyColumn.Name)}")))
                .Append(')')
                .Append(" REFERENCES ")
                .Append(GetSimplifiedSchemaAndTableName(fk.ReferredTable.SchemaAndTableName))
                .Append(" (")
                .Append(string.Join(", \r\n", fk.ForeignKeyColumns.Select(pkc => $"{GuardKeywords(pkc.ReferredColumn.Name)}")))
                .Append(')');

            return sb.ToString();
        }

        private void CreateTableForeignKey(SqlTable table, StringBuilder sb)
        {
            // example: CONSTRAINT [FK_Training_TrainingCatalog] FOREIGN KEY ([TrainingCatalogId]) REFERENCES [dbo].[TrainingCatalog] ([Id])

            var allFks = table.Properties.OfType<ForeignKey>().ToList();

            if (allFks.Count == 0)
                return;

            sb.AppendLine();

            foreach (var fk in allFks)
            {
                sb.Append(", ")
                    .Append(FKConstraint(fk));
            }
        }

        public string CreateUniqueConstrainsts(SqlTable table)
        {
            var sb = new StringBuilder();
            foreach (var uniqueConstraint in table.Properties.OfType<UniqueConstraint>().ToList())
                sb.AppendLine(CreateUniqueConstraint(uniqueConstraint));

            return sb.ToString();
        }

        public string CreateUniqueConstraint(UniqueConstraint uniqueConstraint)
        {
            var clusteredPrefix = uniqueConstraint.Clustered != null
                ? uniqueConstraint.Clustered == true
                ? "CLUSTERED "
                : "NONCLUSTERED "
                : null;

            var sb = new StringBuilder();
            sb.Append("ALTER TABLE ")
                .Append(GetSimplifiedSchemaAndTableName(uniqueConstraint.SqlTable.SchemaAndTableName))
                .Append(" ADD CONSTRAINT ")
                .Append(GuardKeywords(uniqueConstraint.Name))
                .Append(" UNIQUE ")
                .Append(clusteredPrefix)

                .AppendLine(" (")
                .AppendLine(string.Join(", \r\n", uniqueConstraint.SqlColumns.Select(c => $"{GuardKeywords(c.SqlColumn.Name)}"))) // Index column list
                .AppendLine(");");

            return sb.ToString();
        }

        public string DropTable(SqlTable table)
        {
            return $"DROP TABLE {GetSimplifiedSchemaAndTableName(table.SchemaAndTableName)}";
        }

        public string GenerateCreateColumn(SqlColumn column)
        {
            var type = column.Types[Version];

            var sb = new StringBuilder();
            sb.Append(GuardKeywords(column.Name))
                .Append(' ');

            sb.Append(GenerateType(type));

            var identity = column.Properties.OfType<Identity>().FirstOrDefault();
            if (identity != null)
            {
                GenerateCreateColumnIdentity(sb, identity);
            }

            var defaultValue = column.Properties.OfType<DefaultValue>().FirstOrDefault();
            if (defaultValue != null)
            {
                sb.Append(" DEFAULT(")
                    .Append(defaultValue.Value)
                    .Append(')');
            }

            if (type.IsNullable)
                sb.Append(" NULL");
            else
                sb.Append(" NOT NULL");

            return sb.ToString();
        }

        protected virtual string GenerateType(SqlType type)
        {
            var sb = new StringBuilder();
            sb.Append(type.SqlTypeInfo.SqlDataType);

            if (type.Scale.HasValue)
            {
                if (type.Length != null)
                {
                    sb.Append('(')
                        .Append(type.Length?.ToString("D", CultureInfo.InvariantCulture))
                        .Append(", ")
                        .Append(type.Scale?.ToString("D", CultureInfo.InvariantCulture))
                        .Append(')');
                }
                else
                {
                    sb.Append('(')
                        .Append(type.Scale?.ToString("D", CultureInfo.InvariantCulture))
                        .Append(')');
                }
            }
            else if (type.Length.HasValue)
            {
                sb.Append('(');
                if (type.Length == -1)
                    sb.Append("MAX");
                else
                    sb.Append(type.Length?.ToString("D", CultureInfo.InvariantCulture));

                sb.Append(')');
            }

            return sb.ToString();
        }

        protected virtual void GenerateCreateColumnIdentity(StringBuilder sb, Identity identity)
        {
            sb.Append(" IDENTITY(")
                .Append(identity.Seed)
                .Append(',')
                .Append(identity.Increment)
                .Append(')');
        }

        public virtual string CreateStoredProcedure(StoredProcedure sp)
        {
            var sb = new StringBuilder();
            sb.Append("CREATE PROCEDURE ")
                .AppendLine(GetSimplifiedSchemaAndTableName(sp.SchemaAndSpName));

            var idx = 0;
            foreach (var p in sp.SpParameters)
            {
                if (idx++ > 0)
                    sb.AppendLine(",");

                sb.Append('@')
                    .Append(p.Name)
                    .Append(' ');
                sb.Append(GenerateType(p.Type));
            }

            // EXECUTE AS
            sb.AppendLine();
            sb.AppendLine("AS");

            sb.Append(sp.SqlStatementBody);

            return sb.ToString();
        }

        public abstract string GuardKeywords(string name);

        public abstract string DropAllForeignKeys();

        public abstract string DropAllViews();

        public abstract string DropAllTables();

        public abstract string DropAllIndexes();

        public abstract SqlStatementWithParameters DropSchemas(List<string> schemaNames, bool hard = false);

        public virtual SqlStatementWithParameters TableExists(SqlTable table)
        {
            return new SqlStatementWithParameters(@"
SELECT
    CASE WHEN EXISTS(SELECT * FROM information_schema.tables WHERE table_schema = @ShemaName AND table_name = @TableName)
        THEN 1
        ELSE 0
    END", table.SchemaAndTableName.Schema, table.SchemaAndTableName.TableName);
        }

        public SqlStatementWithParameters SchmaExists(SqlTable table)
        {
            return new SqlStatementWithParameters(@"
SELECT
    CASE WHEN EXISTS(SELECT schema_name FROM information_schema.schemata WHERE schema_name = @SchemaName)
        THEN 1
        ELSE 0
    END", GetSchema(table));
        }

        public string TableNotEmpty(SqlTable table)
        {
            return $"SELECT COUNT(*) FROM (SELECT TOP 1 * FROM {GetSimplifiedSchemaAndTableName(table.SchemaAndTableName)}) t";
        }

        public string GetSimplifiedSchemaAndTableName(SchemaAndTableName schemaAndTableName)
        {
            var schema = GetSchema(schemaAndTableName.Schema);
            var tableName = schemaAndTableName.TableName;

            var defaultSchema = Context.Settings.SqlVersionSpecificSettings.GetAs<string>("DefaultSchema", null);

            if (!string.IsNullOrEmpty(defaultSchema) && Context.Settings.Options.ShouldUseDefaultSchema && string.IsNullOrEmpty(schema))
            {
                return GuardKeywords(defaultSchema) + "." + GuardKeywords(tableName);
            }

            if (!string.IsNullOrEmpty(schema) && (string.IsNullOrEmpty(defaultSchema) || !string.Equals(schema, defaultSchema, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                return GuardKeywords(schema) + "." + GuardKeywords(tableName);
            }

            return GuardKeywords(tableName);
        }

        protected string GetSchema(SqlTable table)
        {
            return GetSchema(table.SchemaAndTableName.Schema);
        }

        public virtual string GetSchema(string schema)
        {
            return schema;
        }
    }
}