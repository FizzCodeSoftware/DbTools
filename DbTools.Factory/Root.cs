namespace FizzCode.DbTools.Factory
{
    using Autofac;
    using Autofac.Configuration;
    using Microsoft.Extensions.Configuration;

    public class Root
    {
        private static IContainer Container { get; set; }

        public Root()
        {
            var config = new ConfigurationBuilder();
            config.AddJsonFile("config.json");
            var module = new ConfigurationModule(config.Build());

            var builder = new ContainerBuilder();
            builder.RegisterModule(module);
            Container = builder.Build();
        }

        public TFactory Get<TFactory>() where TFactory : notnull
        {
            var scope = Container.BeginLifetimeScope();
            return scope.Resolve<TFactory>();
        }
    }
}