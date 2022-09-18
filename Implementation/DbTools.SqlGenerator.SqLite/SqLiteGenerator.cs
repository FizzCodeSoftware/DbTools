namespace FizzCode.DbTools.SqlGenerator.SqLite
{
    using FizzCode.DbTools.SqlGenerator.Base;

    public class SqLiteGenerator : AbstractSqlGeneratorBase
    {
        public override string GuardKeywords(string name)
        {
            return $"\"{name}\"";
        }
    }
}