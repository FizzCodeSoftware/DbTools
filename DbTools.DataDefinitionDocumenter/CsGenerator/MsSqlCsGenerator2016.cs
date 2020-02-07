namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.MsSql2016;

    public class MsSqlCsGenerator2016 : GeneratorTypeSpecific
    {
        public MsSqlCsGenerator2016(Context context) : base(context)
        {
            Version = SqlVersions.MsSql2016;
        }

        protected override string GetColumnCreationMethod(SqlColumn column)
        {
            var type = column.Types[Version];
            return type.SqlTypeInfo switch
            {
                SqlChar _ => $"{nameof(MsSql2016Helper.AddChar)}(\"{column.Name}\", {type.Length}",
                SqlNChar _ => $"{nameof(MsSql2016Helper.AddNChar)}(\"{column.Name}\", {type.Length}",
                SqlVarChar _ => $"{nameof(MsSql2016Helper.AddVarChar)}(\"{column.Name}\", {type.Length}",
                SqlNVarChar _ => $"{nameof(MsSql2016Helper.AddNVarChar)}(\"{column.Name}\", {type.Length}",
                SqlNText _ => $"{nameof(MsSql2016Helper.AddNText)}(\"{column.Name}\"",
                SqlFloat _ => $"{nameof(MsSql2016Helper.AddFloat)}(\"{column.Name}\"",
                SqlReal _ => $"{nameof(MsSql2016Helper.AddReal)}(\"{column.Name}\"",
                SqlBit _ => $"{nameof(MsSql2016Helper.AddBit)}(\"{column.Name}\"",
                SqlSmallInt _ => $"{nameof(MsSql2016Helper.AddSmallInt)}(\"{column.Name}\"",
                SqlTinyInt _ => $"{nameof(MsSql2016Helper.AddTinyInt)}(\"{column.Name}\"",
                SqlInt _ => $"{nameof(MsSql2016Helper.AddInt)}(\"{column.Name}\"",
                SqlBigInt _ => $"{nameof(MsSql2016Helper.AddBigInt)}(\"{column.Name}\"",
                SqlDecimal _ => $"{nameof(MsSql2016Helper.AddDecimal)}(\"{column.Name}\", {type.Length}, {type.Scale}",
                SqlNumeric _ => $"{nameof(MsSql2016Helper.AddNumeric)}(\"{column.Name}\", {type.Length}, {type.Scale}",
                SqlMoney _ => $"{nameof(MsSql2016Helper.AddMoney)}(\"{column.Name}\"",
                SqlSmallMoney _ => $"{nameof(MsSql2016Helper.AddSmallMoney)}(\"{column.Name}\"",
                SqlDate _ => $"{nameof(MsSql2016Helper.AddDate)}(\"{column.Name}\"",
                SqlTime _ => $"{nameof(MsSql2016Helper.AddTime)}(\"{column.Name}\", {type.Length}",
                SqlDateTime _ => $"{nameof(MsSql2016Helper.AddDateTime)}(\"{column.Name}\"",
                SqlDateTime2 _ => $"{nameof(MsSql2016Helper.AddDateTime2)}(\"{column.Name}\", {type.Length}",
                SqlDateTimeOffset _ => $"{nameof(MsSql2016Helper.AddDateTimeOffset)}(\"{column.Name}\", {type.Length}",
                SqlSmallDateTime _ => $"{nameof(MsSql2016Helper.AddSmallDateTime)}(\"{column.Name}\"",
                SqlBinary _ => $"{nameof(MsSql2016Helper.AddBinary)}(\"{column.Name}\", {type.Length}",
                SqlVarBinary _ => $"{nameof(MsSql2016Helper.AddVarBinary)}(\"{column.Name}\", {type.Length}",
                SqlImage _ => $"{nameof(MsSql2016Helper.AddImage)}(\"{column.Name}\"",
                SqlXml _ => $"{nameof(MsSql2016Helper.AddXml)}(\"{column.Name}\"",
                SqlUniqueIdentifier _ => $"AddUniqueIdentifier(\"{column.Name}\"",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo}"),
            };
        }
    }
}