namespace FizzCode.DbTools.SqlGenerator.Base
{
    using FizzCode.DbTools.Common;

    public abstract class AbstractSqlGeneratorBase : ISqlGeneratorBase
    {
        public Context Context { get; }

        protected AbstractSqlGeneratorBase(Context context)
        {
            Context = context;
        }

        public abstract string GuardKeywords(string name);
    }
}