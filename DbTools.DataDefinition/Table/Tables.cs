using System.Linq;
using FizzCode.DbTools.DataDefinition.Base;

namespace FizzCode.DbTools.DataDefinition;
public class Tables : AbstractTables<SqlTable>
{
    protected override void EnsureSorted()
    {
        if (_sorted.Count != byName.Count)
        {
            _sorted = TableSorter.GetSortedTables(byName.Values.ToList());
        }
    }
}
