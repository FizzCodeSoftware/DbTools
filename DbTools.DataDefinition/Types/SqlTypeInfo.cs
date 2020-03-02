namespace FizzCode.DbTools.DataDefinition
{
    using System;

    public abstract class SqlTypeInfo
    {
        public abstract bool HasLength { get; }
        public abstract bool HasScale { get; }

        public virtual bool Deprecated => false;

        public virtual string SqlDataType
        {
            get
            {
                var fullTypeName = GetType().Name;

                var typeName = fullTypeName.StartsWith("Sql", StringComparison.InvariantCulture)
                    ? fullTypeName.Remove(0, 3)
                    : fullTypeName;

                return typeName;
            }
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}