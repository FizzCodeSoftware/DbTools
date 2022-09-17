namespace DbTools.SqlGenerator.MsSql
{
    public class MsSqlGenerator : SqlGenerator
    {
        public override string GuardKeywords(string name)
        {
            return $"[{name}]";
        }
    }

    public abstract class SqlGenerator
    {
        public abstract string GuardKeywords(string name);
    }
}