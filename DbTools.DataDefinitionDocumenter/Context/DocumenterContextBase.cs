namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using FizzCode.DbTools.Common;

    public abstract class DocumenterContextBase : ContextWithLogger
    {
        public ITableCustomizer Customizer { get; set; }

        public abstract T GetDocumenterSettings<T>() where T : DocumenterSettingsBase;
    }
}
