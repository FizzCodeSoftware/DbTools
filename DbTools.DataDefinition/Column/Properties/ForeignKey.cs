namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Text;

    public abstract class ForeignKeyBase : SqlTableProperty
    {
        public string Name { get; set; }
        public SchemaAndTableName ReferredTableName { get; }

        public ForeignKeyBase(SqlTable table, SchemaAndTableName referredTableName, string name) : base(table)
        {
            Name = name;
            ReferredTableName = referredTableName;
        }
    }

    public class ForeignKey : ForeignKeyBase
    {
        public List<ForeignKeyColumnMap> ForeignKeyColumns { get; set; } = new List<ForeignKeyColumnMap>();

        public SqlTable ReferredTable => SqlTable.DatabaseDefinition.GetTable(ReferredTableName);

        public ForeignKey(SqlTable table, SchemaAndTableName referredTableName, string name)
            : base(table, referredTableName, name)
        {
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
