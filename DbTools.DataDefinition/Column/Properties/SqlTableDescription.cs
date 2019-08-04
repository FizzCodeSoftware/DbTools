namespace FizzCode.DbTools.DataDefinition
{
    /// <summary>
    /// Contains the <see cref="SqlTableDescription.Description"/>, to document the given table.
    /// </summary>
    public class SqlTableDescription : SqlTableProperty
    {
        public SqlTableDescription(SqlTable sqlTable, string description)
            : base(sqlTable)
        {
            Description = description;
        }

        public string Description { get; set; }
    }
}
