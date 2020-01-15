using System.Collections.Generic;
using System.Linq;

namespace FizzCode.DbTools.Common
{
    public enum SqlDialectX
    {
        Generic,
        MsSql,
        SqLite,
        Oracle
    }

    public abstract class SqlVersion
    {
        public abstract SqlDialectX SqlDialect { get; }

        public override string ToString()
        {
            return SqlDialect.ToString() + "/" + GetType().Name;
        }
    }

    public abstract class OracleVersion : SqlVersion
    {
        public override SqlDialectX SqlDialect => SqlDialectX.Oracle;
    }

    public class Oracle12c : OracleVersion
    {
    }

    public abstract class MsSqlVersion : SqlVersion
    {
        public override SqlDialectX SqlDialect => SqlDialectX.MsSql;
    }

    public class MsSql2016 : MsSqlVersion
    { }

    public abstract class SqLiteVersion : SqlVersion
    {
        public override SqlDialectX SqlDialect => SqlDialectX.SqLite;
    }

    public class SqLite3 : SqLiteVersion
    {
    }

    public abstract class GenericVersion : SqlVersion
    {
        public override SqlDialectX SqlDialect => SqlDialectX.Generic;
    }

    public class Generic1 : GenericVersion
    { }

    public static class SqlEngines
    {
        static SqlEngines()
        {
            Versions.Add(new Generic1());
            Versions.Add(new MsSql2016());
            Versions.Add(new SqLite3());
            Versions.Add(new Oracle12c());
        }

        public static List<SqlVersion> Versions => new List<SqlVersion>();

        public static SqlVersion GetLatestVersion(SqlDialectX sqlDialextX)
        {
            var lastVersion = Versions.Last(v => v.SqlDialect == sqlDialextX);
            return lastVersion;
        }

        public static List<SqlVersion> GetVersions(SqlDialectX sqlDialectX)
        {
            var result = Versions.Where(v => v.SqlDialect == sqlDialectX).ToList();
            return result;
        }
    }
}
