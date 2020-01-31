using System;

namespace FizzCode.DbTools.DataDefinition
{
    public abstract class SqlTypeInfo
    {
        public abstract bool HasLength { get; }
        public abstract bool HasScale { get; }
        
        public virtual bool Deprecated
        {
            get
            {
                return false;
            }
        }

        public virtual string SqlDataType => GetType().Name;
    }
}