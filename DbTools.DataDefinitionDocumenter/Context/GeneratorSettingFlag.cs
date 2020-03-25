namespace FizzCode.DbTools.DataDefinitionDocumenter
{
    public enum GeneratorSettingFlag
    {
        None,
        NoForeignKeys,
        NoIndexes,
        NoUniqueConstraints,
        ShouldCommentOutColumnsWithFkReferencedTables,
        SholdCommentOutFkReferences
    }
}
