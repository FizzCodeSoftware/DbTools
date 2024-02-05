using System;
using FizzCode.DbTools.DataDefinition.Base;
using FizzCode.DbTools.DataDefinition.Base.Interfaces;
using FizzCode.DbTools.DataDefinition.Generic;

namespace FizzCode.DbTools.DataDefinition.SqLite3;
public class SqLite3TypeMapper : AbstractTypeMapper
{
    public override SqlEngineVersion SqlVersion => SqLiteVersion.SqLite3;

    public override ISqlType MapFromGeneric1(ISqlType genericType)
    {
        return genericType.SqlTypeInfo switch
        {
            SqlChar _ => genericType.Clone(SqLiteType3.Text),
            SqlNChar _ => genericType.Clone(SqLiteType3.Text),
            SqlVarChar _ => genericType.Clone(SqLiteType3.Text),
            SqlNVarChar _ => genericType.Clone(SqLiteType3.Text),
            SqlFloatSmall _ => genericType.Clone(SqLiteType3.Real),
            SqlFloatLarge _ => genericType.Clone(SqLiteType3.Real),
            SqlBit _ => genericType.Clone(SqLiteType3.Integer),
            SqlByte _ => genericType.Clone(SqLiteType3.Real),
            SqlInt16 _ => genericType.Clone(SqLiteType3.Integer),
            SqlInt32 _ => genericType.Clone(SqLiteType3.Integer),
            SqlInt64 _ => genericType.Clone(SqLiteType3.Integer),
            SqlDateTime _ => genericType.Clone(SqLiteType3.Text),
            SqlDate _ => genericType.Clone(SqLiteType3.Text),
            _ => throw new NotImplementedException($"Unmapped type {genericType.SqlTypeInfo}"),
        };
    }

    public override ISqlType MapToGeneric1(ISqlType sqlType)
    {
        return sqlType.SqlTypeInfo switch
        {
            SqlText _ => sqlType.Clone(GenericSqlType1.Text),
            SqlReal _ => sqlType.Clone(GenericSqlType1.FloatLarge),
            SqlInteger _ => sqlType.Clone(GenericSqlType1.Int32),
            // todo: finish
            //SqLite3.SqlBlob _ => sqlType.Create(Generic1.GenericSqlType1.),
            _ => throw new NotImplementedException($"Unmapped type {sqlType.SqlTypeInfo}"),
        };
    }
}