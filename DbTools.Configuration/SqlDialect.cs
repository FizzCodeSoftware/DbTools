#pragma warning disable CA1040 // Avoid empty interfaces
namespace FizzCode.DbTools.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    // TODO - test
    // TODO Move to configuraton

    // TODO GuardKeyWord -> ConnectionStringWithProvider.IsEscaped

    // TODO version (server version)  to ConnectionStringWithProvider

    public interface ISqlDialect

    { }

    public interface IGenericDialect : ISqlDialect
    { }

    public interface IMsSqlDialect : ISqlDialect
    { }

    public interface IOracleDialect : ISqlDialect
    { }

    public interface ISqLiteDialect : ISqlDialect
    { }

    /*public enum SqlDialectX
    {
        Generic,
        MsSql,
        SqLite,
        Oracle
    }*/

    public abstract class SqlVersion
    {
        public string VersionString { get; protected set; }
        public string VersionNumber { get; protected set; }
    }

    public class Oracle12c : SqlVersion, IOracleDialect
    {
        public Oracle12c()
        {
            VersionString = "12c";
            VersionNumber = "12.1.0.1";
        }
    }

    public class MsSql2016 : SqlVersion, IMsSqlDialect
    {
        public MsSql2016()
        {
            VersionString = "2016";
            VersionNumber = "13.0";
        }
    }

    public class SqLite3 : SqlVersion, ISqLiteDialect
    {
        public SqLite3()
        {
            VersionString = "3";
            VersionNumber = "3";
        }
    }

    public class Generic1 : SqlVersion, IGenericDialect
    {
        public Generic1()
        {
            VersionString = "1";
            VersionNumber = "1";
        }
    }

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

        public static SqlVersion GetLatestVersion<T>() where T : ISqlDialect
        {
            var lastVersion = Versions.Last(v => v is T);
            return lastVersion;
        }

        public static List<SqlVersion> GetLatestVersions()
        {
            var latestVersions = Versions.GroupBy(t => t.GetType()).SelectMany(t => new[] { t.Last() }).ToList();
            return latestVersions.ToList();
        }

        public static List<SqlVersion> GetVersions<T>() where T : ISqlDialect
        {
            var result = Versions.Where(v => v is T).ToList();
            return result;
        }

        public static SqlVersion GetVersion(Type sqlDialectType, string versionString)
        {
            var result = Versions.Where(v => v.GetType() == sqlDialectType && v.VersionString == versionString).First();
            return result;
        }

        public static SqlVersion GetVersion(string sqlType)
        {
            var result = Versions.Where(v => v.GetType().Name == sqlType).First();
            return result;
        }
    }
}
#pragma warning restore CA1040 // Avoid empty interfaces