using System;
using FizzCode.DbTools.DataDefinition.Base.Interfaces;
using FizzCode.DbTools.DataDefinition.MsSql2016;
using FizzCode.DbTools.DataDefinition.Oracle12c;
using FizzCode.DbTools.DataDefinition.SqLite3;

namespace FizzCode.DbTools.DataDefinition.Factory;
public class TypeMapperFactory : ITypeMapperFactory
{
    public ITypeMapper GetTypeMapper(SqlEngineVersion version)
    {
        if (version == SqLiteVersion.SqLite3)
            return new SqLite3TypeMapper();

        if (version == MsSqlVersion.MsSql2016)
            return new MsSql2016TypeMapper();

        if (version == OracleVersion.Oracle12c)
            return new Oracle12cTypeMapper();

        if (version == GenericVersion.Generic1)
            return new Generic1TypeMapper();

        throw new NotImplementedException($"Not implemented {version}.");
    }
}