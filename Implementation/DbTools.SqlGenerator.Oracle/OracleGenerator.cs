namespace FizzCode.DbTools.SqlGenerator.Oracle
{
    using FizzCode.DbTools.SqlGenerator.Base;

    public class OracleGenerator : AbstractSqlGeneratorBase
    {
        public override string GuardKeywords(string name)
        {
            return $"\"{name}\"";
        }
    }
}