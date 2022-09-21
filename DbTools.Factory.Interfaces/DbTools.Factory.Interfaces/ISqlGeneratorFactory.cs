namespace FizzCode.DbTools.Factory.Interfaces
{
    using FizzCode.DbTools;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition.SqlGenerator;

    public interface ISqlGeneratorFactory
    {
        ISqlMigrationGenerator CreateMigrationGenerator(SqlEngineVersion version, Context context);
        ISqlGenerator CreateSqlGenerator(SqlEngineVersion version, Context context);
    }
}