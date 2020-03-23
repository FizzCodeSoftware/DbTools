namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    public class DocumenterSettings
    {
        public string WorkingDirectory { get; set; }

        public bool NoForeignKeys { get; set; }
        public bool NoIndexes { get; set; }
        public bool NoUniqueConstraints { get; set; }
        public bool NoInternalDataTypes { get; set; }
    }
}
