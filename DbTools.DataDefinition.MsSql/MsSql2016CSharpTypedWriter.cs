namespace FizzCode.DbTools.DataDefinition.MsSql2016
{
    using System;
    using System.Globalization;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionDocumenter;

    public class MsSql2016CSharpTypedWriter : AbstractCSharpTypedWriter
    {
        public MsSql2016CSharpTypedWriter(GeneratorContext context, Type typeMapperType)
            : base(context, MsSqlVersion.MsSql2016, typeMapperType)
        {
        }

        protected override string GetColumnCreationMethod(SqlColumn column)
        {
            var type = column.Types[Version];
            return type.SqlTypeInfo switch
            {
                SqlChar _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddChar)}({type.Length?.ToString("D)", CultureInfo.InvariantCulture)})",
                SqlNChar _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddNChar)}({type.Length?.ToString("D)", CultureInfo.InvariantCulture)})",
                SqlVarChar _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddVarChar)}({type.Length?.ToString("D)", CultureInfo.InvariantCulture)})",
                SqlNVarChar _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddNVarChar)}({type.Length?.ToString("D)", CultureInfo.InvariantCulture)})",
                SqlNText _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddNText)}()",
                SqlFloat _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddFloat)}()",
                SqlReal _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddReal)}()",
                SqlBit _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddBit)}()",
                SqlSmallInt _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddSmallInt)}()",
                SqlTinyInt _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddTinyInt)}()",
                SqlInt _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddInt)}()",
                SqlBigInt _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddBigInt)}()",
                SqlDecimal _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddDecimal)}({type.Length?.ToString("D)", CultureInfo.InvariantCulture)}, {type.Scale?.ToString("D)", CultureInfo.InvariantCulture)})",
                SqlNumeric _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddNumeric)}({type.Length?.ToString("D)", CultureInfo.InvariantCulture)}, {type.Scale?.ToString("D)", CultureInfo.InvariantCulture)})",
                SqlMoney _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddMoney)}()",
                SqlSmallMoney _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddSmallMoney)}()",
                SqlDate _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddDate)}()",
                SqlTime _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddTime)}({type.Length?.ToString("D)", CultureInfo.InvariantCulture)})",
                SqlDateTime _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddDateTime)}()",
                SqlDateTime2 _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddDateTime2)}({type.Length?.ToString("D)", CultureInfo.InvariantCulture)})",
                SqlDateTimeOffset _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddDateTimeOffset)}({type.Length?.ToString("D)", CultureInfo.InvariantCulture)})",
                SqlSmallDateTime _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddSmallDateTime)}()",
                SqlBinary _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddBinary)}({type.Length?.ToString("D)", CultureInfo.InvariantCulture)})",
                SqlVarBinary _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddVarBinary)}({type.Length?.ToString("D)", CultureInfo.InvariantCulture)})",
                SqlImage _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddImage)}()",
                SqlXml _ => $"{nameof(MsSql2016Columns)}.{nameof(MsSql2016Columns.AddXml)}()",
                SqlUniqueIdentifier _ => $"{nameof(MsSql2016Columns)}.AddUniqueIdentifier()",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo}"),
            };
        }
    }
}