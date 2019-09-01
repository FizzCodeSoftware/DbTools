namespace FizzCode.DbTools.TestBase
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class ConnectionsAttribute : Attribute, ITestDataSource
    {
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            foreach (ConnectionStringSettings c in ConfigurationManager.ConnectionStrings)
            {
                if (TestHelper.ShouldRunIntegrationTest(c.ProviderName))
                    yield return new[] { c.Name };
            }
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            var connectionName = (string)data[0];
            return $"{methodInfo.Name} {connectionName}";
        }
    }
}