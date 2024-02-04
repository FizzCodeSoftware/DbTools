using FizzCode.DbTools.Common;
using FizzCode.DbTools.SqlGenerator.Base;

namespace FizzCode.DbTools.SqlGenerator.SqLite;
public class SqLiteGenerator : AbstractSqlGeneratorBase
{
    public override SqlEngineVersion SqlVersion => SqLiteVersion.SqLite3;

    public SqLiteGenerator(Context context)
        : base(context)
    {
    }

    public override string GuardKeywordsImplementation(string name)
    {
        return $"\"{name}\"";
    }
}