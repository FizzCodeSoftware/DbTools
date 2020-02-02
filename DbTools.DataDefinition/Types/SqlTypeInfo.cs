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

        public virtual string SqlDataType
        {
            get
            {
                var fullTypeName = GetType().Name;

                string typeName;

                if (fullTypeName.StartsWith("Sql", StringComparison.InvariantCulture))
                    typeName = fullTypeName.Remove(0, 3);
                else
                    typeName = fullTypeName;

                return typeName;
            }
        }
    }
}