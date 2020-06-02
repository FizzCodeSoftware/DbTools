namespace FizzCode.DbTools.DataDefinition.SqLite3
{
    using System;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionDocumenter;

    public class SqLite3CSharpWriter : AbstractCSharpWriter
    {
        public SqLite3CSharpWriter(GeneratorContext context, Type typeMapperType, string databaseName)
            : base(context, SqLiteVersion.SqLite3, typeMapperType, databaseName)
        {
        }

        protected override string GetColumnCreationMethod(SqlColumn column)
        {
            var type = column.Types[Version];
            return type.SqlTypeInfo switch
            {
                SqlInteger _ => $"{nameof(SqLite3Columns.AddInteger)}(\"{column.Name}\"",
                SqlReal _ => $"{nameof(SqLite3Columns.AddReal)}(\"{column.Name}\"",
                SqlText _ => $"{nameof(SqLite3Columns.AddText)}(\"{column.Name}\"",
                SqlBlob _ => $"{nameof(SqLite3Columns.AddBlob)}(\"{column.Name}\"",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo}"),
            };
        }
    }
}