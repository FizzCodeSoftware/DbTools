namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Globalization;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Generic1;

    public class GenericGenerator1 : GeneratorTypeSpecific
    {
        public GenericGenerator1(Context context) : base(context)
        {
            Version = SqlVersions.Generic1;
        }

        protected override string GetColumnCreationMethod(SqlColumn column)
        {
            var type = column.Types[Version];

            return type.SqlTypeInfo switch
            {
                SqlChar _ => $"AddChar(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlNChar _ => $"AddNChar(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}, {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlVarChar _ => $"AddVarChar(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}, {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlNVarChar _ => $"AddNVarChar(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}, {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlFloatSmall _ => $"AddFloat(\"{column.Name}\", {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlFloatLarge _ => $"AddReal(\"{column.Name}\", {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlBit _ => $"AddBit(\"{column.Name}\", {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlByte _ => $"AddByte(\"{column.Name}\", {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlInt16 _ => $"AddInt16(\"{column.Name}\", {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlInt32 _ => $"AddInt32(\"{column.Name}\", {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlInt64 _ => $"AddInt64(\"{column.Name}\", {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlNumber _ => $"AddNumber(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}, {type.Scale?.ToString("D", CultureInfo.InvariantCulture)}, {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlDate _ => $"AddDate(\"{column.Name}\", {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlTime _ => $"AddTime(\"{column.Name}\", {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlDateTime _ => $"AddDateTime(\"{column.Name}\", {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo}"),
            };
        }
    }
}