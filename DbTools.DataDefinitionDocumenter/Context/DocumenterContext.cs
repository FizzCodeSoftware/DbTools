namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    public class DocumenterContext : DocumenterContextBase
    {
        public DocumenterSettings DocumenterSettings { get; set; }

        public override T GetDocumenterSettings<T>()
        {
            return DocumenterSettings as T;
        }
    }
}
