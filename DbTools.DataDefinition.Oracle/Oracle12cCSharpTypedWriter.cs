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
                SqlBinaryFloat _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddBinaryFloat)}(\"{column.Name}\"",
                SqlBinaryDouble _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddBinaryDouble)}(\"{column.Name}\"",
                SqlBFile _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddBfile)}(\"{column.Name}\"",
                SqlBlob _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddBlob)}(\"{column.Name}\"",
                SqlClob _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddClob)}(\"{column.Name}\"",
                SqlLong _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddLong)}(\"{column.Name}\"",
                SqlLongRaw _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddLongRaw)}(\"{column.Name}\"",
                SqlNumber _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddLongRaw)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}, {type.Scale?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlDate _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddDate)}(\"{column.Name}\"",
                SqlTimeStampWithTimeZone _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddTimeStampWithTimeZone)}(\"{column.Name}\"",
                SqlTimeStampWithLocalTimeZone _ => $"{nameof(Oracle12c)}.{nameof(Oracle12c.AddTimeStampWithLocalTimeZone)}(\"{column.Name}\"",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo}"),
            };
        }
    }
}
