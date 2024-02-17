using FizzCode.DbTools.Common;

namespace FizzCode.DbTools.DataDefinitionDocumenter;

public class GeneratorContext : DocumenterContextBase
{
    public required GeneratorSettings GeneratorSettings { get; init; }

    public override T GetDocumenterSettings<T>()
    {
        return Throw.IfNull(GeneratorSettings as T);
    }
}
