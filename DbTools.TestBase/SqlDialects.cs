namespace FizzCode.DbTools.TestBase
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using FizzCode.DbTools.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public abstract class SqlVersionsBasAttribute : Attribute, ITestDataSource
    {
        protected List<SqlVersion> Versions { get; set; }

        protected SqlVersionsBasAttribute(params Type[] versionTypes)
        {
            Versions = new List<SqlVersion>();
            foreach (var versionType in versionTypes)
            {
                var version = (SqlVersion)Activator.CreateInstance(versionType);
                Versions.Add(version);
            }
        }

        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            foreach (var item in Versions)
            {
                //if (TestHelper.ShouldRunIntegrationTest(item))
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
            Versions = SqlVersions.GetLatestExecutableVersions();
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class SqlVersionsAttribute : SqlVersionsBasAttribute
    {
        public SqlVersionsAttribute()
        {
            Versions = SqlVersions.Versions;
        }

        public SqlVersionsAttribute(params Type[] versionTypes) : base(versionTypes)
        {
        }
    }
}