namespace FizzCode.DbTools.DataDefinitionExecuterMigrationIntegration.Tests
{
    using FizzCode.DbTools.Configuration;
    using Microsoft.Extensions.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public static class SetupAssemblyInitializer
    {
        public static IConfigurationRoot Configuration { get; private set; }
        public static ConnectionStringCollection ConnectionStrings { get; private set; }

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Configuration = Common.Configuration.Load("testconfig");
            ConnectionStrings = new ConnectionStringCollection();
            ConnectionStrings.LoadFromConfiguration(Configuration);
        }
    }
}
