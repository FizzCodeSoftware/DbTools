namespace FizzCode.DbTools.DataDefinitionGenerator.Tests
{
    using System;
    using FizzCode.DbTools.DataDefinition;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GenericSqlTypeMapperTests
    {
        [TestMethod]
        public void GenericSqlTypeMapperCoversAllTypes()
        {
            var sqlTypeMapper = new GenericSqlTypeMapper();
            foreach (var sqlType in (SqlType[])Enum.GetValues(typeof(SqlType)))
            {
                sqlTypeMapper.GetType(sqlType);
            }
        }
    }
}