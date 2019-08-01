namespace FizzCode.DbTools.DataDefinitionGenerator
{
    public class MsSqlGenerator : GenericSqlGenerator
    {
        public override ISqlTypeMapper SqlTypeMapper { get; } = new MsSqTypeMapper();

        protected override string GuardKeywords(string name)
        {
            return $"[{name}]";
        }

        public override string CreateDatabase(string databaseName, bool shouldSkipIfExists)
        {
            return shouldSkipIfExists
                ? $"IF NOT EXISTS(select * from sys.databases where name='{databaseName}')\r\n\tCREATE DATABASE {databaseName}"
                : $"CREATE DATABASE {databaseName}";
        }

        public override string DropDatabaseIfExists(string databaseName)
        {
            return $"IF EXISTS(select * from sys.databases where name='{databaseName}')\r\n\t{DropDatabase(databaseName)}";
        }

        public override string DropAllTables()
        {
            return @"
DECLARE @sql nvarchar(2000)

-- DROP FKs
WHILE(EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS where CONSTRAINT_TYPE='FOREIGN KEY'))
BEGIN
    SELECT TOP 1 @sql=('ALTER TABLE ' + TABLE_SCHEMA + '.[' + TABLE_NAME + '] DROP CONSTRAINT [' + CONSTRAINT_NAME + ']')
    FROM information_schema.table_constraints
    WHERE CONSTRAINT_TYPE = 'FOREIGN KEY'
    EXEC (@sql)
END

-- DROP Tables
WHILE(EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES where TABLE_TYPE = 'BASE TABLE'))
BEGIN
    SELECT TOP 1 @sql=('DROP TABLE ' + TABLE_SCHEMA + '.[' + TABLE_NAME + ']')
    FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_TYPE = 'BASE TABLE'
    EXEC (@sql)
END
";
            // Azure misses sp_MSforeachtable and sp_MSdropconstraints, thus the above
            /* return @"
exec sp_MSforeachtable ""declare @name nvarchar(max); set @name = parsename('?', 1); exec sp_MSdropconstraints @name"";
exec sp_MSforeachtable ""drop table ?"";";
            */
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
    }
}