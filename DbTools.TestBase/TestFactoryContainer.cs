namespace FizzCode.DbTools.TestBase
{
    using System;
    using System.Collections.Generic;
    using FizzCode.DbTools.DataDefinition.Base.Interfaces;
    using FizzCode.DbTools.DataDefinition.Factory;
    using FizzCode.DbTools.Factory.Interfaces;
    using FizzCode.DbTools.QueryBuilder;

    public class TestFactoryContainer : IFactoryContainer
    {
        protected Dictionary<Type, Func<object>> RegisteredCreators { get; set; } = new();
        protected Dictionary<Type, Type> RegisteredTypes { get; set; } = new();
        protected Dictionary<Type, object> RegisteredInstances { get; set; } = new();

        public TestFactoryContainer()
        {
            RegisterInstance(TestHelper.CreateLogger());
            RegisterCreator<IContextFactory>(() => new TestContextFactory(null));
            Register<ITypeMapperFactory>(typeof(TypeMapperFactory));
            Register<ISqlGeneratorFactory>(typeof(SqlGeneratorFactory));
            Register<ISqlExecuterFactory>(typeof(SqlExecuterFactory));
            Register<ISqlGeneratorBaseFactory>(typeof(SqlGeneratorBaseFactory));
            RegisterCreator<ISqlGeneratorBaseFactory>(() => new SqlGeneratorBaseFactory(Get<IContextFactory>()));
            RegisterCreator<IQueryBuilderFactory>(() =>
                new QueryBuilderFactory(Get<IContextFactory>(), Get<ISqlGeneratorBaseFactory>())
            );
        }

        public T Get<T>() where T : notnull
        {
            if (RegisteredCreators.ContainsKey(typeof(T)))
            {
                var creator = RegisteredCreators[typeof(T)];
                return (T)creator.Invoke();
            }

            var factoryType = RegisteredTypes[typeof(T)];
            if (factoryType == null)
                return (T)RegisteredInstances[typeof(T)];

            var instance = (T)Activator.CreateInstance(factoryType);
            return instance;
        }

        public void Register<TFactory>(Type implementationType) where TFactory : notnull
        {
            RegisteredTypes.Add(typeof(TFactory), implementationType);
        }

        public void RegisterInstance<T>(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            RegisteredInstances.Add(typeof(T), instance);
        }

        public void RegisterCreator<T>(Func<T> creator)
        {
            var creatorObj = creator as Func<object>;
            RegisteredCreators.Add(typeof(T), creatorObj);
        }

        public bool TryGet<TFactory>(out TFactory factory) where TFactory : class
        {
            if (!RegisteredTypes.ContainsKey(typeof(TFactory))
                && !RegisteredInstances.ContainsKey(typeof(TFactory)))
            {
                factory = null;
                return false;
            }
            else
            {
                factory = Get<TFactory>();
                return true;
            }
        }
    }
}