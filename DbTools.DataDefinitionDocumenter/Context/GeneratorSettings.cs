namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    public class GeneratorSettings : DocumenterSettingsBase
    {
        public bool ShouldCommentOutColumnsWithFkReferencedTables { get; set; }
        public bool SholdCommentOutFkReferences { get; set; }
    }
}
