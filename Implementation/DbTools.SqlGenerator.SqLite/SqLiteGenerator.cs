namespace FizzCode.DbTools.SqlGenerator.SqLite
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.SqlGenerator.Base;

    public class SqLiteGenerator : AbstractSqlGeneratorBase
    {
        public SqLiteGenerator(Context context)
            : base(context)
        {
        }

        public override string GuardKeywords(string name)
        {
            return $"\"{name}\"";
        }
    }
}