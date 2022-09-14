namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class CircularFK : SqlTableOrViewPropertyBase<SqlTable>
    {
        public List<ForeignKey> ForeignKeyChain { get; set; } = new List<ForeignKey>();
        
        public SqlTable SqlTable { get => SqlTableOrView; }

        public CircularFK(SqlTable sqlTable)
            : base(sqlTable)
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(ForeignKeyChain.Count)
                .Append(' ')
                .AppendJoin(", ", ForeignKeyChain.Select(fk => fk.ToString()));

            return sb.ToString();
        }
    }
}
