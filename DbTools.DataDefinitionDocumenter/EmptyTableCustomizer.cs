namespace FizzCode.DbTools.DataDefinitionDocumenter
{

    public class EmptyTableCustomizer : ITableCustomizer
    {
        public string BackGroundColor(string tableName)
        {
            return null;
        }

        public string Category(string tableName)
        {
            return null;
        }

        public bool ShouldSkip(string tableName)
        {
            return false;
        }
    }
}
