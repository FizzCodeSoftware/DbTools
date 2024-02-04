using FizzCode.DbTools.Common;
using FizzCode.DbTools.SqlGenerator.Base;

namespace FizzCode.DbTools.SqlGenerator.MsSql;
public class MsSqlGenerator : AbstractSqlGeneratorBase
{
    public override SqlEngineVersion SqlVersion => MsSqlVersion.MsSql2016;

    public MsSqlGenerator(Context context)
        : base(context)
    {
    }

    public override string GuardKeywordsImplementation(string name)
    {
        return $"[{name}]";
    }
}