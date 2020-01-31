namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using FizzCode.DbTools.Common;
using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqLite3;

    public class SqLiteCsGeneratorColumns3: GeneratorColumns
    {
        public SqLiteCsGeneratorColumns3(Context context) : base(context)
        {
            Version = new DbTools.Configuration.SqLite3();
        }

        protected override string GetColumnCreationMethod(SqlColumn column)
        {
            var type = column.Types[Version];
            return type.SqlTypeInfo switch
            {
                Integer _ => $"AddNVarChar(\"{column.Name}\"",
                Real _  => $"AddNVarChar(\"{column.Name}\"",
                Text _ => $"AddNVarChar(\"{column.Name}\"",
                Blob _ => $"AddNVarChar(\"{column.Name}\"",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo}"),
            };
        }
    }
}