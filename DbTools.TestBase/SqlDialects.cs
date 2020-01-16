namespace FizzCode.DbTools.TestBase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FizzCode.DbTools.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public abstract class SqlVersionsBasAttribute : Attribute, ITestDataSource
    {
        protected List<SqlVersion> Versions { get; set;  }

        protected SqlVersionsBasAttribute(params SqlVersion[] versions)
        {
            Versions = versions.ToList();
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
            var versionKey = (SqlVersion)data[0];
            return $"{methodInfo.Name} {versionKey}";
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class LatestSqlVersionsAttribute : SqlVersionsBasAttribute
    {
        public LatestSqlVersionsAttribute()
        {
            Versions = SqlEngines.GetLatestExecutableVersions();
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class SqlVersionsAttribute : SqlVersionsBasAttribute
    {
        public SqlVersionsAttribute()
        {
            Versions = SqlEngines.Versions;
        }

        public SqlVersionsAttribute(params SqlVersion[] versions) : base(versions)
        {
        }
    }
}