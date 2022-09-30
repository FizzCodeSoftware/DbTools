namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    using FizzCode.DbTools.DataDefinition.Base;

    public class EmptyTableCustomizer : ITableCustomizer
    {
        public string BackGroundColor(SchemaAndTableName tableName)
        {
            return null;
        }

        public string Category(SchemaAndTableName tableName)
        {
            return null;
        }

        public bool ShouldSkip(SchemaAndTableName tableName)
        {
            return false;
        }
    }
}
