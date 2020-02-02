namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using FizzCode.DbTools.Configuration;

    public static class TypeMapperFactory
    {
        public static TypeMapper GetTypeMapper(SqlVersion version)
        {
            if (version is Configuration.SqLite3)
                return new SqLiteTypeMapper3();

            if (version is Configuration.MsSql2016)
                return new MsSqlTypeMapper2016();

            if (version is Configuration.Oracle12c)
                return new OracleTypeMapper12c();

            throw new NotImplementedException($"Not implemented {version}.");
        }
    }
}