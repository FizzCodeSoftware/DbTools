namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Linq;

    public class SqlViewColumn : SqlColumnBase
    {
        public SqlView View { get => (SqlView)SqlTableOrView; set => SqlTableOrView = value; }
    }
    
    public class SqlColumn : SqlColumnBase
    {
        public SqlTable Table { get => (SqlTable)SqlTableOrView; set => SqlTableOrView = value; }

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

        public SqlColumn CopyTo(SqlColumn column)
        {
            var copy = base.CopyTo(column);
            return (SqlColumn)copy;
        }
    }

    public abstract class SqlColumnBase : SqlElementWithNameAndType
    {
        public SqlTableOrView SqlTableOrView { get; set; }

        private List<SqlColumnProperty> _properties;
        public List<SqlColumnProperty> Properties => _properties ??= new List<SqlColumnProperty>();

        public override string ToString()
        {
            return $"{Name} {Types.Describe(SqlTableOrView?.DatabaseDefinition?.MainVersion)} on {SqlTableOrView.SchemaAndTableName}";
        }

        public SqlColumnBase CopyTo(SqlColumnBase column)
        {
            column.Name = Name;
            Types.CopyTo(column.Types);
            column.SqlTableOrView = SqlTableOrView;
            return column;
        }
        
        public bool HasProperty<T>()
            where T : SqlColumnProperty
        {
            return Properties.Any(x => x is T);
        }

        protected override DatabaseDefinition DatabaseDefinition => SqlTableOrView.DatabaseDefinition;
    }
}