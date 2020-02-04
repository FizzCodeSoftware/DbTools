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
                SqlNChar _ => $"AddNChar(\"{column.Name}\", {type.Length}",
                SqlVarChar _ => $"AddVarChar(\"{column.Name}\", {type.Length}",
                SqlNVarChar t_ => $"AddNVarChar(\"{column.Name}\", {type.Length}",
                SqlFloatSmall _ => $"AddFloat(\"{column.Name}\"",
                SqlFloatLarge _ => $"AddReal(\"{column.Name}\"",
                SqlBit _ => $"AddBit(\"{column.Name}\"",
                SqlByte _ => $"AddByte(\"{column.Name}\"",
                SqlInt16 _ => $"AddInt16(\"{column.Name}\"",
                SqlInt32 _ => $"AddInt32(\"{column.Name}\"",
                SqlInt64 _ => $"AddInt64(\"{column.Name}\"",
                SqlNumber _ => $"AddNumber(\"{column.Name}\", {type.Length}, {type.Scale}",
                SqlDate _ => $"AddDate(\"{column.Name}\"",
                SqlTime _ => $"AddTime(\"{column.Name}\"",
                SqlDateTime _ => $"AddDateTime(\"{column.Name}\"",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo}"),
            };
        }
    }
}