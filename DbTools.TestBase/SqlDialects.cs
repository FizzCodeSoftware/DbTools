namespace FizzCode.DbTools.TestBase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FizzCode.DbTools.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public abstract class SqlVersionsBasAttribute : Attribute, ITestDataSource
    {
        protected SqlVersion[] Versions { get; }

        protected SqlVersionsBasAttribute(params SqlVersion[] versions)
        {
            Versions = versions;
        }

        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            foreach (var item in Versions)
            {
                if (TestHelper.ShouldRunIntegrationTest(item))
                    yield return new[] { (object)item };
            }
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            var versionKey = (Version)data[0];
            return $"{methodInfo.Name} {versionKey}";
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class LatestSqlVersionsAttribute : SqlVersionsBasAttribute
    {
        public LatestSqlVersionsAttribute()
        {
            var list = new List<SqlVersion>();
            foreach (var sqlDialect in Enum.GetValues(typeof(SqlDialectX)).Cast<SqlDialectX>())
            {
                list.Add(SqlEngines.GetLatestVersion(sqlDialect));
            }

            //VersionKeys = new VersionKey[list.Count];
            list.CopyTo(Versions, 0);
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class SqlVersionsAttribute : SqlVersionsBasAttribute
    {
        public SqlVersionsAttribute()
        {
            var list = SqlEngines.Versions;
            //VersionKeys = new VersionKey[list.Count];
            list.CopyTo(Versions, 0);
        }

        public SqlVersionsAttribute(params SqlVersion[] versions) : base(versions)
        {
        }
    }
}