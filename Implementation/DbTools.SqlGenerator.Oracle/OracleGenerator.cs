namespace FizzCode.DbTools.SqlGenerator.Oracle
{
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.SqlGenerator.Base;

    public class OracleGenerator : AbstractSqlGeneratorBase
    {
        public OracleGenerator(Context context)
            : base(context)
        {
        }

        public override string GuardKeywords(string name)
        {
            var shouldUpperCaseEscapedNames = Context.Settings.SqlVersionSpecificSettings["UpperCaseEscapedNames"].ToString() == "true";

            if (shouldUpperCaseEscapedNames)
                name = name.ToUpper();

            return $"\"{name}\"";
        }
    }
}