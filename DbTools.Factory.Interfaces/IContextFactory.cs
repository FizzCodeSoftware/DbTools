namespace FizzCode.DbTools.Factory.Interfaces
{
    using FizzCode.DbTools;
    using FizzCode.DbTools.Common;

    public interface IContextFactory
    {
        Context CreateContext(SqlEngineVersion version);
        ContextWithLogger CreateContextWithLogger(SqlEngineVersion version);
    }
}