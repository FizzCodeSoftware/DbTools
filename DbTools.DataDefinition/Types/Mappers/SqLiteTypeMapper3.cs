namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using FizzCode.DbTools.DataDefinition.SqLite3;

    public class SqLiteTypeMapper3 : TypeMapper
    {
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
    }
}