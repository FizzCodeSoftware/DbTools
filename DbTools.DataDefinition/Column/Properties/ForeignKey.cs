namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Text;

    public class ForeignKey : SqlTableProperty
    {
        public List<ForeignKeyColumnMap> ForeignKeyColumns { get; set; } = new List<ForeignKeyColumnMap>();
        public string Name { get; set; }

        public SqlTable ReferredTable => SqlTable.DatabaseDefinition.GetTable(_referredTableName);
        private readonly SchemaAndTableName _referredTableName;

        public ForeignKey(SqlTable table, SchemaAndTableName referredTableName, string name)
            : base(table)
        {
            _referredTableName = referredTableName;
            Name = name;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            var isFirst = true;
            foreach (var fkColumn in ForeignKeyColumns)
            {
                if (!isFirst)
                    sb.Append(", ");
                else
                    isFirst = false;

                sb.Append(fkColumn.ForeignKeyColumn.Table.SchemaAndTableName)
                    .Append(".")
                    .Append(fkColumn.ForeignKeyColumn.Name)
                    .Append(" -> ")
                    .Append(fkColumn.ReferredColumn.Table.SchemaAndTableName)
                    .Append(".")
                    .Append(fkColumn.ReferredColumn.Name);
            }

            return sb.ToString();
        }
    }
}
