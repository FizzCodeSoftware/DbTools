namespace FizzCode.DbTools.TestBase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public abstract class SqlVersionsBasAttribute : Attribute, ITestDataSource
    {
        protected List<SqlEngineVersion> Versions { get; set; }

        protected SqlVersionsBasAttribute(params Type[] versionTypes)
        {
            Versions = new List<SqlEngineVersion>();
            foreach (var versionType in versionTypes)
            {
                var version = (SqlEngineVersion)Activator.CreateInstance(versionType);
                if (!Versions.Contains(version))
                    Versions.Add(version);
            }
        }

        public virtual IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            foreach (var item in Versions)
            {
                if (TestHelper.ShouldRunIntegrationTest(item))
                    yield return new[] { (object)item };
            }
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            var versionKey = (SqlEngineVersion)data[0];
            return $"{methodInfo.Name} {versionKey}";
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public sealed class LatestSqlVersionsAttribute : SqlVersionsBasAttribute
    {
        public LatestSqlVersionsAttribute(bool forceIntegrationTests = false)
        {
            _forceIntegrationTests = forceIntegrationTests;
            Versions = SqlEngineVersions.GetLatestExecutableVersions()
                .Where(x => x is MsSqlVersion || x is OracleVersion || x is SqLiteVersion)
                .ToList();
        }

        private readonly bool _forceIntegrationTests;

        public override IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            foreach (var item in Versions)
            {
                if (_forceIntegrationTests || TestHelper.ShouldRunIntegrationTest(item))
                    yield return new[] { (object)item };
            }
        }
    }

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public sealed class SqlVersionsAttribute : SqlVersionsBasAttribute
    {
        public SqlVersionsAttribute(params string[] versionTypeNames)
        {
            Versions = new List<SqlEngineVersion>();
            var allversions = SqlEngineVersions.AllVersions;
            foreach (var versionTypeName in versionTypeNames)
            {
                var version = allversions.Find(v => v.UniqueName == versionTypeName);
                if (version != null && TestHelper.ShouldRunIntegrationTest(version))
                {
                    Versions.Add(version);
                }
            }

            if (Versions.Count == 0)
                Versions.Add(SqLiteVersion.SqLite3);
        }
    }
}