namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Linq;

    public class SqlColumn
    {
        public SqlTable Table { get; set; }
        public string Name { get; set; }

        public SqlTypes Types { get; } = new SqlTypes();

        private List<SqlColumnProperty> _properties;
        public List<SqlColumnProperty> Properties => _properties ?? (_properties = new List<SqlColumnProperty>());

        public override string ToString()
        {
            return $"{Name} {Types.Describe(Table?.DatabaseDefinition?.MainVersion)} on {Table.SchemaAndTableName}";
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

        public SqlType Type
        {
            get
            {
                if (Table.DatabaseDefinition.MainVersion != null)
                    return Types[Table.DatabaseDefinition.MainVersion];

                if (Types.Count == 1)
                    return Types.First().Value;

                return null;
            }
        }
    }
}