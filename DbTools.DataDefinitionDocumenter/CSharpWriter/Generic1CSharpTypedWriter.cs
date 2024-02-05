using System;
using System.Globalization;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Generic;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
public class Generic1CSharpTypedWriter : AbstractCSharpTypedWriter
{
    public Generic1CSharpTypedWriter(GeneratorContext context, Type typeMapperType, string databaseName)
        : base(context, GenericVersion.Generic1, typeMapperType, databaseName)
    {
    }

    protected override string GetColumnCreationMethod(SqlColumn column)
    {
        var type = column.Types[Version];

        return type.SqlTypeInfo switch
        {
            SqlChar _ => $"{nameof(Generic1)}.{nameof(Generic1Columns.AddChar)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}",
            SqlNChar _ => $"{nameof(Generic1)}.{nameof(Generic1Columns.AddNChar)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}, {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
            SqlVarChar _ => $"{nameof(Generic1)}.{nameof(Generic1Columns.AddVarChar)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}, {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
            SqlNVarChar _ => $"{nameof(Generic1)}.{nameof(Generic1Columns.AddNVarChar)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}, {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
            SqlFloatSmall _ => $"{nameof(Generic1)}.{nameof(Generic1Columns.AddFloatSmall)}({type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
            SqlFloatLarge _ => $"{nameof(Generic1)}.{nameof(Generic1Columns.AddFloatLarge)}({type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
            SqlBit _ => $"{nameof(Generic1)}.{nameof(Generic1Columns.AddBit)}({type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
            SqlByte _ => $"{nameof(Generic1)}.{nameof(Generic1Columns.AddByte)}({type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
            SqlInt16 _ => $"{nameof(Generic1)}.{nameof(Generic1Columns.AddInt16)}({type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
            SqlInt32 _ => $"{nameof(Generic1)}.{nameof(Generic1Columns.AddInt32)}({type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
            SqlInt64 _ => $"{nameof(Generic1)}.{nameof(Generic1Columns.AddInt64)}({type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
            SqlNumber _ => $"{nameof(Generic1)}.{nameof(Generic1Columns.AddNumber)}({type.Length?.ToString("D", CultureInfo.InvariantCulture)}, {type.Scale?.ToString("D", CultureInfo.InvariantCulture)}, {type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
            SqlDate _ => $"{nameof(Generic1)}.{nameof(Generic1Columns.AddDate)}({type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
            SqlTime _ => $"{nameof(Generic1)}.{nameof(Generic1Columns.AddTime)}({type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
            SqlDateTime _ => $"{nameof(Generic1)}.{nameof(Generic1Columns.AddDateTime)}({type.IsNullable.ToString(CultureInfo.InvariantCulture)}",
            _ => throw new NotImplementedException($"Unmapped type: {type.SqlTypeInfo}"),
        };
    }
}