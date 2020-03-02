namespace FizzCode.DbTools.DataDefinition
{
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition.MsSql2016;
    using FizzCode.DbTools.DataDefinition.Oracle12c;
    using FizzCode.DbTools.DataDefinition.SqLite3;

    public static class TypeMapperFactory
    {
        public static AbstractTypeMapper GetTypeMapper(this SqlEngineVersion version)
        {
            if (version == SqLiteVersion.SqLite3)
                return new SqLite3TypeMapper();

            if (version == MsSqlVersion.MsSql2016)
                return new MsSql2016TypeMapper();

            if (version == OracleVersion.Oracle12c)
                return new Oracle12cTypeMapper();

            if (version == GenericVersion.Generic1)
                return new Generic1TypeMapper();

            return null;
        }
    }
}