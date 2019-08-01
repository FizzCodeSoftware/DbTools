namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;

    public class SqlTable
    {
        public string Name { get; protected set; }

        public SqlTable(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public List<SqlTableProperty> Properties { get; } = new List<SqlTableProperty>();
        public Dictionary<string, SqlColumn> Columns { get; } = new Dictionary<string, SqlColumn>();

        public SqlColumn this[string columnName] => Columns[columnName];
    }
}
