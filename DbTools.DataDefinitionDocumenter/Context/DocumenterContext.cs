using FizzCode.DbTools.Common;

namespace FizzCode.DbTools.DataDefinitionDocumenter;

public class DocumenterContext : DocumenterContextBase
{
    public required DocumenterSettings DocumenterSettings { get; init; }

    public override T GetDocumenterSettings<T>()
    {
        return Throw.IfNull(DocumenterSettings as T);
    }
}
