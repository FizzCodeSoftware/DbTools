namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;

    public class Index : IndexBase
    {
        public List<SqlColumn> Includes { get; set; } = new List<SqlColumn>();

        public Index(SqlTable sqlTable, string name, bool unique = false)
            : base(sqlTable, name, unique)
        {
        }
    }
}
