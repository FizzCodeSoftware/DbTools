namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Linq;
    using FizzCode.DbTools.Configuration;

    public class SqlColumn
    {
        public SqlTable Table { get; set; }
        public string Name { get; set; }

        public SqlTypes Types { get; } = new SqlTypes();

        private List<SqlColumnProperty> _properties;
        public List<SqlColumnProperty> Properties => _properties ?? (_properties = new List<SqlColumnProperty>());

        public override string ToString()
        {
            return $"{Name} {Types.Describe()} on {Table.SchemaAndTableName}";
        }

        public SqlColumn CopyTo(SqlColumn column)
        {
            column.Name = Name;
            Types.CopyTo(column.Types);
            column.Table = Table;
            return column;
        }

        public SqlColumn SetPK()
        {
            Table.SetPK(this);
            return this;
        }

        public SqlColumn SetIdentity()
        {
            Properties.Add(new Identity(this));
            return this;
        }

        public bool HasProperty<T>()
            where T : SqlColumnProperty
        {
            return Properties.Any(x => x is T);
        }
    }
}