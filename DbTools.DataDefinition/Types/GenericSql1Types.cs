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

    public class SqlChar : GenericSqlType1
    {
        public override bool HasLength => true;
        public override bool HasScale => false;
    }

    public class SqlNChar : GenericSqlType1
    {
        public override bool HasLength => true;
        public override bool HasScale => false;
    }

    public class SqlVarChar : GenericSqlType1
    {
        public override bool HasLength => true;
        public override bool HasScale => false;
    }

    public class SqlNVarChar : GenericSqlType1
    {
        public override bool HasLength => true;
        public override bool HasScale => false;
    }

    public class SqlFloatSmall : GenericSqlType1
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
    }

    public class SqlFloatLarge : GenericSqlType1
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
    }

    public class SqlBit : GenericSqlType1
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
    }

    public class SqlByte : GenericSqlType1
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
    }

    public class SqlInt16 : GenericSqlType1
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
    }

    public class SqlInt32 : GenericSqlType1
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
    }

    public class SqlInt64 : GenericSqlType1
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
    }

    public class SqlNumber : GenericSqlType1
    {
        public override bool HasLength => true;
        public override bool HasScale => true;
    }

    public class SqlDate : GenericSqlType1
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
    }

    public class SqlTime : GenericSqlType1
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
    }

    public class SqlDateTime : GenericSqlType1
    {
        public override bool HasLength => false;
        public override bool HasScale => false;
    }
}