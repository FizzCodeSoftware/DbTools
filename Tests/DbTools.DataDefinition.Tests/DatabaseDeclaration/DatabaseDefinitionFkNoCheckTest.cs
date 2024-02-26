using System.Linq;
using FizzCode.DbTools.DataDefinition.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FizzCode.DbTools.DataDefinition.Tests.DatabaseDeclaration;

[TestClass]
public class DatabaseDefinitionFkNoCheckTest
{
    [TestMethod]
    public void FkNoCheckTest()
    {
        var dd_ = new TestDatabaseFkNoCheckTest();

        var property = dd_.GetTable("Foreign").Properties.OfType<ForeignKey>().First().SqlEngineVersionSpecificProperties.First();

        Assert.AreEqual(MsSqlVersion.MsSql2016, property.Version);
        Assert.AreEqual("Nocheck", property.Name);
        Assert.AreEqual("true", property.Value);
    }
}
