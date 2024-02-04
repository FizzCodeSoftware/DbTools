namespace FizzCode.DbTools.QueryBuilder;

public enum QueryColumnAliasStrategy
{
    /// <summary>
    /// Prefix column names with table name IF there is already a column with the same name
    /// Example: SELECT p.Id, c.Id as 'ChildId' ...
    /// </summary>
    PrefixTableNameIfNeeded,
    /// <summary>
    /// Prefix column names with table name IF there is already a column with the same name
    /// Example: SELECT p.Id, c.Id as 'c_Id' ...
    /// </summary>
    PrefixTableAliasIfNeeded,
    /// <summary>Use Column Names, enabling SELECT with duplicate column names</summary>
    EnableDuplicates,
    PrefixTableNameAlways,
    PrefixTableAliasAlways

}
