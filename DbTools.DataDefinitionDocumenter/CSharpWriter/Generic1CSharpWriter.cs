namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using System;
    using System.Globalization;
    using FizzCode.DbTools.DataDefinition;
    using FizzCode.DbTools.DataDefinition.Generic1;

    public class Generic1CSharpWriter : AbstractCSharpWriter
    {
        public Generic1CSharpWriter(GeneratorContext context, Type typeMapperType, string databaseName)
            : base(context, GenericVersion.Generic1, typeMapperType, databaseName)
        {
        }

        protected override string GetColumnCreationMethod(SqlColumn column)
        {
            var type = column.Types[Version];

            return type.SqlTypeInfo switch
            {
                SqlChar _ => $"{nameof(Generic1Columns.AddChar)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
                SqlNChar _ => $"{nameof(Generic1Columns.AddNChar)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}, {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlVarChar _ => $"{nameof(Generic1Columns.AddVarChar)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}, {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlNVarChar _ => $"{nameof(Generic1Columns.AddNVarChar)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}, {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlFloatSmall _ => $"{nameof(Generic1Columns.AddFloatSmall)}(\"{column.Name}\", {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlFloatLarge _ => $"{nameof(Generic1Columns.AddFloatLarge)}(\"{column.Name}\", {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlBit _ => $"{nameof(Generic1Columns.AddBit)}(\"{column.Name}\", {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlByte _ => $"{nameof(Generic1Columns.AddByte)}(\"{column.Name}\", {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlInt16 _ => $"{nameof(Generic1Columns.AddInt16)}(\"{column.Name}\", {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlInt32 _ => $"{nameof(Generic1Columns.AddInt32)}(\"{column.Name}\", {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlInt64 _ => $"{nameof(Generic1Columns.AddInt64)}(\"{column.Name}\", {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlNumber _ => $"{nameof(Generic1Columns.AddNumber)}(\"{column.Name}\", {type.Length?.ToString("D", CultureInfo.InvariantCulture)}, {type.Scale?.ToString("D", CultureInfo.InvariantCulture)}, {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlDate _ => $"{nameof(Generic1Columns.AddDate)}(\"{column.Name}\", {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlTime _ => $"{nameof(Generic1Columns.AddTime)}(\"{column.Name}\", {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                SqlDateTime _ => $"{nameof(Generic1Columns.AddDateTime)}(\"{column.Name}\", {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
                _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo}"),
            };
        }
    }
}