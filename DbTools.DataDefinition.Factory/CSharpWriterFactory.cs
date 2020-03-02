namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition.MsSql2016;
    using FizzCode.DbTools.DataDefinition.Oracle12c;
    using FizzCode.DbTools.DataDefinition.SqLite3;
    using FizzCode.DbTools.DataDefinitionDocumenter;

    public static class CSharpWriterFactory
    {
        public static AbstractCSharpWriter GetCSharpWriter(SqlEngineVersion version, Context context)
        {
            // TODO handle versions

            if (version is GenericVersion)
                return new Generic1CSharpWriter(context);

            if (version is SqLiteVersion)
                return new SqLite3CSharpWriter(context);

            if (version is MsSqlVersion)
                return new MsSql2016CSharpWriter(context);

            if (version is OracleVersion)
                return new Oracle12cCSharpWriter(context);

            throw new NotImplementedException($"Not implemented {version}.");
        }
    }
}