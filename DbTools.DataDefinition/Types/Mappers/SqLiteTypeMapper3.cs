namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using FizzCode.DbTools.Configuration;
    using FizzCode.DbTools.DataDefinition.Generic1;
    using FizzCode.DbTools.DataDefinition.SqLite3;

    public class SqLiteTypeMapper3 : TypeMapper
    {
        public override SqlVersion SqlVersion => SqlEngines.SqLite3;

        public override SqlType MapFromGeneric1(SqlType genericType)
        {
            return genericType.SqlTypeInfo switch
            {
                Generic1.SqlChar _ => genericType.Create(SqLiteType3.Text),
                Generic1.SqlNChar _ => genericType.Create(SqLiteType3.Text),
                Generic1.SqlVarChar _ => genericType.Create(SqLiteType3.Text),
                Generic1.SqlNVarChar _ => genericType.Create(SqLiteType3.Text),
                Generic1.SqlFloatSmall _ => genericType.Create(SqLiteType3.Real),
                Generic1.SqlFloatLarge _ => genericType.Create(SqLiteType3.Real),
                Generic1.SqlBit _ => genericType.Create(SqLiteType3.Real),
                Generic1.SqlByte _ => genericType.Create(SqLiteType3.Real),
                Generic1.SqlInt16 _ => genericType.Create(SqLiteType3.Real),
                Generic1.SqlInt32 _ => genericType.Create(SqLiteType3.Real),
                Generic1.SqlInt64 _ => genericType.Create(SqLiteType3.Real),
                Generic1.SqlDateTime _ => genericType.Create(SqLiteType3.Real),
                Generic1.SqlDate _ => genericType.Create(SqLiteType3.Text),
                _ => throw new NotImplementedException($"Unmapped type {genericType.SqlTypeInfo}"),
            };
        }

        public override SqlType MapToGeneric1(SqlType sqlType)
        {
            return sqlType.SqlTypeInfo switch
            {
                SqLite3.SqlText _ => sqlType.Create(GenericSqlType1.Text),
                SqLite3.SqlReal _ => sqlType.Create(SqLiteType3.Real),
                SqLite3.SqlInteger _ => sqlType.Create(SqLiteType3.Integer),
                SqLite3.SqlBlob _ => sqlType.Create(SqLiteType3.Blob),
                _ => throw new NotImplementedException($"Unmapped type {sqlType.SqlTypeInfo}"),
            };
        }
    }
}