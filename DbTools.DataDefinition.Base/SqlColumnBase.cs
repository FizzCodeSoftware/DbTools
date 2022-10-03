namespace FizzCode.DbTools.DataDefinition.Base
{
    using System.Collections.Generic;
    using System.Linq;

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

        protected override IDatabaseDefinition DatabaseDefinition => SqlTableOrView.DatabaseDefinition;
    }
}