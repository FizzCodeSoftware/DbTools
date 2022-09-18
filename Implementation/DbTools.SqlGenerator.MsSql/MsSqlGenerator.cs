namespace FizzCode.DbTools.SqlGenerator.MsSql
{
    using FizzCode.DbTools.SqlGenerator.Base;

    public class MsSqlGenerator : AbstractSqlGeneratorBase
    {
        public override string GuardKeywords(string name)
        {
            return $"[{name}]";
        }
    }
}