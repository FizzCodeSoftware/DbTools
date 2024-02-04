using FizzCode.DbTools.Common;
using FizzCode.DbTools.SqlGenerator.Base;

namespace FizzCode.DbTools.SqlGenerator.Oracle;
public class OracleGenerator : AbstractSqlGeneratorBase
{
    public override SqlEngineVersion SqlVersion => OracleVersion.Oracle12c;

    public OracleGenerator(Context context)
        : base(context)
    {
    }

    public override string GuardKeywordsImplementation(string name)
    {
        var shouldUpperCaseEscapedNames = Context.Settings.SqlVersionSpecificSettings["UpperCaseEscapedNames"].ToString() == "true";

        if (shouldUpperCaseEscapedNames)
            name = name.ToUpper();

        return $"\"{name}\"";
    }
}