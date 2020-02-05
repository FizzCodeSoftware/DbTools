namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqLite3;

    public class SqLiteCsGeneratorColumns3 : GeneratorColumns
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
                SqlInteger _ => $"AddNVarChar(\"{column.Name}\"",
                SqlReal _ => $"AddNVarChar(\"{column.Name}\"",
                SqlText _ => $"AddNVarChar(\"{column.Name}\"",
                SqlBlob _ => $"AddNVarChar(\"{column.Name}\"",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo}"),
            };
        }
    }
}