namespace FizzCode.DbTools.DataDefinition
{
    using System;

    public class SqLiteTypeMapper3 : TypeMapper
    {
        public SqLiteTypeMapper3()
        {
        }

        public override SqlType MapFromGeneric1(SqlType genericType)
        {
            return genericType.SqlTypeInfo switch
            {
                Generic1.Char _ => genericType.Create(typeof(SqLite3.Text)),
                Generic1.NChar _ => genericType.Create(typeof(SqLite3.Text)),
                Generic1.VarChar _ => genericType.Create(typeof(SqLite3.Text)),
                Generic1.NVarChar _ => genericType.Create(typeof(SqLite3.Text)),
                Generic1.FloatSmall _ => genericType.Create(typeof(SqLite3.Real)),
                Generic1.FloatLarge _ => genericType.Create(typeof(SqLite3.Real)),
                Generic1.Bit _ => genericType.Create(typeof(SqLite3.Real)),
                Generic1.Byte _ => genericType.Create(typeof(SqLite3.Real)),
                Generic1.Int16 _ => genericType.Create(typeof(SqLite3.Real)),
                Generic1.Int32 _ => genericType.Create(typeof(SqLite3.Real)),
                Generic1.Int64 _ => genericType.Create(typeof(SqLite3.Real)),
                Generic1.DateTime _ => genericType.Create(typeof(SqLite3.Real)),
                Generic1.Date _ => genericType.Create(typeof(SqLite3.Text)),
                _ => throw new NotImplementedException($"Unmapped type {genericType.SqlTypeInfo}"),
            };
        }
    }
}