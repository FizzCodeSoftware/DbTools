namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using FizzCode.DbTools.DataDefinition.MsSql2016;
    using FizzCode.DbTools.DataDefinition.Oracle12c;
    using FizzCode.DbTools.DataDefinition.SqLite3;
    using FizzCode.DbTools.DataDefinitionDocumenter;

    public static class CSharpWriterFactory
    {
        public static AbstractCSharpWriter GetCSharpWriter(SqlEngineVersion version, GeneratorContext context, string databaseName)
        {
            if (version is GenericVersion)
                return new Generic1CSharpWriter(context, typeof(Generic1TypeMapper), databaseName);

            if (version is SqLiteVersion)
                return new SqLite3CSharpWriter(context, typeof(SqLite3TypeMapper), databaseName);

            if (version is MsSqlVersion)
                return new MsSql2016CSharpWriter(context, typeof(MsSql2016TypeMapper), databaseName);

            if (version is OracleVersion)
                return new Oracle12cCSharpWriter(context, typeof(Oracle12cTypeMapper), databaseName);

            throw new NotImplementedException($"Not implemented {version}.");
        }
    }
}