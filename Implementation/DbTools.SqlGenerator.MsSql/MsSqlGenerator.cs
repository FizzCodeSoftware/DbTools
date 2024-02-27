using FizzCode.DbTools.Common;
using FizzCode.DbTools.SqlGenerator.Base;

namespace FizzCode.DbTools.SqlGenerator.MsSql;
public class MsSqlGenerator(Context context) : AbstractSqlGeneratorBase(context)
{
    public override SqlEngineVersion SqlVersion => MsSqlVersion.MsSql2016;

    public override string GuardKeywordsImplementation(string name)
    {
        return $"[{name}]";
    }
}