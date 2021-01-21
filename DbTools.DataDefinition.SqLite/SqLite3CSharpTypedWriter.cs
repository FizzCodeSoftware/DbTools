namespace FizzCode.DbTools.DataDefinition.SqLite3
{
    using System;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinitionDocumenter;

    public class SqLite3CSharpTypedWriter : AbstractCSharpTypedWriter
    {
        public SqLite3CSharpTypedWriter(GeneratorContext context, Type typeMapperType, string databaseName)
            : base(context, SqLiteVersion.SqLite3, typeMapperType, databaseName)
        {
        }

        protected override string GetColumnCreationMethod(SqlColumn column)
        {
            var type = column.Types[Version];
            return type.SqlTypeInfo switch
            {
                SqlInteger _ => $"{nameof(SqLite3)}.{nameof(SqLite3.AddInteger)}(",
                SqlReal _ => $"{nameof(SqLite3)}.{nameof(SqLite3.AddReal)}(",
                SqlText _ => $"{nameof(SqLite3)}.{nameof(SqLite3.AddText)}(",
                SqlBlob _ => $"{nameof(SqLite3)}.{nameof(SqLite3.AddBlob)}(",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo}"),
            };
        }
    }
}