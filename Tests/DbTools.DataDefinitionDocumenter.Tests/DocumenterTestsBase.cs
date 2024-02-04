namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests
{
    using FizzCode.DbTools.TestBase;

    public class DocumenterTestsBase
    {
        public void Document(TestDatabaseDeclaration dd, SqlEngineVersion version)
        {
            dd.SetVersions(version);
            var documenter = new Documenter(DocumenterTestsHelper.CreateTestDocumenterContext(version), version, dd.GetType().Name, dd.GetType().Name + "_" + version + ".xlsx");

            documenter.Document(dd);
        }
    }    
}