using System;
using FizzCode.DbTools.DataDefinition.MsSql2016;
using FizzCode.DbTools.DataDefinition.Oracle12c;
using FizzCode.DbTools.DataDefinition.SqLite3;
using FizzCode.DbTools.DataDefinitionDocumenter;

namespace FizzCode.DbTools.DataDefinition.Factory;
public static class CSharpTypedWriterFactory
{
    public static AbstractCSharpTypedWriter GetCSharpTypedWriter(SqlEngineVersion version, GeneratorContext context, string databaseName)
    {
        if (version is GenericVersion)
            return new Generic1CSharpTypedWriter(context, typeof(Generic1TypeMapper), databaseName);

        if (version is SqLiteVersion)
            return new SqLite3CSharpTypedWriter(context, typeof(SqLite3TypeMapper), databaseName);

        if (version is MsSqlVersion)
            return new MsSql2016CSharpTypedWriter(context, typeof(MsSql2016TypeMapper), databaseName);

        if (version is OracleVersion)
            return new Oracle12cCSharpTypedWriter(context, typeof(Oracle12cTypeMapper), databaseName);

        throw new NotImplementedException($"Not implemented {version}.");
    }
}