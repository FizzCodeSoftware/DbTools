namespace FizzCode.DbTools.SqlGenerator.Base
{
    using FizzCode.DbTools.Common;

    public interface ISqlGeneratorBase
    {
        string GuardKeywords(string name);
        Context Context { get; }
    }
}