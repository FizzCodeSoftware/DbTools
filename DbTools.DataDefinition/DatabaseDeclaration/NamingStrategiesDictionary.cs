namespace FizzCode.DbTools.DataDefinition
{
    using System;
    using System.Collections.Generic;

    public class NamingStrategiesDictionary : Dictionary<Type, INamingStrategy>
    {
        public NamingStrategiesDictionary()
        {
            SetDefaultNamingStrategies();
        }

        public NamingStrategiesDictionary(params INamingStrategy[] namingStrategies)
        {
            SetDefaultNamingStrategies();
            foreach (var namingStrategy in namingStrategies)
            {
                if (namingStrategy is IPrimaryKeyNamingStrategy)
                    this[typeof(IPrimaryKeyNamingStrategy)] = namingStrategy;
                else if (namingStrategy is IForeignKeyNamingStrategy)
                    this[typeof(IForeignKeyNamingStrategy)] = namingStrategy;
                else if (namingStrategy is IIndexNamingStrategy)
                    this[typeof(IIndexNamingStrategy)] = namingStrategy;
                else
                    throw new NotImplementedException($"Unhandled naming strategy. {namingStrategy.GetType()}");
            }
        }

        public T GetNamingStrategy<T>()
        {
            return (T)this[typeof(T)];
        }

        private void SetDefaultNamingStrategies()
        {
            this[typeof(IPrimaryKeyNamingStrategy)] = new PrimaryKeyNamingDefaultStrategy();
            this[typeof(IForeignKeyNamingStrategy)] = new ForeignKeyNamingDefaultStrategy();
            this[typeof(IIndexNamingStrategy)] = new IndexNamingMsSqlDefaultStrategy();
        }
    }
}
