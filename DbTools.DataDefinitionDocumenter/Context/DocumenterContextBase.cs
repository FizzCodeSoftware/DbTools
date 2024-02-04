using FizzCode.DbTools.Common;

namespace FizzCode.DbTools.DataDefinitionDocumenter;
public abstract class DocumenterContextBase : ContextWithLogger
{
    public ITableCustomizer Customizer { get; set; }

    public abstract T GetDocumenterSettings<T>() where T : DocumenterSettingsBase;
}
