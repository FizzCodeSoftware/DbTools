namespace FizzCode.DbTools.DataDefinition.SqLite3
{
    using System;
    using FizzCode.DbTools.DataDefinition.Base;
    using FizzCode.DbTools.DataDefinition.Base.Interfaces;

    public class SqLite3TypeMapper : AbstractTypeMapper
    {
        public override SqlEngineVersion SqlVersion => SqLiteVersion.SqLite3;

        public override ISqlType MapFromGeneric1(ISqlType genericType)
        {
            return genericType.SqlTypeInfo switch
            {
                Generic1.SqlChar _ => genericType.Clone(SqLiteType3.Text),
                Generic1.SqlNChar _ => genericType.Clone(SqLiteType3.Text),
                Generic1.SqlVarChar _ => genericType.Clone(SqLiteType3.Text),
                Generic1.SqlNVarChar _ => genericType.Clone(SqLiteType3.Text),
                Generic1.SqlFloatSmall _ => genericType.Clone(SqLiteType3.Real),
                Generic1.SqlFloatLarge _ => genericType.Clone(SqLiteType3.Real),
                Generic1.SqlBit _ => genericType.Clone(SqLiteType3.Integer),
                Generic1.SqlByte _ => genericType.Clone(SqLiteType3.Real),
                Generic1.SqlInt16 _ => genericType.Clone(SqLiteType3.Integer),
                Generic1.SqlInt32 _ => genericType.Clone(SqLiteType3.Integer),
                Generic1.SqlInt64 _ => genericType.Clone(SqLiteType3.Integer),
                Generic1.SqlDateTime _ => genericType.Clone(SqLiteType3.Text),
                Generic1.SqlDate _ => genericType.Clone(SqLiteType3.Text),
                _ => throw new NotImplementedException($"Unmapped type {genericType.SqlTypeInfo}"),
            };
        }

        public override ISqlType MapToGeneric1(ISqlType sqlType)
        {
            return sqlType.SqlTypeInfo switch
            {
                SqlText _ => sqlType.Clone(Generic1.GenericSqlType1.Text),
                SqlReal _ => sqlType.Clone(Generic1.GenericSqlType1.FloatLarge),
                SqlInteger _ => sqlType.Clone(Generic1.GenericSqlType1.Int32),
                // todo: finish
                //SqLite3.SqlBlob _ => sqlType.Create(Generic1.GenericSqlType1.),
                _ => throw new NotImplementedException($"Unmapped type {sqlType.SqlTypeInfo}"),
            };
        }
    }
}