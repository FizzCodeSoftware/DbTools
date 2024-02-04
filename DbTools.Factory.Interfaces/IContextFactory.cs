using FizzCode.DbTools.Common;

namespace FizzCode.DbTools.Factory.Interfaces;
public interface IContextFactory
{
    Context CreateContext(SqlEngineVersion version);
    ContextWithLogger CreateContextWithLogger(SqlEngineVersion version);
}