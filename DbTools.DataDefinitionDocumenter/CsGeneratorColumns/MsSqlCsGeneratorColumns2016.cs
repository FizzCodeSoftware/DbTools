namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Linq;
    using System.Text;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.MsSql2016;

    public class MsSqlCsGeneratorColumns2016 : GeneratorColumns
    {
        public MsSqlCsGeneratorColumns2016(Context context) : base(context)
        {
            Version = new Generic1();
        }

        protected override string GetColumnCreationMethod(SqlColumn column)
        {
            var type = column.Types[Version];
            return type.SqlTypeInfo switch
            {
                DataDefinition.MsSql2016.Char _ => $"AddNVarChar(\"{column.Name}\", {type.Length}",
                NChar _ => $"AddNChar(\"{column.Name}\", {type.Length}",
                VarChar _ => $"AddVarChar(\"{column.Name}\", {type.Length}",
                NVarChar _ => $"AddNVarChar(\"{column.Name}\", {type.Length}",
                NText _ => $"AddNText(\"{column.Name}\"",
                Float _ => $"AddFloat(\"{column.Name}\"",
                Real _ => $"AddReal(\"{column.Name}\"",
                Bit _ => $"AddBit(\"{column.Name}\"",
                SmallInt _ => $"AddByte(\"{column.Name}\"",
                TinyInt _ => $"AddInt16(\"{column.Name}\"",
                Int _ => $"AddInt32(\"{column.Name}\"",
                BigInt _ => $"AddInt64(\"{column.Name}\"",
                DataDefinition.MsSql2016.Decimal _ => $"AddDecimal(\"{column.Name}\", {type.Length}, {type.Scale}",
                Numeric _ => $"AddNumeric(\"{column.Name}\", {type.Length}, {type.Scale}",
                Money _ => $"AddMoney(\"{column.Name}\"",
                SmallMoney _ => $"AddSmallMoney(\"{column.Name}\"",
                Date _ => $"AddDate(\"{column.Name}\"",
                Time _ => $"AddTime(\"{column.Name}\", {type.Length}",
                DataDefinition.MsSql2016.DateTime _ => $"AddDateTime(\"{column.Name}\"",
                DateTime2 _ => $"AddDateTime2(\"{column.Name}\", {type.Length}",
                DataDefinition.MsSql2016.DateTimeOffset _ => $"AddDateTime2(\"{column.Name}\", {type.Length}",
                SmallDateTime _ => $"AddSmallDateTime(\"{column.Name}\"",
                Binary _ => $"AddBinary(\"{column.Name}\", {type.Length}",
                VarBinary _ => $"AddVarBinary(\"{column.Name}\", {type.Length}",
                Image _ => $"AddImage(\"{column.Name}\"",
                Xml _ => $"AddXml(\"{column.Name}\"",
                UniqueIdentifier _ => $"AddUniqueIdentifier(\"{column.Name}\"",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo}"),
            };
        }
    }
}