namespace FizzCode.DbTools.DataDefinition.Oracle12c
{
    using System;
    using System.Globalization;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionDocumenter;

    public class Oracle12cCSharpTypedWriter : AbstractCSharpTypedWriter
    {
        public Oracle12cCSharpTypedWriter(GeneratorContext context, Type typeMapperType)
            : base(context, OracleVersion.Oracle12c, typeMapperType)
        {
        }

        protected override string GetColumnCreationMethod(SqlColumn column)
        {
            var type = column.Types[Version];
            return type.SqlTypeInfo switch
            {
                SqlChar _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddChar)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlNChar _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddNChar)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlVarChar _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddVarChar)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlVarChar2 _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddVarChar2)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlNVarChar2 _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddNVarChar2)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlBinaryFloat _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddBinaryFloat)}(",
                SqlBinaryDouble _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddBinaryDouble)}(",
                SqlBFile _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddBfile)}(",
                SqlBlob _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddBlob)}(",
                SqlClob _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddClob)}(",
                SqlLong _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddLong)}(",
                SqlLongRaw _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddLongRaw)}(",
                SqlNumber _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.addnu)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}, {type.Scale?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlDate _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddDate)}(",
                SqlTimeStampWithTimeZone _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddTimeStampWithTimeZone)}(",
                SqlTimeStampWithLocalTimeZone _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddTimeStampWithLocalTimeZone)}(",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo}"),
            };
        }
    }
}
