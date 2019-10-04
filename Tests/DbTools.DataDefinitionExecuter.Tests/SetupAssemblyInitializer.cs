namespace DbTools.DataDefinitionExecuter.Tests
{
    using FizzCode.DbTools.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SetupAssemblyInitializer
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Configuration.Load("testconfig");
        }
    }
}
