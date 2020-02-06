namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using FizzCode.DbTools.Common;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.SqLite3;

    public class SqLiteCsGeneratorColumns3 : GeneratorColumns
    {
        public SqLiteCsGeneratorColumns3(Context context) : base(context)
        {
            Version = SqlVersions.SqLite3;
        }

        protected override string GetColumnCreationMethod(SqlColumn column)
        {
            var type = column.Types[Version];
            return type.SqlTypeInfo switch
            {
                SqlInteger _ => $"{nameof(SqLite3Helper.AddInteger)}(\"{column.Name}\"",
                SqlReal _ => $"{nameof(SqLite3Helper.AddReal)}(\"{column.Name}\"",
                SqlText _ => $"{nameof(SqLite3Helper.AddText)}(\"{column.Name}\"",
                SqlBlob _ => $"{nameof(SqLite3Helper.AddBlob)}(\"{column.Name}\"",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo}"),
            };
        }
    }
}