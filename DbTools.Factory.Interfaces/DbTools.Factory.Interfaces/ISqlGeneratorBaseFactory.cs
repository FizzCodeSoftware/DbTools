namespace FizzCode.DbTools.Factory.Interfaces
{
    using FizzCode.DbTools;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.SqlGenerator.Base;

    public interface ISqlGeneratorBaseFactory
    {
        ISqlGeneratorBase CreateGenerator(SqlEngineVersion version, Context context);
    }
}