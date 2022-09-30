namespace FizzCode.DbTools.Factory
{
    using FizzCode.DbTools;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Interfaces;

    public interface ISqlGeneratorBaseFactory
    {
        ISqlGeneratorBase CreateGenerator(SqlEngineVersion version, Context context);
    }
}