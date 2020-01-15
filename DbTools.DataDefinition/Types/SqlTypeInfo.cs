using System.Collections.Generic;
using FizzCode.DbTools.Common;

namespace FizzCode.DbTools.DataDefinition
{
    public class SqlTypeInfo
    {
        public SqlTypeInfo(string dbType)
        {
            DbType = dbType;
        }

        public SqlTypeInfo(string dbType, bool hasLength, bool hasScale, bool deprecated = false)
        {
            DbType = dbType;
            HasLength = hasLength;
            IsLengthMandatory = hasLength;
            HasScale = hasScale;
            IsScaleMandatory = hasScale;
            Deprecated = deprecated;
        }

        public SqlTypeInfo(string dbType, bool isMaxLengthAllowed, bool deprecated = false)
        {
            DbType = dbType;
            Deprecated = deprecated;

            IsMaxLengthAllowed = isMaxLengthAllowed;
            HasLength = IsMaxLengthAllowed;
        }

        public SqlTypeInfo(string dbType, bool hasLength, bool isLengthMandatory, bool hasScale, bool isScaleMandatory, bool deprecated = false)
        {
            DbType = dbType;
            HasLength = hasLength;
            HasScale = hasScale;
            Deprecated = deprecated;

            IsLengthMandatory = isLengthMandatory;
            IsScaleMandatory = isScaleMandatory;
        }

        public string DbType { get; }
        public bool HasLength { get; }
        public bool IsLengthMandatory { get; }
        public bool IsMaxLengthAllowed { get; }
        public bool HasScale { get; }
        public bool IsScaleMandatory { get; }
        public bool Deprecated { get; }
    }
}