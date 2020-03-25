namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    public class GeneratorContext : DocumenterContextBase
    {
        public GeneratorSettings GeneratorSettings { get; set; }

        public override T GetDocumenterSettings<T>()
        {
            return GeneratorSettings as T;
        }
    }
}
