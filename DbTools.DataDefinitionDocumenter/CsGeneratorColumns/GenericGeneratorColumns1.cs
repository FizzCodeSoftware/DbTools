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
            Version = new Generic1();
        }

        protected override string GetColumnCreationMethod(SqlColumn column)
        {
            var type = column.Types[Version];

            return type.SqlTypeInfo switch
            {
                DataDefinition.Generic1.Char _ => $"AddChar(\"{column.Name}\", {type.Length}",
                NChar _ => $"AddNChar(\"{column.Name}\", {type.Length}",
                VarChar _ => $"AddVarChar(\"{column.Name}\", {type.Length}",
                NVarChar t_ => $"AddNVarChar(\"{column.Name}\", {type.Length}",
                FloatSmall _ => $"AddFloat(\"{column.Name}\"",
                FloatLarge _ => $"AddReal(\"{column.Name}\"",
                Bit _ => $"AddBit(\"{column.Name}\"",
                DataDefinition.Generic1.Byte _ => $"AddByte(\"{column.Name}\"",
                DataDefinition.Generic1.Int16 _ => $"AddInt16(\"{column.Name}\"",
                DataDefinition.Generic1.Int32 _ => $"AddInt32(\"{column.Name}\"",
                DataDefinition.Generic1.Int64 _=> $"AddInt64(\"{column.Name}\"",
                Number _ => $"AddNumber(\"{column.Name}\", {type.Length}, {type.Scale}",
                Date _ => $"AddDate(\"{column.Name}\"",
                Time _ => $"AddTime(\"{column.Name}\"",
                DataDefinition.Generic1.DateTime _ => $"AddDateTime(\"{column.Name}\"",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo}"),
            };
        }
    }
}