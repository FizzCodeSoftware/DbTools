namespace FizzCode.DbTools.DataDefinition.SqLite3
{
    using System;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionDocumenter;

    public class SqLite3CSharpTypedWriter : AbstractCSharpTypedWriter
    {
        public SqLite3CSharpTypedWriter(GeneratorContext context, Type typeMapperType)
            : base(context, SqLiteVersion.SqLite3, typeMapperType)
        {
        }

        protected override string GetColumnCreationMethod(SqlColumn column)
        {
            var type = column.Types[Version];
            return type.SqlTypeInfo switch
            {
                SqlInteger _ => $"{nameof(SqLite3Columns.AddInteger)}(",
                SqlReal _ => $"{nameof(SqLite3Columns.AddReal)}(",
                SqlText _ => $"{nameof(SqLite3Columns.AddText)}(",
                SqlBlob _ => $"{nameof(SqLite3Columns.AddBlob)}(",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo}"),
            };
        }
    }
}