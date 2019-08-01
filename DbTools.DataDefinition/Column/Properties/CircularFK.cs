namespace FizzCode.DbTools.DataDefinition
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class CircularFK : SqlTableProperty
    {
        public List<ForeignKey> ForeignKeyChain { get; set; } = new List<ForeignKey>();

        public CircularFK(SqlTable sqlTable)
            : base(sqlTable)
        {
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(ForeignKeyChain.Count)
                .Append(" ")
                .Append(
                string.Join(", ", ForeignKeyChain.Select(fk => fk.ToString()))
                );

            return sb.ToString();
        }
    }
}
