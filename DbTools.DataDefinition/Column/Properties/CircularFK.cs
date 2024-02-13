using System.Collections.Generic;
using System.Linq;
using System.Text;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinition;
public class CircularFK(SqlTable sqlTable)
    : SqlTableOrViewPropertyBase<SqlTable>(sqlTable)
{
    public List<ForeignKey> ForeignKeyChain { get; set; } = [];
    
    public SqlTable SqlTable { get => SqlTableOrView!; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(ForeignKeyChain.Count)
            .Append(' ')
            .AppendJoin(", ", ForeignKeyChain.Select(fk => fk.ToString()));

        return sb.ToString();
    }
}
