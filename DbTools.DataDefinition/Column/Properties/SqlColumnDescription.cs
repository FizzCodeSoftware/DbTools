namespace FizzCode.DbTools.DataDefinition
{
    /// <summary>
    /// Contains the <see cref="Description"/>, to document the given column.
    /// </summary>
    public class SqlColumnDescription : SqlColumnProperty
    {
        public SqlColumnDescription(SqlColumnBase sqlColumn, string description)
            : base(sqlColumn)
        {
            Description = description;
        }

        public string Description { get; set; }
    }
}