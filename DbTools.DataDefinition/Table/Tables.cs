namespace FizzCode.DbTools.DataDefinition
{
    using System.Linq;

    internal class Tables : AbstractTables<SqlTable>
    {
        protected override void EnsureSorted()
        {
            if (_sorted.Count != byName.Count)
            {
                _sorted = TableSorter.GetSortedTables(byName.Values.ToList());
            }
        }
    }
}
