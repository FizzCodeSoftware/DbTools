namespace FizzCode.DbTools.TestBase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.DataDeclaration;
    using FizzCode.DbTools.DataDefinition.Factory;
    using FizzCode.DbTools.DataDefinition.Factory.Interfaces;
    using FizzCode.DbTools.Factory.Interfaces;

    public class TestDatabaseDeclaration : DatabaseDeclaration
    {
        protected TestDatabaseDeclaration()
            : base(new TestFactoryContainer(), MsSqlVersion.MsSql2016, new SqlEngineVersion[] { OracleVersion.Oracle12c, SqLiteVersion.SqLite3 })
        {
        }

        public void SetVersions(SqlEngineVersion mainVersion, SqlEngineVersion[] secondaryVersions = null)
        {
            MainVersion = mainVersion;
            SecondaryVersions = secondaryVersions?.ToList();
        }
    }

    public class TestFactoryContainer : IFactoryContainer
    {
        protected Dictionary<Type, Type> RegisteredTypes { get; set; } = new();

        public TestFactoryContainer()
        {
            Register<ITypeMapperFactory>(typeof(TypeMapperFactory));
            Register<ISqlGeneratorFactory>(typeof(SqlGeneratorFactory));
            Register<ISqlExecuterFactory>(typeof(SqlExecuterFactory));
        }

        public TFactory Get<TFactory>() where TFactory : notnull
        {
            var factoryType = RegisteredTypes[typeof(TFactory)];
            var instance = (TFactory)Activator.CreateInstance(factoryType);
            return instance;
        }

        public void Register<TFactory>(Type implementationType) where TFactory : notnull
        {
            RegisteredTypes.Add(typeof(TFactory), implementationType);
        }

        public bool TryGet<TFactory>(out TFactory factory) where TFactory : class
        {
            if (!RegisteredTypes.ContainsKey(typeof(TFactory)))
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