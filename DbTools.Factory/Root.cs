using Autofac;
using Autofac.Configuration;
using FizzCode.DbTools.Factory.Interfaces;
using Microsoft.Extensions.Configuration;

namespace FizzCode.DbTools.Factory;
public class Root : IFactoryContainer
{
    protected static IContainer Container { get; set; }
    protected static List<(Type, Type)> RegisteredTypes { get; set; } = new();
    protected static List<(Type, object)> RegisteredInstances { get; set; } = new();
    private static bool _isInitialized;

    protected void Build()
    {
        var builder = new ContainerBuilder();

        if (File.Exists("config.json"))
        {
            var config = new ConfigurationBuilder();

            config.AddJsonFile("config.json");
            var module = new ConfigurationModule(config.Build());

            builder.RegisterModule(module);
        }

        var miRegisterType = typeof(RegistrationExtensions).GetMethods().Where(m => m.Name == "RegisterType" && m.IsGenericMethod).First();
        foreach (var (type1, type2) in RegisteredTypes)
        {
            var registerTypeRef = miRegisterType.MakeGenericMethod(type1);
            var result = registerTypeRef.Invoke(builder, new object[] { builder });
            var miAs = result.GetType().GetMethods().Where(m => m.Name == "As" && m.IsGenericMethod).First();
            var asRef = miAs.MakeGenericMethod(type2);
            asRef.Invoke(result, null);
        }

        var miRegisterInstance = typeof(RegistrationExtensions).GetMethods().Where(m => m.Name == "RegisterInstance" && m.IsGenericMethod).First();
        foreach (var (type, instance) in RegisteredInstances)
        {
            var registerTypeRef = miRegisterInstance.MakeGenericMethod(type);
            var result = registerTypeRef.Invoke(builder, new object[] { builder, instance });
        }

        Container = builder.Build();
    }

    public TFactory Get<TFactory>() where TFactory : notnull
    {
        if (!_isInitialized)
        {
            Build();
            _isInitialized = true;
        }
        var scope = Container.BeginLifetimeScope();
        return scope.Resolve<TFactory>();
    }

    public bool TryGet<TFactory>(out TFactory? factory) where TFactory : class
    {
        if (!_isInitialized)
        {
            Build();
            _isInitialized = true;
        }
        var scope = Container.BeginLifetimeScope();
        var succeed = scope.TryResolve(typeof(TFactory), out var factoryObj);
        factory = factoryObj as TFactory;
        return succeed;

    }

    public void Register<TFactory>(Type implementationType) where TFactory : notnull
    {
        RegisteredTypes.Add((typeof(TFactory), implementationType));
    }
    public void RegisterInstance<T>(T instance)
    {
        if (instance == null)
            throw new ArgumentNullException(nameof(instance));

        RegisteredInstances.Add((typeof(T), instance));
    }
}