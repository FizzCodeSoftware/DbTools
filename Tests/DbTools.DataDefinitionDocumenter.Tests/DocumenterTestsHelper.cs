using FizzCode.DbTools.TestBase;

namespace FizzCode.DbTools.DataDefinitionDocumenter.Tests;
public static class DocumenterTestsHelper
{
    public static DocumenterContext CreateTestDocumenterContext(SqlEngineVersion version, ITableCustomizer customizer = null)
    {
        var context = new DocumenterContext
        {
            Settings = TestHelper.GetDefaultTestSettings(version),
            DocumenterSettings = new DocumenterSettings(),
            Customizer = customizer ?? new EmptyTableCustomizer(),
            Logger = TestHelper.CreateLogger()
        };

        return context;
    }

    public static ChangeDocumenterContext CreateTestChangeContext(SqlEngineVersion version, ITableCustomizer customizerOriginal = null, ITableCustomizer customizerNew = null)
    {
        var documenterContext = CreateTestDocumenterContext(version, null);

        var changeDocumenterContext = new ChangeDocumenterContext
        {
            Settings = documenterContext.Settings,
            DocumenterSettings = documenterContext.DocumenterSettings,
            Logger = documenterContext.Logger,
            CustomizerOriginal = customizerOriginal ?? new EmptyTableCustomizer(),
            CustomizerNew = customizerNew ?? new EmptyTableCustomizer()
        };

        return changeDocumenterContext;
    }

    public static GeneratorContext CreateTestGeneratorContext(SqlEngineVersion version, ITableCustomizer customizer = null)
    {
        var context = new GeneratorContext
        {
            Settings = TestHelper.GetDefaultTestSettings(version),
            GeneratorSettings = new GeneratorSettings(),
            Customizer = customizer ?? new EmptyTableCustomizer(),
            Logger = TestHelper.CreateLogger()
        };

        return context;
    }
}