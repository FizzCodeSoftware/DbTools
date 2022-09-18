namespace FizzCode.DbTools.SqlGenerator.Base
{
    public abstract class AbstractSqlGeneratorBase : ISqlGeneratorBase
    {
        public abstract string GuardKeywords(string name);
    }
}