namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Linq;

    public class SqlColumn : SqlElementWithNameAndType
    {
        public SqlTable Table { get; set; }

        private List<SqlColumnProperty> _properties;
        public List<SqlColumnProperty> Properties => _properties ??= new List<SqlColumnProperty>();

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

        protected override DatabaseDefinition DatabaseDefinition => Table.DatabaseDefinition;
    }
}