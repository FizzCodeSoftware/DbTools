namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.MsSql2016;

    public class MsSqlCsGeneratorColumns2016 : GeneratorColumns
    {
        public MsSqlCsGeneratorColumns2016(Context context) : base(context)
        {
            Version = SqlVersions.MsSql2016;
        }

        protected override string GetColumnCreationMethod(SqlColumn column)
        {
            var type = column.Types[Version];
            return type.SqlTypeInfo switch
            {
                SqlChar _ => $"AddNVarChar(\"{column.Name}\", {type.Length}",
                SqlNChar _ => $"AddNChar(\"{column.Name}\", {type.Length}",
                SqlVarChar _ => $"AddVarChar(\"{column.Name}\", {type.Length}",
                SqlNVarChar _ => $"AddNVarChar(\"{column.Name}\", {type.Length}",
                SqlNText _ => $"AddNText(\"{column.Name}\"",
                SqlFloat _ => $"AddFloat(\"{column.Name}\"",
                SqlReal _ => $"AddReal(\"{column.Name}\"",
                SqlBit _ => $"AddBit(\"{column.Name}\"",
                SqlSmallInt _ => $"AddByte(\"{column.Name}\"",
                SqlTinyInt _ => $"AddInt16(\"{column.Name}\"",
                SqlInt _ => $"AddInt32(\"{column.Name}\"",
                SqlBigInt _ => $"AddInt64(\"{column.Name}\"",
                SqlDecimal _ => $"AddDecimal(\"{column.Name}\", {type.Length}, {type.Scale}",
                SqlNumeric _ => $"AddNumeric(\"{column.Name}\", {type.Length}, {type.Scale}",
                SqlMoney _ => $"AddMoney(\"{column.Name}\"",
                SqlSmallMoney _ => $"AddSmallMoney(\"{column.Name}\"",
                SqlDate _ => $"AddDate(\"{column.Name}\"",
                SqlTime _ => $"AddTime(\"{column.Name}\", {type.Length}",
                SqlDateTime _ => $"AddDateTime(\"{column.Name}\"",
                SqlDateTime2 _ => $"AddDateTime2(\"{column.Name}\", {type.Length}",
                SqlDateTimeOffset _ => $"AddDateTime2(\"{column.Name}\", {type.Length}",
                SqlSmallDateTime _ => $"AddSmallDateTime(\"{column.Name}\"",
                SqlBinary _ => $"AddBinary(\"{column.Name}\", {type.Length}",
                SqlVarBinary _ => $"AddVarBinary(\"{column.Name}\", {type.Length}",
                SqlImage _ => $"AddImage(\"{column.Name}\"",
                SqlXml _ => $"AddXml(\"{column.Name}\"",
                SqlUniqueIdentifier _ => $"AddUniqueIdentifier(\"{column.Name}\"",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo}"),
            };
        }
    }
}