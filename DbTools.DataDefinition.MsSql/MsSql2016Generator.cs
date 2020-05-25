namespace FizzCode.DbTools.DataDefinition.MsSql2016
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqlGenerator;

    public class MsSql2016Generator : AbstractSqlGenerator, ISqlGeneratorDropAndCreateDatabase
    {
        public MsSql2016Generator(Context context)
            : base(context)
        {
            Version = MsSqlVersion.MsSql2016;
        }

        public override string GuardKeywords(string name)
        {
            return $"[{name}]";
        }

        // TODO paramter
        public SqlStatementWithParameters CreateDatabase(string databaseName)
        {
            return $"CREATE DATABASE {GuardKeywords(databaseName)}";
        }

        public string DropDatabase(string databaseName)
        {
            return $"DROP DATABASE {GuardKeywords(databaseName)}";
        }

        // TODO paramter
        public SqlStatementWithParameters DropDatabaseIfExists(string databaseName)
        {
            return new SqlStatementWithParameters($"IF EXISTS(select * from sys.databases where name = @DatabaseName)\r\n\t{DropDatabase(databaseName)}", databaseName);
        }

        public override string DropAllForeignKeys()
        {
            return @"DECLARE @sql nvarchar(2000)
-- DROP FKs
WHILE(EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE='FOREIGN KEY'))
BEGIN
    SELECT TOP 1 @sql=('ALTER TABLE ' + TABLE_SCHEMA + '.[' + TABLE_NAME + '] DROP CONSTRAINT [' + CONSTRAINT_NAME + ']')
    FROM information_schema.table_constraints
    WHERE CONSTRAINT_TYPE = 'FOREIGN KEY'
    EXEC (@sql)
END";
        }

        public override string DropAllTables()
        {
            return @"DECLARE @sql nvarchar(2000)
-- DROP Tables
WHILE(EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES where TABLE_TYPE = 'BASE TABLE'))
BEGIN
    SELECT TOP 1 @sql=('DROP TABLE ' + TABLE_SCHEMA + '.[' + TABLE_NAME + ']')
    FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_TYPE = 'BASE TABLE'
    EXEC (@sql)
END";
            // Azure misses sp_MSforeachtable and sp_MSdropconstraints, thus the above
            /* return @"
exec sp_MSforeachtable ""declare @name nvarchar(max); set @name = parsename('?', 1); exec sp_MSdropconstraints @name"";
exec sp_MSforeachtable ""drop table ?"";";
            */
        }

        public override SqlStatementWithParameters DropSchemas(List<string> schemaNames, bool hard = false)
        {
            return string.Join(Environment.NewLine, schemaNames.Select(x => "DROP SCHEMA IF EXISTS " + GuardKeywords(x) + ";"));
        }

        public override string DropAllViews()
        {
            return @"DECLARE @sql nvarchar(2000)

-- DROP Views
WHILE(EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.VIEWS))
BEGIN
    SELECT TOP 1 @sql=('DROP VIEW ' + TABLE_SCHEMA + '.[' + TABLE_NAME + ']')
    FROM INFORMATION_SCHEMA.VIEWS
    EXEC (@sql)
END";
        }

        public override string DropAllIndexes()
        {
            return @"
DECLARE @sql NVARCHAR(MAX);
SELECT @sql = (
    SELECT 'IF EXISTS(SELECT * FROM sys.indexes WHERE name='''+ i.name +''' AND object_id = OBJECT_ID(''['+s.name+'].['+o.name+']''))\
		DROP INDEX ['+i.name+'] ON ['+s.name+'].['+o.name+'];'
    FROM sys.indexes i
        INNER JOIN sys.objects o ON i.object_id=o.object_id
        INNER JOIN sys.schemas s ON o.schema_id = s.schema_id
    WHERE o.type <> 'S' AND is_primary_key <> 1 AND index_id > 0
    AND s.name != 'sys' AND s.name != 'sys' AND is_unique_constraint = 0
FOR XML path(''));

EXEC sp_executesql @sql";
        }

        public override SqlStatementWithParameters CreateDbTableDescription(SqlTable table)
        {
            var sqlTableDescription = table.Properties.OfType<SqlTableDescription>().FirstOrDefault();
            if (sqlTableDescription == null)
            {
                return null;
            }

            var sqlStatementWithParameters = new SqlStatementWithParameters("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value = @Description, @level0type=N'SCHEMA', @level0name = @SchemaName, @level1type=N'TABLE', @level1name = @TableName");

            sqlStatementWithParameters.Parameters.Add("@Description", sqlTableDescription.Description);

            sqlStatementWithParameters.Parameters.Add("@SchemaName", table.SchemaAndTableName.Schema ?? Context.Settings.SqlVersionSpecificSettings.GetAs<string>("DefaultSchema"));
            sqlStatementWithParameters.Parameters.Add("@TableName", table.SchemaAndTableName.TableName);

            return sqlStatementWithParameters;
        }

        public override SqlStatementWithParameters CreateDbColumnDescription(SqlColumn column)
        {
            var sqlColumnDescription = column.Properties.OfType<SqlColumnDescription>().FirstOrDefault();
            if (sqlColumnDescription == null)
                return null;

            var sqlStatementWithParameters = new SqlStatementWithParameters("EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value = @Description, @level0type=N'SCHEMA', @level0name=@SchemaName, @level1type=N'TABLE', @level1name = @TableName, @level2type=N'COLUMN', @level2name= @ColumnName");

            var defaultSchema = Context.Settings.SqlVersionSpecificSettings.GetAs<string>("DefaultSchema");

            sqlStatementWithParameters.Parameters.Add("@Description", sqlColumnDescription.Description);
            sqlStatementWithParameters.Parameters.Add("@SchemaName", column.Table.SchemaAndTableName.Schema ?? defaultSchema);
            sqlStatementWithParameters.Parameters.Add("@TableName", column.Table.SchemaAndTableName.TableName);
            sqlStatementWithParameters.Parameters.Add("@ColumnName", column.Name);

            return sqlStatementWithParameters;
        }

        public override string CreateForeignKey(ForeignKey fk)
        {
            /* example: ALTER TABLE [dbo].[Dim_Currency]  WITH CHECK ADD  CONSTRAINT [FK_Dim_Currency_Dim_CurrencyGroup] FOREIGN KEY([Dim_CurrencyGroupId])
            REFERENCES[dbo].[Dim_CurrencyGroup]([Dim_CurrencyGroupId])
            
            ALTER TABLE [dbo].[Dim_Currency] CHECK CONSTRAINT [FK_Dim_Currency_Dim_CurrencyGroup]
            */

            var sb = new StringBuilder();

            sb.Append("ALTER TABLE ")
                .Append(GetSimplifiedSchemaAndTableName(fk.SqlTable.SchemaAndTableName));
            if (fk.SqlEngineVersionSpecificProperties.Contains(Version, "Nocheck") && fk.SqlEngineVersionSpecificProperties[Version, "Nocheck"] == "true")
                sb.Append(" WITH NOCHECK ADD ");
            else
                sb.Append(" WITH CHECK ADD ");

            sb.AppendLine(FKConstraint(fk))
            .Append("ALTER TABLE ")
            .Append(GetSimplifiedSchemaAndTableName(fk.SqlTable.SchemaAndTableName));
            if (fk.SqlEngineVersionSpecificProperties.Contains(Version, "Nocheck") && fk.SqlEngineVersionSpecificProperties[Version, "Nocheck"] == "true")
                sb.Append(" NOCHECK CONSTRAINT ");
            else
                sb.Append(" CHECK CONSTRAINT ");

            sb.AppendLine(GuardKeywords(fk.Name));

            return sb.ToString();
        }
    }
}