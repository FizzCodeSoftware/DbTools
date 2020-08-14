namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Text;

    public class ForeignKey : SqlTableProperty
    {
        public string Name { get; set; }

        public List<ForeignKeyColumnMap> ForeignKeyColumns { get; set; } = new List<ForeignKeyColumnMap>();

        public SqlTable ReferredTable { get; }

        public ForeignKey(SqlTable table, SqlTable referredTable, string name)
            : base(table)
        {
            Name = name;
            ReferredTable = referredTable;
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
                    .Append('.')
                    .Append(fkColumn.ForeignKeyColumn.Name)
                    .Append(" -> ")
                    .Append(fkColumn.ReferredColumn.Table.SchemaAndTableName)
                    .Append('.')
                    .Append(fkColumn.ReferredColumn.Name);
            }

            return sb.ToString();
        }
    }
}
