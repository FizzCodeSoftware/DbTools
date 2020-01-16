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
            SqlEngines.GetLatestVersions().CopyTo(Versions);
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class SqlVersionsAttribute : SqlVersionsBasAttribute
    {
        public SqlVersionsAttribute()
        {
            SqlEngines.Versions.CopyTo(Versions, 0);
        }

        public SqlVersionsAttribute(params SqlVersion[] versions) : base(versions)
        {
        }
    }
}