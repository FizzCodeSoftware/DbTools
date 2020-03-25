namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    public abstract class DocumenterSettingsBase
    {
        public string WorkingDirectory { get; set; }

        public bool NoForeignKeys { get; set; }

        public bool NoIndexes { get; set; }

        public bool NoUniqueConstraints { get; set; }
    }
}
