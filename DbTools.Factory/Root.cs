namespace FizzCode.DbTools.Factory
{
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.Loader;
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

            /*var executionFolder = Path.GetDirectoryName(typeof(Root).Assembly.Location);
            Debug.WriteLine("HERE");
            Debug.WriteLine(executionFolder);
            
            AssemblyLoadContext.Default.Resolving += (AssemblyLoadContext context, AssemblyName assembly) => {
                Debug.WriteLine(assembly.Name);
                return context.LoadFromAssemblyPath(Path.Combine(executionFolder, $"{assembly.Name}.dll"));
                };*/

            // var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            // var referencedAssemblies = someAssembly.GetReferencedAssemblies();

            // ForceLoadLocalDllsToAppDomain();

            // var loadedAssemblies2 = AppDomain.CurrentDomain.GetAssemblies();

            var builder = new ContainerBuilder();
            builder.RegisterModule(module);
            //builder.RegisterType<ISqlExecuterFactory>();
            //builder.RegisterType<TodayWriter>().As<IDateWriter>();
            Container = builder.Build();
        }

        public void TempRetreive()
        {
            using (var scope = Container.BeginLifetimeScope())
            {
                var writer = scope.Resolve<int>();
            }
        }

        public TFactory Get<TFactory>() where TFactory : notnull
        {
            var scope = Container.BeginLifetimeScope();
            return scope.Resolve<TFactory>();
        }

        private static void ForceLoadLocalDllsToAppDomain()
        {
            var selfFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var localDllFileNames = Directory.GetFiles(selfFolder, "*.dll", SearchOption.TopDirectoryOnly)
                .Where(x =>
                {
                    var fn = Path.GetFileName(x);
                    if (fn.Equals("testhost.dll", StringComparison.InvariantCultureIgnoreCase))
                        return false;

                    if (fn.StartsWith("Microsoft", StringComparison.InvariantCultureIgnoreCase))
                        return false;

                    if (fn.StartsWith("System", StringComparison.InvariantCultureIgnoreCase))
                        return false;

                    return true;
                });

            foreach (var fn in localDllFileNames)
            {
                var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic);
                var match = false;
                foreach (var loadedAssembly in loadedAssemblies)
                {
                    try
                    {
                        if (string.Equals(fn, loadedAssembly.Location, StringComparison.InvariantCultureIgnoreCase))
                        {
                            match = true;
                            break;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                if (!match)
                {
                    Debug.WriteLine("loading " + fn);
                    try
                    {
                        Assembly.LoadFile(fn);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
    }
}