namespace FizzCode.DbTools.DataDefinition.Generic1
{
    using System;

    public abstract class GenericSqlType : SqlTypeInfo
    {
        /*public bool IsLengthMandatory
        {
            get
            {
                if (IsLengthMandatoryInternal.HasValue)
                    return IsLengthMandatoryInternal.Value;
                else
                    if (!HasLength)
                    return false;
                else
                    throw new Exception();
            }
        }

        public bool IsScaleMandatory
        {
            get
            {
                if (IsScaleMandatoryInternal.HasValue)
                    return IsScaleMandatoryInternal.Value;
                else
                    if (!HasScale)
                    return false;
                else
                    throw new Exception();
            }
        }

        protected virtual bool? IsLengthMandatoryInternal { get; }

        public virtual bool? IsScaleMandatoryInternal { get; }*/
    }

    public abstract class GenericSqlType1 : SqlTypeInfo
    {
    }

    public class Char : GenericSqlType1
    {
        public override bool HasLength => true;
        public override bool HasScale => false;
    }

    public class NChar : GenericSqlType1
    {
        public override bool HasLength => true;
        public override bool HasScale => false;
    }

    public class VarChar : GenericSqlType1
    {
        public override bool HasLength => true;
        public override bool HasScale => false;
    }

    public class NVarChar : GenericSqlType1
    {
        public override bool HasLength => true;
        public override bool HasScale => false;
    }

    public class FloatSmall : GenericSqlType1
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
    }

    public class FloatLarge : GenericSqlType1
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
    }

    public class Bit : GenericSqlType1
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
    }

    public class Byte : GenericSqlType1
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
    }

    public class Int16 : GenericSqlType1
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
    }

    public class Int32 : GenericSqlType1
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
    }

    public class Int64 : GenericSqlType1
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
    }

    public class Number : GenericSqlType1
    {
        public override bool HasLength => true;
        public override bool HasScale => true;
    }


    public class Date : GenericSqlType1
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
    }

    public class Time : GenericSqlType1
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
    }

    public class DateTime : GenericSqlType1
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
    }




}