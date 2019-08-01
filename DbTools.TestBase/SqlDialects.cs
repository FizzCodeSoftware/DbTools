namespace FizzCode.DbTools.TestBase
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using FizzCode.DbTools.DataDefinition;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class SqlDialectsAttribute : Attribute, ITestDataSource
    {
        public SqlDialectsAttribute()
        {
            var array = Enum.GetValues(typeof(SqlDialect));
            _sqlDialects = new SqlDialect[array.Length];
            array.CopyTo(_sqlDialects, 0);
        }

        public SqlDialectsAttribute(params SqlDialect[] sqlDialects)
        {
            _sqlDialects = sqlDialects;
        }

        private readonly SqlDialect[] _sqlDialects;

        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            foreach (var item in _sqlDialects)
            {
                if (Helper.ShouldRunIntegrationTest(item))
                    yield return new[] { (object)item };
            }
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            var sqlDialect = (SqlDialect)data[0];
            var sqlDialectName = Enum.GetName(typeof(SqlDialect), sqlDialect);

            return $"{methodInfo.Name} {sqlDialectName}";
        }
    }
}