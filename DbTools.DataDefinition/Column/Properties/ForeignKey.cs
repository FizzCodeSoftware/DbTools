namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Text;

    public class ForeignKey : SqlTableProperty
    {
        public List<ForeignKeyColumnMap> ForeignKeyColumns { get; set; } = new List<ForeignKeyColumnMap>();
        public PrimaryKey PrimaryKey { get; set; }

        public string Name { get; set; }

        public ForeignKey(SqlTable table, PrimaryKey pk, string name)
            : base(table)
        {
            PrimaryKey = pk;
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
                    .Append(fkColumn.PrimaryKeyColumn.Table.SchemaAndTableName)
                    .Append(".")
                    .Append(fkColumn.PrimaryKeyColumn.Name);
            }

            return sb.ToString();
        }
    }
}
