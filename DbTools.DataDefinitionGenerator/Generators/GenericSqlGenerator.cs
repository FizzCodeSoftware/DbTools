namespace FizzCode.DbTools.DataDefinitionGenerator
{
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;

    public abstract class GenericSqlGenerator : ISqlGenerator
    {
        public virtual ISqlTypeMapper SqlTypeMapper { get; } = new GenericSqlTypeMapper();

        private readonly Settings _settings;

        protected GenericSqlGenerator(Settings settings)
        {
            _settings = settings;
        }

        public virtual string CreateTable(SqlTable table)
        {
            return CreateTableInternal(table, false);
        }

        public virtual string CreateSchema(string schemaName)
        {
            if (!string.IsNullOrEmpty(schemaName))
                return $"CREATE SCHEMA {schemaName}";

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

                sb.Append(GenerateCreateColumn(column.Value));
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

        public string CreateIndex(Index index)
        {
            var clusteredPrefix = index.Clustered == true
                ? "CLUSTERED "
                : "NONCLUSTERED ";

            var sb = new StringBuilder();
            sb.Append("CREATE ")
                .Append(clusteredPrefix)
                .Append("INDEX ")
                .Append(GuardKeywords(index.Name))
                .Append(" ON ")
                .Append(GetSimplifiedSchemaAndTableName(index.SqlTable.SchemaAndTableName))
                .AppendLine(" (")
                .AppendLine(string.Join(", \r\n", index.SqlColumns.Select(c => $"{GuardKeywords(c.SqlColumn.Name)} {c.OrderAsKeyword}"))) // Index column list + asc desc
                .AppendLine(")");

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
                .Append(")");
        }

        public virtual string CreateForeignKeys(SqlTable table)
        {
            /* example: ALTER TABLE [dbo].[Dim_Currency]  WITH CHECK ADD  CONSTRAINT [FK_Dim_Currency_Dim_CurrencyGroup] FOREIGN KEY([Dim_CurrencyGroupId])
            REFERENCES[dbo].[Dim_CurrencyGroup]([Dim_CurrencyGroupId])
            
            ALTER TABLE [dbo].[Dim_Currency] CHECK CONSTRAINT [FK_Dim_Currency_Dim_CurrencyGroup]
            */

            var allFks = table.Properties.OfType<ForeignKey>().ToList();

            if (allFks.Count == 0)
                return null;

            var sb = new StringBuilder();

            foreach (var fk in allFks)
            {
                sb.Append("ALTER TABLE ")
                    .Append(GetSimplifiedSchemaAndTableName(table.SchemaAndTableName))
                    .Append(" WITH CHECK ADD ")
                    .AppendLine(FKConstraint(fk))
                    .Append("ALTER TABLE ")
                    .Append(GetSimplifiedSchemaAndTableName(table.SchemaAndTableName))
                    .Append(" CHECK CONSTRAINT ")
                    .AppendLine(GuardKeywords(fk.Name));
            }

            return sb.ToString();
        }

        protected string FKConstraint(ForeignKey fk)
        {
            var sb = new StringBuilder();

            sb.Append("CONSTRAINT ")
                .Append(GuardKeywords(fk.Name))
                .Append(" FOREIGN KEY ")
                .Append("(")
                .Append(string.Join(", \r\n", fk.ForeignKeyColumns.Select(fkc => $"{GuardKeywords(fkc.ForeignKeyColumn.Name)}")))
                .Append(")")
                .Append(" REFERENCES ")
                .Append(GetSimplifiedSchemaAndTableName(fk.ReferredTable.SchemaAndTableName))
                .Append(" (")
                .Append(string.Join(", \r\n", fk.ForeignKeyColumns.Select(pkc => $"{GuardKeywords(pkc.ReferredColumn.Name)}")))
                .Append(")");

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

        public string DropTable(SqlTable table)
        {
            return $"DROP TABLE {GetSimplifiedSchemaAndTableName(table.SchemaAndTableName)}";
        }

        protected string GenerateCreateColumn(SqlColumn column)
        {
            var sb = new StringBuilder();
            sb.Append(GuardKeywords(column.Name))
                .Append(" ")
                .Append(SqlTypeMapper.GetType(column.Type));

            if (column.Precision.HasValue)
            {
                sb.Append("(")
                    .Append(column.Length)
                    .Append(", ")
                    .Append(column.Precision)
                    .Append(")");
            }
            else if (column.Length.HasValue)
            {
                sb.Append("(");
                if (column.Length == -1)
                    sb.Append("MAX");
                else
                    sb.Append(column.Length);
                sb.Append(")");
            }

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
                    .Append(")");
            }

            if (column.IsNullable)
                sb.Append(" NULL");
            else
                sb.Append(" NOT NULL");

            return sb.ToString();
        }

        protected virtual void GenerateCreateColumnIdentity(StringBuilder sb, Identity identity)
        {
            sb.Append(" IDENTITY(")
                .Append(identity.Seed)
                .Append(",")
                .Append(identity.Increment)
                .Append(")");
        }

        protected abstract string GuardKeywords(string name);

        public abstract string DropAllViews();

        public abstract string DropAllTables();

        public abstract string DropAllIndexes();

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
    END", table.SchemaAndTableName.Schema);
        }

        public string TableNotEmpty(SqlTable table)
        {
            return $"SELECT COUNT(*) FROM (SELECT TOP 1 * FROM {GetSimplifiedSchemaAndTableName(table.SchemaAndTableName)}) t";
        }

        public string GetSimplifiedSchemaAndTableName(SchemaAndTableName schemaAndTableName)
        {
            var schema = schemaAndTableName.Schema;
            var tableName = schemaAndTableName.TableName;

            var defaultSchema = _settings.SqlDialectSpecificSettings.GetAs<string>("DefaultSchema");

            if (_settings.Options.ShouldUseDefaultSchema && schema == null)
                return GuardKeywords(defaultSchema) + "." + GuardKeywords(tableName);

            if (!_settings.Options.ShouldUseDefaultSchema && schema == defaultSchema)
                return GuardKeywords(tableName);

            if (schema != null)
                return GuardKeywords(schema) + "." + GuardKeywords(tableName);

            return GuardKeywords(tableName);
        }

        public Settings GetSettings()
        {
            return _settings;
        }
    }
}