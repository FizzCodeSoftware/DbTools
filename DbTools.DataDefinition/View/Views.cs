namespace FizzCode.DbTools.DataDefinition.View
{
    using System.Linq;

    internal class Views : AbstractTables<SqlView>
    {
        protected override void EnsureSorted()
        {
            if (_sorted.Count != byName.Count)
            {
                var i = 0;
                foreach (var sqlView in byName.OrderBy(n => n.Key).Select(n => n.Value))
                    _sorted.Add(i++, sqlView);
            }
        }
    }
}
