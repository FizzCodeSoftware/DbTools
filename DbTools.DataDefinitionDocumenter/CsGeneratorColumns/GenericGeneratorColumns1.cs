namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Generic1;

    public class GenericGeneratorColumns1 : GeneratorColumns
    {
        public GenericGeneratorColumns1(Context context) : base(context)
        {
            Version = SqlVersions.Generic1;
        }

        protected override string GetColumnCreationMethod(SqlColumn column)
        {
            var type = column.Types[Version];

            return type.SqlTypeInfo switch
            {
                SqlChar _ => $"AddChar(\"{column.Name}\", {type.Length}",
                SqlNChar _ => $"AddNChar(\"{column.Name}\", {type.Length}, {type.IsNullable}",
                SqlVarChar _ => $"AddVarChar(\"{column.Name}\", {type.Length}, {type.IsNullable}",
                SqlNVarChar _ => $"AddNVarChar(\"{column.Name}\", {type.Length}, {type.IsNullable}",
                SqlFloatSmall _ => $"AddFloat(\"{column.Name}\", {type.IsNullable}",
                SqlFloatLarge _ => $"AddReal(\"{column.Name}\", {type.IsNullable}",
                SqlBit _ => $"AddBit(\"{column.Name}\", {type.IsNullable}",
                SqlByte _ => $"AddByte(\"{column.Name}\", {type.IsNullable}",
                SqlInt16 _ => $"AddInt16(\"{column.Name}\", {type.IsNullable}",
                SqlInt32 _ => $"AddInt32(\"{column.Name}\", {type.IsNullable}",
                SqlInt64 _ => $"AddInt64(\"{column.Name}\", {type.IsNullable}",
                SqlNumber _ => $"AddNumber(\"{column.Name}\", {type.Length}, {type.Scale}, {type.IsNullable}",
                SqlDate _ => $"AddDate(\"{column.Name}\", {type.IsNullable}",
                SqlTime _ => $"AddTime(\"{column.Name}\", {type.IsNullable}",
                SqlDateTime _ => $"AddDateTime(\"{column.Name}\", {type.IsNullable}",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo}"),
            };
        }
    }
}