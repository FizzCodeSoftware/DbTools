namespace FizzCode.DbTools.DataDefinition.MsSql2016
{
    using System;
    using System.Globalization;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionDocumenter;

    public class MsSql2016CSharpWriter : AbstractCSharpWriter
    {
        public MsSql2016CSharpWriter(Context context)
            : base(context)
        {
            Version = MsSqlVersion.MsSql2016;
        }

        protected override string GetColumnCreationMethod(SqlColumn column)
        {
            var type = column.Types[Version];
            return type.SqlTypeInfo switch
            {
                SqlChar _ => $"{nameof(MsSql2016Columns.AddChar)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlNChar _ => $"{nameof(MsSql2016Columns.AddNChar)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlVarChar _ => $"{nameof(MsSql2016Columns.AddVarChar)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlNVarChar _ => $"{nameof(MsSql2016Columns.AddNVarChar)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlNText _ => $"{nameof(MsSql2016Columns.AddNText)}(\"{column.Name}\"",
                SqlFloat _ => $"{nameof(MsSql2016Columns.AddFloat)}(\"{column.Name}\"",
                SqlReal _ => $"{nameof(MsSql2016Columns.AddReal)}(\"{column.Name}\"",
                SqlBit _ => $"{nameof(MsSql2016Columns.AddBit)}(\"{column.Name}\"",
                SqlSmallInt _ => $"{nameof(MsSql2016Columns.AddSmallInt)}(\"{column.Name}\"",
                SqlTinyInt _ => $"{nameof(MsSql2016Columns.AddTinyInt)}(\"{column.Name}\"",
                SqlInt _ => $"{nameof(MsSql2016Columns.AddInt)}(\"{column.Name}\"",
                SqlBigInt _ => $"{nameof(MsSql2016Columns.AddBigInt)}(\"{column.Name}\"",
                SqlDecimal _ => $"{nameof(MsSql2016Columns.AddDecimal)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}, {type.Scale?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlNumeric _ => $"{nameof(MsSql2016Columns.AddNumeric)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}, {type.Scale?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlMoney _ => $"{nameof(MsSql2016Columns.AddMoney)}(\"{column.Name}\"",
                SqlSmallMoney _ => $"{nameof(MsSql2016Columns.AddSmallMoney)}(\"{column.Name}\"",
                SqlDate _ => $"{nameof(MsSql2016Columns.AddDate)}(\"{column.Name}\"",
                SqlTime _ => $"{nameof(MsSql2016Columns.AddTime)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlDateTime _ => $"{nameof(MsSql2016Columns.AddDateTime)}(\"{column.Name}\"",
                SqlDateTime2 _ => $"{nameof(MsSql2016Columns.AddDateTime2)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlDateTimeOffset _ => $"{nameof(MsSql2016Columns.AddDateTimeOffset)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlSmallDateTime _ => $"{nameof(MsSql2016Columns.AddSmallDateTime)}(\"{column.Name}\"",
                SqlBinary _ => $"{nameof(MsSql2016Columns.AddBinary)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlVarBinary _ => $"{nameof(MsSql2016Columns.AddVarBinary)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlImage _ => $"{nameof(MsSql2016Columns.AddImage)}(\"{column.Name}\"",
                SqlXml _ => $"{nameof(MsSql2016Columns.AddXml)}(\"{column.Name}\"",
                SqlUniqueIdentifier _ => $"AddUniqueIdentifier(\"{column.Name}\"",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo}"),
            };
        }
    }
}